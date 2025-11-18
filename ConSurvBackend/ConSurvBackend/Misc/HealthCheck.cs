using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.Services.Init;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ConSurvBackend.Core.Misc
{
    public class HealthCheck : IHealthCheck
    {
        private readonly IGeneralLogger _Logger;
        private readonly IPersistence _Persistence;
        private readonly IBusinessLogicService _CameraService;
        private readonly IInitializationService _InitializationService;
        public HealthCheck(IGeneralLogger logger, IPersistence persistence, IBusinessLogicService cameraSchedulerService, IInitializationService initializationService)
        {
            this._Logger = logger;
            this._Persistence = persistence;
            this._CameraService = cameraSchedulerService;
            this._InitializationService = initializationService;
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Tools.CheckHealthAsync(this._Logger, () =>
            {
                IList<string> messages = new List<string>();
                HealthStatus result = HealthStatus.Healthy;
                Tools.CheckSingleExternalService(this._Logger, this._Persistence.GetType().Name, this._Persistence, ref result, messages, true, true);
                (HealthStatus, IList<string>) abortResult;
                if (GUtilities.CheckCancellationToken(messages, cancellationToken, out abortResult))
                {
                    return abortResult;
                }
                foreach (Camera camera in this._CameraService.GetAllCameras().Values)
                {
                    Tools.CheckSingleExternalService(this._Logger, $"Camera_{camera.Id}", () => this._CameraService.IsAvailable(camera), ref result, messages, true, false);
                    if (GUtilities.CheckCancellationToken(messages, cancellationToken, out abortResult))
                    {
                        return abortResult;
                    }
                }

                return (result, messages);

            }, context, cancellationToken, this._InitializationService);
        }
    }
}
