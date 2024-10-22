namespace ConSurvBackend.Core.Model.ZoomDirections
{
    public class ZoomOut : ZoomDirection
    {
        public override void Accept(IZoomDirectionVisitor visitor) => visitor.Handle(this);

        public override T Accept<T>(IZoomDirectionVisitor<T> visitor) => visitor.Handle(this);
    }
}
