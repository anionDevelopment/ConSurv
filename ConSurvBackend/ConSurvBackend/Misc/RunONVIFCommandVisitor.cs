using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.MoveDirections;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.ZoomDirections;
using Onvif.Core.Client.Camera;
using Onvif.Core.Client.Camera.Requests;
using Onvif.Core.Client.Common;
using System;
using OnvifCamera = Onvif.Core.Client.Camera.Camera;

namespace ConSurvBackend.Core.Misc
{
    /// <summary>
    /// Visitor that executes ONVIF commands (e.g. zoom, move) on a specific camera via the ONVIF protocol.
    /// </summary>
    internal class RunONVIFCommandVisitor : IONVIFCommandVisitor
    {
        private const float StepSize = 0.1f;

        private readonly Model.Base.Camera _Camera;

        /// <summary>
        /// Initializes a new instance of <see cref="RunONVIFCommandVisitor"/> for the given camera.
        /// </summary>
        /// <param name="camera">The camera on which ONVIF commands are executed.</param>
        public RunONVIFCommandVisitor(Model.Base.Camera camera)
        {
            this._Camera = camera;
        }

        /// <inheritdoc />
        public void Handle(Zoom zoom)
        {
            float delta = zoom.ZoomDirection.Accept(new ZoomDeltaVisitor());
            var vector = new PTZVector { Zoom = new Vector1D { x = delta } };
            this.GetOnvifCamera().MoveAsync(MoveType.Relative, vector, null).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public void Handle(Move move)
        {
            (float panX, float panY) = move.MoveDirection.Accept(new PanTiltDeltaVisitor());
            var vector = new PTZVector { PanTilt = new Vector2D { x = panX, y = panY } };
            this.GetOnvifCamera().MoveAsync(MoveType.Relative, vector, null).GetAwaiter().GetResult();
        }

        private OnvifCamera GetOnvifCamera()
        {
            var info = this._Camera.VideoInformation;
            var account = new Account(info.ONVIFHost!, info.ONVIFUsername!, info.ONVIFPassword!);
            return OnvifCamera.CreateAsync(account, null).GetAwaiter().GetResult()
                ?? throw new InvalidOperationException($"Could not connect to ONVIF camera {this._Camera}");
        }

        private sealed class PanTiltDeltaVisitor : IMoveDirectionVisitor<(float x, float y)>
        {
            public (float x, float y) Handle(MoveUp _) => (0f, StepSize);
            public (float x, float y) Handle(MoveDown _) => (0f, -StepSize);
            public (float x, float y) Handle(MoveLeft _) => (-StepSize, 0f);
            public (float x, float y) Handle(MoveRight _) => (StepSize, 0f);
        }

        private sealed class ZoomDeltaVisitor : IZoomDirectionVisitor<float>
        {
            public float Handle(ZoomIn _) => StepSize;
            public float Handle(ZoomOut _) => -StepSize;
        }
    }
}
