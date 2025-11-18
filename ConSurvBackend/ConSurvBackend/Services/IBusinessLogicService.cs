using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Model.RecordStates;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using System;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Services
{
    public interface IBusinessLogicService
    {
        /// <returns>Returns the id of the created camera</returns>
        public string CreateCamera(string name, string streamURL);
        public (bool,Exception?) IsAvailable(Camera camera);
        public RecordState GetCurrentRecordingInformation(Camera camera);
        public void UpdateCamera(Camera camera);
        public void RemoveCamera(string cameraId);
        public void RunONVIFCommand(string cameraId, ONVIFCommand onvifCommand);
        public Camera GetCameraById(string cameraId);
        public double GetRateOfAvailableCameras();
        public string Register(string username, string password);
        public bool UserWithNameExists(string username);
        public IDictionary<string, Camera> GetAllCameras();
        public CameraDTO ToDTO(Camera camera);
        public void EnsureUserHasRole(string userId, string roleId);
        public void EnsureUserDoesNotHaveRole(string userId, string roleId);
        public AccessToken Login(string name, string v);
        public User GetUser(string userId);
        /// <summary>
        /// key: camera-id
        /// value: filenames of videos
        /// </summary>
        public IDictionary<string, IList<string>> GetVideos();
        public void RemoveVideo(string cameraId,string filename);
        byte[] GetPreviewOfVideo(string cameraId, string filename);
        byte[] GetVideo(string cameraId, string filename);
    }
}
