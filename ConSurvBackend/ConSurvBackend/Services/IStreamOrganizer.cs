using System;

namespace ConSurvBackend.Core.Services
{
    [Obsolete]
    public interface IStreamOrganizerService
    {
        public bool IsAvailable(string cameraId);
        public string GetStreamURL(string  cameraId);
    }
}
