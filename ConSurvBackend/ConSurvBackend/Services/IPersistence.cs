using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services;
using GRYLibrary.Core.APIServer.Services.Trans;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Services
{
    public interface IPersistence : IExternalService, IAuthenticationServicePersistence<User>
    {
        public void Reset();
        public void CreateCamera(Camera camera);
        public void UpdateCamera(Camera camera);
        public void RemoveCamera(string cameraId);
        /// <remarks>
        /// dictionary-key: camera-id
        /// dictionary-valur: camera-object
        /// </remarks>
        public IDictionary<string, Camera> GetAllCameras();
        public bool IsCamera(string id);

    }
}
