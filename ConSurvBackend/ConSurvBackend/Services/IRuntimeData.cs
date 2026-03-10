using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Model.Internals;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Services
{
    public interface IRuntimeData
    {
        public IDictionary<string, CameraInternalsBase> GetCameraInternals();
        public CameraInternalsBase GetCameraInternals(string cameraId);
        public void SetCameraInternals(CameraInternalsBase cameraInternals);
        public bool InternalsAreAvailable(string cameraId);
        public byte[] GetPreviewFallbackPicture();
        public void AddPreview(string cameraId, Preview preview);
        public Preview GetLatestPreview(string cameraId);
    }
}
