using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// A wrapper around an <see cref="ImmutableArray{T}"/> of <see cref="XElement"/> instances that
    /// are, themselves, made immutable inside this class.
    /// </summary>
    public sealed class ImmutableXElementContainer : IReadOnlyList<XElement>
    {
        private static readonly EventHandler<XObjectChangeEventArgs> DisallowXObjectChangeEventHandler = (sender, args) => throw new NotSupportedException("The base GPX data model is immutable, including the default representation of extensions.  Use .WithExtensions for changing extensions instead.");

        private readonly ImmutableArray<XNode> upcasted;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableXElementContainer"/> class.
        /// </summary>
        /// <param name="items">
        /// The items to copy to <see cref="Items"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="items"/> is <see langword="null"/>.
        /// </exception>
        public ImmutableXElementContainer(IEnumerable<XElement> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var resultBuilder = items.TryGetCount(out int count)
                ? ImmutableArray.CreateBuilder<XElement>(count)
                : ImmutableArray.CreateBuilder<XElement>();

            foreach (var item in items)
            {
                resultBuilder.Add(CloneAsImmutable(item));
            }

            resultBuilder.Capacity = resultBuilder.Count;
            this.Items = resultBuilder.MoveToImmutable();
            this.upcasted = ImmutableArray<XNode>.CastUp(this.Items);
        }

        /// <summary>
        /// Gets the <see cref="XElement"/>s contained in this container.
        /// </summary>
        public ImmutableArray<XElement> Items { get; }

        /// <inheritdoc />
        public XElement this[int index] => this.Items[index];

        /// <inheritdoc />
        public int Count => this.Items.Length;

        /// <inheritdoc cref="ImmutableArray{T}.GetEnumerator" />
        public ImmutableArray<XElement>.Enumerator GetEnumerator() => this.Items.GetEnumerator();

        /// <inheritdoc />
        IEnumerator<XElement> IEnumerable<XElement>.GetEnumerator() => ((IEnumerable<XElement>)this.Items).GetEnumerator();

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => ((System.Collections.IEnumerable)this.Items).GetEnumerator();

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is ImmutableXElementContainer other &&
                                                   this.upcasted.SequenceEqual(other.upcasted, XNode.EqualityComparer);

        /// <inheritdoc />
        public override int GetHashCode() => this.upcasted.ListToHashCode(XNode.EqualityComparer);

        private static XElement CloneAsImmutable(XElement item)
        {
            item = new XElement(item);
            item.Changing += DisallowXObjectChangeEventHandler;
            return item;
        }
    }
}
