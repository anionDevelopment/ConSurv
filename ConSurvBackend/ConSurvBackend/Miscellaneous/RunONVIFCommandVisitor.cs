using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using System;

namespace ConSurvBackend.Core.Miscellaneous
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
