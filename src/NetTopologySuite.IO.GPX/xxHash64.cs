using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static System.Runtime.CompilerServices.Unsafe;

namespace NetTopologySuite.IO
{
    internal static class xxHash64
    {
        [StructLayout(LayoutKind.Auto)]
        public unsafe struct State
        {
            internal fixed ulong Values[4];
            internal fixed byte Buffer[32];
            internal ulong BytesProcessedSoFar;
            internal byte BufferUsed; // [0, 32)
        }

        private const ulong Prime1 = 11400714785074694791;
        private const ulong Prime2 = 14029467366897019727;
        private const ulong Prime3 = 1609587929392839161;
        private const ulong Prime4 = 9650029242287828579;
        private const ulong Prime5 = 2870177450012600261;

        // shortcut for Init / Add / Finish so one-shot callers don't need to worry about state.
        public static ulong Hash(ReadOnlySpan<byte> input, ulong seed = 0)
        {
            var state = Init(seed);
            Add(ref state, input);
            return Finish(ref state);
        }

        // start a streaming hash
        public static unsafe State Init(ulong seed = 0)
        {
            State state = default;
            unchecked
            {
                state.Values[0] = seed + Prime1 + Prime2;
                state.Values[1] = seed + Prime2;
                state.Values[2] = seed;
                state.Values[3] = seed - Prime1;
            }

            return state;
        }

        // add the next chunk to a streaming hash.
        public static unsafe void Add(ref State state, ReadOnlySpan<byte> input)
        {
            if (input.IsEmpty)
            {
                return;
            }

            // pin because accessing the fixed-size buffers requires it.
            fixed (State* p = &state)
            {
                p->BytesProcessedSoFar += unchecked((ulong)input.Length);

                var stateBuffer = new Span<byte>(p->Buffer, 32);
                var freeBuffer = stateBuffer.Slice(p->BufferUsed);
                if (input.Length < freeBuffer.Length)
                {
                    input.CopyTo(freeBuffer.Slice(0, input.Length));

                    p->BufferUsed = unchecked((byte)(p->BufferUsed + input.Length));
                    return;
                }

                var values = new Span<ulong>(p->Values, 4);
                if (freeBuffer.Length < 32)
                {
                    input.Slice(0, freeBuffer.Length).CopyTo(freeBuffer);
                    input = input.Slice(freeBuffer.Length);
                    Process(ref MemoryMarshal.GetReference(stateBuffer), values);
                }

                // feels like SIMD could help this loop, since each iteration only looks at the next
                // 32 bytes in independent 8-byte chunks.
                while (input.Length >= 32)
                {
                    Process(ref MemoryMarshal.GetReference(input), values);
                    input = input.Slice(32);
                }

                if (input.Length != 0)
                {
                    input.CopyTo(stateBuffer.Slice(0, input.Length));
                }

                p->BufferUsed = unchecked((byte)input.Length);
            }
        }

        // wrap up a streaming hash
        public static unsafe ulong Finish(ref State state)
        {
            unchecked
            {
                // pin because accessing the fixed-size buffers requires it.
                fixed (State* p = &state)
                {
                    var values = new Span<ulong>(p->Values, 4);

                    ulong h;
                    if (p->BytesProcessedSoFar >= 32)
                    {
                        h = RotateLeft(values[0], 1) +
                            RotateLeft(values[1], 7) +
                            RotateLeft(values[2], 12) +
                            RotateLeft(values[3], 18);

                        h = (h ^ ProcessSingle(0, values[0])) * Prime1 + Prime4;
                        h = (h ^ ProcessSingle(0, values[1])) * Prime1 + Prime4;
                        h = (h ^ ProcessSingle(0, values[2])) * Prime1 + Prime4;
                        h = (h ^ ProcessSingle(0, values[3])) * Prime1 + Prime4;
                    }
                    else
                    {
                        h = values[2] + Prime5;
                    }

                    h += p->BytesProcessedSoFar;

                    ref byte bytesStart = ref AsRef<byte>(p->Buffer);
                    int offset = 0;
                    int remainingBytes = p->BufferUsed;

                    for (; offset + 8 <= remainingBytes; offset += 8)
                    {
                        h = RotateLeft(h ^ ProcessSingle(0, ReadUnaligned<ulong>(ref AddByteOffset(ref bytesStart, new IntPtr(offset)))), 27) * Prime1 + Prime4;
                    }

                    if (offset + 4 <= remainingBytes)
                    {
                        h = RotateLeft(h ^ ReadUnaligned<uint>(ref AddByteOffset(ref bytesStart, new IntPtr(offset))), 23) * Prime2 + Prime3;
                        offset += 4;
                    }

                    while (offset < remainingBytes)
                    {
                        h = RotateLeft(h ^ AddByteOffset(ref bytesStart, new IntPtr(offset)) * Prime5, 11) * Prime1;
                        offset++;
                    }

                    h ^= h >> 33;
                    h *= Prime2;
                    h ^= h >> 29;
                    h *= Prime3;
                    h ^= h >> 32;
                    return h;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Process(ref byte data, Span<ulong> state)
        {
            state[0] = ProcessSingle(state[0], ReadUnaligned<ulong>(ref AddByteOffset(ref data, new IntPtr(sizeof(ulong) * 0))));
            state[1] = ProcessSingle(state[1], ReadUnaligned<ulong>(ref AddByteOffset(ref data, new IntPtr(sizeof(ulong) * 1))));
            state[2] = ProcessSingle(state[2], ReadUnaligned<ulong>(ref AddByteOffset(ref data, new IntPtr(sizeof(ulong) * 2))));
            state[3] = ProcessSingle(state[3], ReadUnaligned<ulong>(ref AddByteOffset(ref data, new IntPtr(sizeof(ulong) * 3))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ProcessSingle(ulong previous, ulong input) => unchecked(RotateLeft(previous + input * Prime2, 31) * Prime1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong RotateLeft(ulong value, int count) => unchecked((value << count) | (value >> (64 - count)));
    }
}
