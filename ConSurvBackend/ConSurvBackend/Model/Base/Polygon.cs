using System.Collections.Generic;

namespace ConSurvBackend.Core.Model.Base
{
    public class Polygon
    {
        public IList<Point> Points { get; set; } = new List<Point>();
    }
}
