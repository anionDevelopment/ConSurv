namespace ConSurvBackend.Core.Model.Base
{
    public class Point
    {
        public Point(uint x, uint y)
        {
            this.X = x;
            this.Y = y;
        }

        public uint X { get; set; }
        public uint Y { get; set; }
    }
}
