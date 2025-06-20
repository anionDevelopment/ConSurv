using ConSurvBackend.Core.Model.Base;

namespace ConSurvBackend.Core.Services
{
    public interface IStreamOrganizerService
    {
        /// <summary>
        /// This function ensures that it is possible or not possible to access the camera by <see cref="IRTSPManager"/>.
        /// If it will be possible or not depends on <see cref="Camera.Enabled"/>.
        /// </summary>
        public void OrganizeCamera(Camera camera);
        public bool IsAvailable(string cameraId);
        public string GetStreamURL(string  cameraId);
    }
}
