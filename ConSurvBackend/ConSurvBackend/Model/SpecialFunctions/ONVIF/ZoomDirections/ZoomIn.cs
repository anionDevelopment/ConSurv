namespace ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.ZoomDirections
{
    public class ZoomIn : ZoomDirection
    {
        public override void Accept(IZoomDirectionVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IZoomDirectionVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
