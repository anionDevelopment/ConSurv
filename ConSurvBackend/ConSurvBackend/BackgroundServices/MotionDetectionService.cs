using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Misc.Logger;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.APIServer.Utilities.InitializationStates;
using System;

namespace ConSurvBackend.Core.BackgroundServices
{
    public class MotionDetectionService : IteratingBackgroundService, IMotionDetectionService
    {
        private readonly IBusinessLogicService _CameraService;
        private readonly IRuntimeData _RuntimeData;
        private readonly IInitializationService<CommandlineParameter> _InitializationService;
        private readonly CommandlineParameter _CommandlineParameter;
        /// <summary>
        /// Initializes a new instance of <see cref="MotionDetectionService"/> with all required dependencies.
        /// </summary>
        /// <param name="constants">Application-wide constants used to retrieve the execution mode.</param>
        /// <param name="logger">Logger passed to the base class.</param>
        /// <param name="cameraService">Service used to enumerate all configured cameras.</param>
        /// <param name="runtimeData">Shared in-memory runtime state containing camera previews.</param>
        /// <param name="initializationService">Service that tracks the application initialization state.</param>
        /// <param name="commandlineParameter">Parsed command-line parameters; used to check whether a real run is active.</param>
        public MotionDetectionService(IApplicationConstants constants, IMotionDetectionServiceLog logger, IBusinessLogicService cameraService, IRuntimeData runtimeData, IInitializationService<CommandlineParameter> initializationService, CommandlineParameter commandlineParameter) : base(constants.ExecutionMode, logger.Logger)
        {
            this.Enabled = true;
            this.AdditionalDelay = TimeSpan.FromSeconds(2);
            this._CameraService = cameraService;
            this._RuntimeData = runtimeData;
            this._InitializationService = initializationService;
            this._CommandlineParameter = commandlineParameter;
        }

        /// <inheritdoc />
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
        /// <inheritdoc />
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
