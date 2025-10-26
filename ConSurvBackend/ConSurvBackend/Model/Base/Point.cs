namespace ConSurvBackend.Core.Model.Base
{
    public class Point
    {
        public Point(uint x, uint y)
        {
            this.X = x;
            this.Y = y;
        }

        public uint X { get; internal set; }
        public uint Y { get; internal set; }
    }
}
