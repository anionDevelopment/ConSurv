namespace ConSurvBackend.Core.Model.Base
{
    /// <summary>
    /// Represents an immutable 2-D pixel coordinate within a video frame.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Initializes a new <see cref="Point"/> with the specified pixel coordinates.
        /// </summary>
        /// <param name="x">The horizontal pixel position (0 = left edge).</param>
        /// <param name="y">The vertical pixel position (0 = top edge).</param>
        public Point(uint x, uint y)
        {
            this.X = x;
            this.Y = y;
        }

        public uint X { get; internal set; }
        public uint Y { get; internal set; }
    }
}
