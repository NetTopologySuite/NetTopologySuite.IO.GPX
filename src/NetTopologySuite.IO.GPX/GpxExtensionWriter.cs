using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Provides hooks to turn extension content into GPX "extensions" elements.
    /// <para>
    /// The default behavior in this base class passes through an <see cref="IEnumerable{T}"/> of
    /// <see cref="XElement"/> objects if that's what's stored, otherwise it will yield an empty
    /// sequence.  This allows us to round-trip when the default <see cref="GpxExtensionReader"/>
    /// implementation is used, and not much else.
    /// </para>
    /// </summary>
    public class GpxExtensionWriter
    {
        /// <summary>
        /// Transforms an arbitrary extension object into a sequence of <see cref="XElement"/>
        /// instances to write out as the content of <c>/gpx/extensions</c>.
        /// <para>
        /// If this method returns an empty sequence of elements, then the result will be an empty
        /// <c>/gpx/extensions</c> element.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if there were no extension content at all.
        /// </para>
        /// </summary>
        /// <param name="extension">
        /// The extension object to transform.
        /// </param>
        /// <returns>
        /// The content to write out as <c>/gpx/extensions</c>.
        /// </returns>
        public virtual IEnumerable<XElement> ConvertGpxExtension(object extension) => ConvertExtensionCommon(extension);

        /// <summary>
        /// Transforms an arbitrary extension object into a sequence of <see cref="XElement"/>
        /// instances to write out as the content of <c>/gpx/metadata/extensions</c>.
        /// <para>
        /// If this method returns an empty sequence of elements, then the result will be an empty
        /// <c>/gpx/metadata/extensions</c> element.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if there were no extension content at all.
        /// </para>
        /// </summary>
        /// <param name="extension">
        /// The extension object to transform.
        /// </param>
        /// <returns>
        /// The content to write out as <c>/gpx/metadata/extensions</c>.
        /// </returns>
        public virtual IEnumerable<XElement> ConvertMetadataExtension(object extension) => ConvertExtensionCommon(extension);

        /// <summary>
        /// Transforms an arbitrary extension object into a sequence of <see cref="XElement"/>
        /// instances to write out as the content of <c>wpt/extensions</c>.
        /// <para>
        /// If this method returns an empty sequence of elements, then the result will be an empty
        /// <c>wpt/extensions</c> element.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if there were no extension content at all.
        /// </para>
        /// </summary>
        /// <param name="extension">
        /// The extension object to transform.
        /// </param>
        /// <returns>
        /// The content to write out as <c>wpt/extensions</c>.
        /// </returns>
        public virtual IEnumerable<XElement> ConvertWaypointExtension(object extension) => ConvertExtensionCommon(extension);

        /// <summary>
        /// Transforms an arbitrary extension object into a sequence of <see cref="XElement"/>
        /// instances to write out as the content of <c>rte/extensions</c>.
        /// <para>
        /// If this method returns an empty sequence of elements, then the result will be an empty
        /// <c>rte/extensions</c> element.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if there were no extension content at all.
        /// </para>
        /// </summary>
        /// <param name="extension">
        /// The extension object to transform.
        /// </param>
        /// <returns>
        /// The content to write out as <c>rte/extensions</c>.
        /// </returns>
        public virtual IEnumerable<XElement> ConvertRouteExtension(object extension) => ConvertExtensionCommon(extension);

        /// <summary>
        /// Transforms an arbitrary extension object into a sequence of <see cref="XElement"/>
        /// instances to write out as the content of <c>rtept/extensions</c>.
        /// <para>
        /// If this method returns an empty sequence of elements, then the result will be an empty
        /// <c>rtept/extensions</c> element.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if there were no extension content at all.
        /// </para>
        /// </summary>
        /// <param name="extension">
        /// The extension object to transform.
        /// </param>
        /// <returns>
        /// The content to write out as <c>rtept/extensions</c>.
        /// </returns>
        public virtual IEnumerable<XElement> ConvertRoutePointExtension(object extension) => ConvertExtensionCommon(extension);

        /// <summary>
        /// Transforms an arbitrary extension object into a sequence of <see cref="XElement"/>
        /// instances to write out as the content of <c>trk/extensions</c>.
        /// <para>
        /// If this method returns an empty sequence of elements, then the result will be an empty
        /// <c>trk/extensions</c> element.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if there were no extension content at all.
        /// </para>
        /// </summary>
        /// <param name="extension">
        /// The extension object to transform.
        /// </param>
        /// <returns>
        /// The content to write out as <c>trk/extensions</c>.
        /// </returns>
        public virtual IEnumerable<XElement> ConvertTrackExtension(object extension) => ConvertExtensionCommon(extension);

        /// <summary>
        /// Transforms an arbitrary extension object into a sequence of <see cref="XElement"/>
        /// instances to write out as the content of <c>trkseg/extensions</c>.
        /// <para>
        /// If this method returns an empty sequence of elements, then the result will be an empty
        /// <c>trkseg/extensions</c> element.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if there were no extension content at all.
        /// </para>
        /// </summary>
        /// <param name="extension">
        /// The extension object to transform.
        /// </param>
        /// <returns>
        /// The content to write out as <c>trkseg/extensions</c>.
        /// </returns>
        public virtual IEnumerable<XElement> ConvertTrackSegmentExtension(object extension) => ConvertExtensionCommon(extension);

        /// <summary>
        /// Transforms an arbitrary extension object into a sequence of <see cref="XElement"/>
        /// instances to write out as the content of <c>trkpt/extensions</c>.
        /// <para>
        /// If this method returns an empty sequence of elements, then the result will be an empty
        /// <c>trkpt/extensions</c> element.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if there were no extension content at all.
        /// </para>
        /// </summary>
        /// <param name="extension">
        /// The extension object to transform.
        /// </param>
        /// <returns>
        /// The content to write out as <c>trkpt/extensions</c>.
        /// </returns>
        public virtual IEnumerable<XElement> ConvertTrackPointExtension(object extension) => ConvertExtensionCommon(extension);

        /// <summary>
        /// Transforms an arbitrary extension object into a sequence of <see cref="XElement"/>
        /// instances to write out as the content of an "extensions" element.
        /// <para>
        /// If this method returns an empty sequence of elements, then the result will be an empty
        /// "extensions" element.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if there were no extension content at all.
        /// </para>
        /// </summary>
        /// <param name="extension">
        /// The extension object to transform.
        /// </param>
        /// <returns>
        /// The content to write out as the content of an "extensions" element.
        /// </returns>
        protected virtual IEnumerable<XElement> ConvertExtensionCommon(object extension) => extension as IEnumerable<XElement> ?? Enumerable.Empty<XElement>();
    }
}
