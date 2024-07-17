using ContinuousSurveillanceBackend.Core.Configuration;
using ContinuousSurveillanceBackend.Core.Model;
using ContinuousSurveillanceBackend.Core.Model.RecordingModes;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuousSurveillanceBackend.Core.Miscellaneous
{
    internal class CameraManagementLoopVisitor : IRecordModeVisitor
    {
        private readonly IGeneralLogger _Logger;
        private readonly Camera _Camera;
        private readonly CodeUnitSpecificConfiguration _CodeUnitSpecificConfiguration;
        public CameraManagementLoopVisitor(IGeneralLogger logger,Camera camera, CodeUnitSpecificConfiguration codeUnitSpecificConfiguration)
        {
            this._Logger = logger;
            this._Camera = camera;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
        }

        public void Handle(NoRecording noRecording)
        {
            this._Camera.GetCurrentRecordingInformation().Accept(new CameraManagementLoopNoRecordingVisitor(this._Logger, this._Camera));
        }

        public void Handle(RecordAlways recordAlways)
        {
            this._Camera.GetCurrentRecordingInformation().Accept(new CameraManagementLoopRecordAlwaysVisitor(this._Logger, this._Camera, this._CodeUnitSpecificConfiguration));
        }
    }
}
