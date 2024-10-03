namespace ConSurvBackend.Core.Model.ZoomDirections
{
    public abstract class ZoomDirection
    {
        public abstract void Accept(IZoomDirectionVisitor visitor);
        public abstract T Accept<T>(IZoomDirectionVisitor<T> visitor);
    }
    public interface IZoomDirectionVisitor
    {
        void Handle(ZoomIn zoomIn);
        void Handle(ZoomOut zoomOut);
    }
    public interface IZoomDirectionVisitor<T>
    {
        T Handle(ZoomIn zoomIn);
        T Handle(ZoomOut zoomOut);
    }
}