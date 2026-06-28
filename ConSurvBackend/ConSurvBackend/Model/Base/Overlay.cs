using System.Collections.Generic;

namespace ConSurvBackend.Core.Model.Base
{
    /// <summary>
    /// Defines the overlay configuration applied to a camera's video feed,
    /// including the reference resolution and the list of motion-detection polygons.
    /// </summary>
    public class Overlay
    {
        public uint Width { get; internal set; } = 1920;
        public uint Height { get; internal set; } = 1080;
        public IList<Polygon> Polygons { get; internal set; } = new List<Polygon>() {
           //just as an example:
           //new Polygon(){ Points = new List<Point> { new Point(100, 100), new Point(400, 150), new Point(350, 400) } },
           //new Polygon(){ Points = new List<Point> { new Point(800, 300), new Point(950, 500), new Point(800, 550), new Point(650, 400) } },
        };
    }
}