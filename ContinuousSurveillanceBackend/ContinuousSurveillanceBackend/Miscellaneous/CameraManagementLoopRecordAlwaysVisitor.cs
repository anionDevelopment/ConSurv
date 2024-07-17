using ContinuousSurveillanceBackend.Core.Configuration;
using ContinuousSurveillanceBackend.Core.Model;
using ContinuousSurveillanceBackend.Core.Model.RecordingModes;
using ContinuousSurveillanceBackend.Core.Model.RecordingStates;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Miscellaneous;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace ContinuousSurveillanceBackend.Core.Miscellaneous
{
    public class CameraManagementLoopRecordAlwaysVisitor : IRecordingStateVisitor
    {
        private readonly IGeneralLogger _Logger;
        private readonly Camera _Camera;
        private readonly CodeUnitSpecificConfiguration _CodeUnitSpecificConfiguration;
        public CameraManagementLoopRecordAlwaysVisitor(IGeneralLogger logger, Camera camera, CodeUnitSpecificConfiguration codeUnitSpecificConfiguration)
        {
            this._Logger = logger;
            this._Camera = camera;
            this._CodeUnitSpecificConfiguration = codeUnitSpecificConfiguration;
        }

        public void Handle(CurrentlyRecording currentlyRecording)
        {
            GUtilities.NoOperation();
        }

        public void Handle(Idle idle)
        {
            throw new NotImplementedException();//TODO start camera using folder and length from _CodeUnitSpecificConfiguration
        }

        public void Handle(Unavailable unavailable)
        {
            this._Logger.Log($"Can not start record on camera {this._Camera.Id} because it is not available.", LogLevel.Warning);
        }
    }
}
