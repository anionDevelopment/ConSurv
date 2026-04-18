using GRYLibrary.Core.APIServer.BaseServices;
using System;

namespace ConSurvBackend.Core.BackgroundServices
{
    /// <summary>
    /// Marker interface for the motion detection background service.
    /// </summary>
    [Obsolete]
    public interface IMotionDetectionService : IIteratingBackgroundService
    {
    }
}
