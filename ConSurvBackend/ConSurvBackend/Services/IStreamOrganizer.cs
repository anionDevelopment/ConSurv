namespace ConSurvBackend.Core.Services
{
    public interface IStreamOrganizerService
    {
        public bool IsAvailable(string cameraId);
        public string GetStreamURL(string  cameraId);
    }
}
