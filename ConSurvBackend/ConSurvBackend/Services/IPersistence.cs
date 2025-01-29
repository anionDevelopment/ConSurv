using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.APIServer.Services;
using GRYLibrary.Core.APIServer.Services.Trans;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Services
{
    public interface IPersistence : IExternalService, IAuthenticationServicePersistence<Model.User>
    {
        void CreateCamera(Camera camera);
        void UpdateCamera(Camera camera);
        void RemoveCamera(string cameraId);
        bool UserWithNameExists(string username);
        IDictionary<string,Camera> GetAllCameras();
    }
}
