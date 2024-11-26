using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Services
{
    public interface ICameraService
    {
        /// <returns>Returns the id of the created camera</returns>
        string CreateCamera(string name);
        void UpdateCamera(string cameraId, string name, RecordMode recordMode);
        void RemoveCamera(string cameraId);
        void RunONVIFCommand(string cameraId, ONVIFCommand onvifCommand);
        Camera GetCameraById(string cameraId);
        double GetRateOfAvailableCameras();
        string Register(string username, string password);
        bool UserWithNameExists(string username);
        IEnumerable<Camera> GetAllCameras();
    }
}
