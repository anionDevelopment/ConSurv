using ContinuousSurveillanceBackend.Core.Services;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace ContinuousSurveillanceBackend.Core.Miscellaneous
{
    public class HealthCheck : IHealthCheck
    {
        private readonly IGeneralLogger _Logger;
        private readonly IPersistence _Persistence;
        public HealthCheck(IGeneralLogger logger, IPersistence persistence)
        {
            this._Logger = logger;
            this._Persistence = persistence;
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Tools.CheckHealthAsync(this._Logger, () =>
            {
                IList<string> messages = new List<string>();
                HealthStatus result=HealthStatus.Healthy;
                if (GUtilities.CheckCancellationToken(messages, cancellationToken, out (HealthStatus, IList<string>) abortResult))
                { return abortResult; }
                Tools.CheckService(this._Logger, nameof(this._Persistence), this._Persistence, ref result, messages, true,true);
                return (result, messages);
            }, context, cancellationToken);
        }
    }
}
