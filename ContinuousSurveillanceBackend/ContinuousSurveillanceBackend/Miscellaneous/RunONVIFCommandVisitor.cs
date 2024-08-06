using ContinuousSurveillanceBackend.Core.Model;
using ContinuousSurveillanceBackend.Core.Model.CameraProperties.ONVIF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuousSurveillanceBackend.Core.Miscellaneous
{
    internal class RunONVIFCommandVisitor : IONVIFCommandVisitor
    {
        private readonly Camera _Camera;

        public RunONVIFCommandVisitor(Camera camera)
        {
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
