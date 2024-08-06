using ContinuousSurveillanceBackend.Core.Model;
using ContinuousSurveillanceBackend.Core.Model.CameraProperties.ONVIF;
using ContinuousSurveillanceBackend.Core.Model.CameraProperties.VideoTypes;
using System;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ContinuousSurveillanceBackend.Core.Miscellaneous
{
    internal class RunONVIFCommandVisitor : IONVIFCommandVisitor
    {
        private readonly Camera _Camera;

        public RunONVIFCommandVisitor(Camera camera)
        {
            GUtilities.AssertCondition(camera.VideoType is ONVIF, "ONVIF-commands can only be executed on camers whose camera-type is ONVIF.");
            this._Camera = camera;
        }

        public void Handle(Zoom zoom)
        {
            throw new NotImplementedException();
        }

        public void Handle(Move move)
        {
            throw new NotImplementedException();
        }
    }
}
