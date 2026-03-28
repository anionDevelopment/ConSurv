using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Utilities.InitializationStates;
using GRYLibrary.Core.Logging.GRYLogger;
using System;

namespace ConSurvBackend.Core.BackgroundServices
{
    public class MotionDetectionService : IteratingBackgroundService, IMotionDetectionService
    {
        private readonly IBusinessLogicService _CameraService;
        private readonly IRuntimeData _RuntimeData;
        private readonly IInitializationService<CommandlineParameter> _InitializationService;
        private readonly CommandlineParameter _CommandlineParameter;
        public MotionDetectionService(IApplicationConstants constants, IGRYLog logger, IBusinessLogicService cameraService, IRuntimeData runtimeData, IInitializationService<CommandlineParameter> initializationService, CommandlineParameter commandlineParameter) : base(constants.ExecutionMode, logger)
        {
            this.Enabled = true;
            this.AdditionalDelay = TimeSpan.FromSeconds(2);
            this._CameraService = cameraService;
            this._RuntimeData = runtimeData;
            this._InitializationService = initializationService;
            this._CommandlineParameter = commandlineParameter;
        }

        protected override void Run()
        {
            if (this._CommandlineParameter.RealRun && this._InitializationService.GetInitializationState() is Initialized)
            {
                foreach (System.Collections.Generic.KeyValuePair<string, Model.Base.Camera> camera in this._CameraService.GetAllCameras())
                {
                    //TODO for example retrieve last previews and analyze it and raise event if there is a change.
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //add dispose logic here if required
            }
            base.Dispose(disposing);
        }
    }
}
