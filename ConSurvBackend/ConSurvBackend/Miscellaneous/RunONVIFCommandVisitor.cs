using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.ONVIFVideo;
using ConSurvBackend.Core.Model.CameraProperties.VideoTypes.ONVIFVideo.Commands;
using System;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ConSurvBackend.Core.Miscellaneous
{
    internal class RunONVIFCommandVisitor : IONVIFCommandVisitor
    {
        private readonly Camera _Camera;

        public RunONVIFCommandVisitor(Camera camera)
        {
            GUtilities.AssertCondition(camera.VideoType is ONVIF, "ONVIF-commands can only be executed on camers whose camera-type is ONVIF.");
            this._Camera = camera;
        }

        public void Handle(Zoom zoom) => throw new NotImplementedException();

        public void Handle(Move move) => throw new NotImplementedException();
    }
}
