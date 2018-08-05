namespace NetTopologySuite.IO
{
    /// <summary>
    /// Specifies what kind of GPS fix a particular waypoint had.
    /// </summary>
    public enum GpxFixKind
    {
        /// <summary>
        /// No fix.
        /// </summary>
        None,

        /// <summary>
        /// 2D fix.
        /// </summary>
        TwoDimensional,

        /// <summary>
        /// 3D fix.
        /// </summary>
        ThreeDimensional,

        /// <summary>
        /// Differential GPS (DGPS) fix.
        /// </summary>
        DGPS,

        /// <summary>
        /// Precise Positioning Service (PPS) fix.
        /// </summary>
        PPS
    }
}
