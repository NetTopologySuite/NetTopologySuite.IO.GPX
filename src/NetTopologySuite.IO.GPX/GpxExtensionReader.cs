using System.Collections.Generic;
using System.Xml.Linq;

namespace NetTopologySuite.IO
{
    /// <summary>
    /// Provides hooks to interpret arbitrary GPX extension element content as (perhaps) something
    /// with a richer, more type-safe data model.
    /// <para>
    /// The default behavior in this base class preserves the <see cref="XElement"/> representation
    /// of the source data, so that a <see cref="GpxExtensionWriter"/> in its own default mode will
    /// be able to write it back out again.
    /// </para>
    /// </summary>
    public class GpxExtensionReader
    {
        /// <summary>
        /// Transforms a sequence of <see cref="XElement"/> instances that represent an "extensions"
        /// element's content into what <see cref="GpxVisitorBase.VisitExtensions"/> should observe.
        /// <para>
        /// If <c>/gpx/extensions</c> does not exist, then this method will not be called.
        /// </para>
        /// <para>
        /// If <c>/gpx/extensions/*</c> selects no elements, then this method will be called with an
        /// empty sequence of elements.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if <c>/gpx/extensions</c> did not exist at all.
        /// </para>
        /// </summary>
        /// <param name="extensionElements">
        /// The individual elements of the "extensions" element's content.
        /// </param>
        /// <returns>
        /// The value that <see cref="GpxVisitorBase.VisitExtensions"/> should observe.
        /// </returns>
        public virtual object ConvertGpxExtensionElement(IEnumerable<XElement> extensionElements) => ConvertExtensionElementsCommon(extensionElements);

        /// <summary>
        /// Transforms a sequence of <see cref="XElement"/> instances that represent an "extensions"
        /// element's content into a value suitable for <see cref="GpxMetadata.Extensions"/>.
        /// <para>
        /// If <c>/gpx/metadata/extensions</c> does not exist, then this method will not be called.
        /// </para>
        /// <para>
        /// If <c>/gpx/metadata/extensions/*</c> selects no elements, then this method will be
        /// called with an empty sequence of elements.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if <c>/gpx/metadata/extensions</c> did not exist at all.
        /// </para>
        /// </summary>
        /// <param name="extensionElements">
        /// The individual elements of the "extensions" element's content.
        /// </param>
        /// <returns>
        /// The value to store in <see cref="GpxMetadata.Extensions"/>.
        /// </returns>
        public virtual object ConvertMetadataExtensionElement(IEnumerable<XElement> extensionElements) => ConvertExtensionElementsCommon(extensionElements);

        /// <summary>
        /// Transforms a sequence of <see cref="XElement"/> instances that represent an "extensions"
        /// element's content into a value suitable for <see cref="GpxWaypoint.Extensions"/>, when
        /// stored in a waypoint that <see cref="GpxVisitorBase.VisitWaypoint"/> will observe.
        /// <para>
        /// If <c>wpt/extensions</c> does not exist, then this method will not be called for that
        /// node.
        /// </para>
        /// <para>
        /// If <c>wpt/extensions/*</c> selects no elements, then this method will be called for
        /// that node, with an empty sequence of elements.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if <c>wpt/extensions</c> did not exist at all.
        /// </para>
        /// </summary>
        /// <param name="extensionElements">
        /// The individual elements of the "extensions" element's content.
        /// </param>
        /// <returns>
        /// The value to store in <see cref="GpxWaypoint.Extensions"/>.
        /// </returns>
        public virtual object ConvertWaypointExtensionElement(IEnumerable<XElement> extensionElements) => ConvertExtensionElementsCommon(extensionElements);

        /// <summary>
        /// Transforms a sequence of <see cref="XElement"/> instances that represent an "extensions"
        /// element's content into a value suitable for <see cref="GpxRoute.Extensions"/>.
        /// <para>
        /// If <c>rte/extensions</c> does not exist, then this method will not be called for that
        /// node.
        /// </para>
        /// <para>
        /// If <c>rte/extensions/*</c> selects no elements, then this method will be called for
        /// that node, with an empty sequence of elements.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if <c>rte/extensions</c> did not exist at all.
        /// </para>
        /// </summary>
        /// <param name="extensionElements">
        /// The individual elements of the "extensions" element's content.
        /// </param>
        /// <returns>
        /// The value to store in <see cref="GpxRoute.Extensions"/>.
        /// </returns>
        public virtual object ConvertRouteExtensionElement(IEnumerable<XElement> extensionElements) => ConvertExtensionElementsCommon(extensionElements);

        /// <summary>
        /// Transforms a sequence of <see cref="XElement"/> instances that represent an "extensions"
        /// element's content into a value suitable for <see cref="GpxWaypoint.Extensions"/>, when
        /// stored in a <see cref="GpxRoute"/> instance's table.
        /// <para>
        /// If <c>rtept/extensions</c> does not exist, then this method will not be called for that
        /// node.
        /// </para>
        /// <para>
        /// If <c>rtept/extensions/*</c> selects no elements, then this method will be called for
        /// that node, with an empty sequence of elements.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if <c>rtept/extensions</c> did not exist at all.
        /// </para>
        /// </summary>
        /// <param name="extensionElements">
        /// The individual elements of the "extensions" element's content.
        /// </param>
        /// <returns>
        /// The value to store in <see cref="GpxWaypoint.Extensions"/>.
        /// </returns>
        public virtual object ConvertRoutePointExtensionElement(IEnumerable<XElement> extensionElements) => ConvertExtensionElementsCommon(extensionElements);

        /// <summary>
        /// Transforms a sequence of <see cref="XElement"/> instances that represent an "extensions"
        /// element's content into a value suitable for <see cref="GpxTrack.Extensions"/>.
        /// <para>
        /// If <c>trk/extensions</c> does not exist, then this method will not be called for that
        /// node.
        /// </para>
        /// <para>
        /// If <c>trk/extensions/*</c> selects no elements, then this method will be called for
        /// that node, with an empty sequence of elements.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if <c>trk/extensions</c> did not exist at all.
        /// </para>
        /// </summary>
        /// <param name="extensionElements">
        /// The individual elements of the "extensions" element's content.
        /// </param>
        /// <returns>
        /// The value to store in <see cref="GpxTrack.Extensions"/>.
        /// </returns>
        public virtual object ConvertTrackExtensionElement(IEnumerable<XElement> extensionElements) => ConvertExtensionElementsCommon(extensionElements);

        /// <summary>
        /// Transforms a sequence of <see cref="XElement"/> instances that represent an "extensions"
        /// element's content into a value suitable for <see cref="GpxTrackSegment.Extensions"/>.
        /// <para>
        /// If <c>trkseg/extensions</c> does not exist, then this method will not be called for that
        /// node.
        /// </para>
        /// <para>
        /// If <c>trkseg/extensions/*</c> selects no elements, then this method will be called for
        /// that node, with an empty sequence of elements.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if <c>trkseg/extensions</c> did not exist at all.
        /// </para>
        /// </summary>
        /// <param name="extensionElements">
        /// The individual elements of the "extensions" element's content.
        /// </param>
        /// <returns>
        /// The value to store in <see cref="GpxTrackSegment.Extensions"/>.
        /// </returns>
        public virtual object ConvertTrackSegmentExtensionElement(IEnumerable<XElement> extensionElements) => ConvertExtensionElementsCommon(extensionElements);

        /// <summary>
        /// Transforms a sequence of <see cref="XElement"/> instances that represent an "extensions"
        /// element's content into a value suitable for <see cref="GpxWaypoint.Extensions"/>, when
        /// stored in a <see cref="GpxTrackSegment"/> instance's table.
        /// <para>
        /// If <c>trkpt/extensions</c> does not exist, then this method will not be called for that
        /// node.
        /// </para>
        /// <para>
        /// If <c>trkpt/extensions/*</c> selects no elements, then this method will be called for
        /// that node, with an empty sequence of elements.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if <c>trkpt/extensions</c> did not exist at all.
        /// </para>
        /// </summary>
        /// <param name="extensionElements">
        /// The individual elements of the "extensions" element's content.
        /// </param>
        /// <returns>
        /// The value to store in <see cref="GpxWaypoint.Extensions"/>.
        /// </returns>
        public virtual object ConvertTrackPointExtensionElement(IEnumerable<XElement> extensionElements) => ConvertExtensionElementsCommon(extensionElements);

        /// <summary>
        /// Transforms a sequence of <see cref="XElement"/> instances that represent an "extensions"
        /// element's content into a value suitable for an <c>Extensions</c> property.
        /// <para>
        /// If an "extensions" element does not exist, then this method will not be called for that
        /// node.
        /// </para>
        /// <para>
        /// If the "extensions" element exists, but it has no child elements, then this method will
        /// be called for that node, with an empty sequence of elements.
        /// </para>
        /// <para>
        /// If this method returns <see langword="null"/>, then the result will be the same as it
        /// would be if the "extensions" element did not exist at all.
        /// </para>
        /// </summary>
        /// <param name="extensionElements">
        /// The individual elements of the "extensions" element's content.
        /// </param>
        /// <returns>
        /// The value to store in the corresponding <c>Extensions</c> property.
        /// </returns>
        protected virtual object ConvertExtensionElementsCommon(IEnumerable<XElement> extensionElements) => new ImmutableXElementContainer(extensionElements);
    }
}
