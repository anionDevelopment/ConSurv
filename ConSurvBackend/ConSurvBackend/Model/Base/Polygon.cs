using System.Collections.Generic;

namespace ConSurvBackend.Core.Model.Base
{
    /// <summary>
    /// Represents a closed polygon defined by an ordered list of <see cref="Point"/> vertices,
    /// used to mark regions of interest within a camera's overlay.
    /// </summary>
    public class Polygon
    {
        public IList<Point> Points { get; internal set; } = new List<Point>();
    }
}
