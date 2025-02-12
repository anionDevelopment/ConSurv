using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Model.RecordStates;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Services
{
    public interface ICameraService
    {
        /// <returns>Returns the id of the created camera</returns>
        public string CreateCamera(string name, string streamURL);
        public bool IsAvailable(Camera camera);
        public RecordState GetCurrentRecordingInformation(Camera camera);
        public byte[] GetPreview(Camera camera, uint? maximalHeight, uint? maximalWidth);
        public void UpdateCamera(Camera camera);
        public void RemoveCamera(string cameraId);
        public void RunONVIFCommand(string cameraId, ONVIFCommand onvifCommand);
        public Camera GetCameraById(string cameraId);
        public double GetRateOfAvailableCameras();
        public string Register(string username, string password);
        public bool UserWithNameExists(string username);
        public IDictionary<string, Camera> GetAllCameras();
        public CameraDTO ToDTO(Camera camera);
    }
}
