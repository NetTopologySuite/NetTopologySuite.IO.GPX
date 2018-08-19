namespace NetTopologySuite.IO
{
    /// <summary>
    /// Base class for the visitors that enable all the higher-order GPX reading functionality.
    /// </summary>
    public abstract class GpxVisitorBase
    {
        /// <summary>
        /// Invoked when the reader has moved past either the GPX metadata element or where it would
        /// have been.
        /// </summary>
        /// <param name="metadata">
        /// The <see cref="GpxMetadata"/> that we read, guaranteed non-<see langword="null"/>.
        /// </param>
        /// <remarks>
        /// <para>
        /// This is guaranteed to be called once per GPX file, even if there is no metadata element.
        /// When there is no metadata element, <see cref="GpxMetadata.IsTrivial"/> will be
        /// <see langword="true"/>, and only <see cref="GpxMetadata.Creator"/> will be set.
        /// </para>
        /// <para>
        /// This will be called before any other method for a given GPX file.
        /// </para>
        /// </remarks>
        public virtual void VisitMetadata(GpxMetadata metadata) { }

        /// <summary>
        /// Invoked when the reader has moved past a GPX wpt element.
        /// </summary>
        /// <param name="waypoint">
        /// The <see cref="GpxWaypoint"/> instance that represents what we read, guaranteed non-
        /// <see langword="null"/>.
        /// </param>
        /// <remarks>
        /// This is not guaranteed to be called for every GPX file.
        /// </remarks>
        public virtual void VisitWaypoint(GpxWaypoint waypoint) { }

        /// <summary>
        /// Invoked when the reader has moved past a GPX rte element.
        /// </summary>
        /// <param name="route">
        /// The <see cref="GpxRoute"/> instance that represents what we read, guaranteed non-
        /// <see langword="null"/>.
        /// </param>
        /// <remarks>
        /// This is not guaranteed to be called for every GPX file.
        /// </remarks>
        public virtual void VisitRoute(GpxRoute route) { }

        /// <summary>
        /// Invoked when the reader has moved past a GPX trk element.
        /// </summary>
        /// <param name="track">
        /// The <see cref="GpxTrack"/> instance that represents what we read, guaranteed non-
        /// <see langword="null"/>.
        /// </param>
        /// <remarks>
        /// This is not guaranteed to be called for every GPX file.
        /// </remarks>
        public virtual void VisitTrack(GpxTrack track) { }

        /// <summary>
        /// Invoked when the reader has moved past a GPX wpt element.
        /// </summary>
        /// <param name="extensions">
        /// The object instance that represents what we read, guaranteed non-<see langword="null"/>.
        /// </param>
        /// <remarks>
        /// This is not guaranteed to be called for every GPX file.
        /// </remarks>
        public virtual void VisitExtensions(object extensions) { }
    }
}
