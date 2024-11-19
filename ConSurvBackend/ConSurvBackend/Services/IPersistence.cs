using ConSurvBackend.Core.Model;
using GRYLibrary.Core.APIServer.Services;

namespace ConSurvBackend.Core.Services
{
    public interface IPersistence : IExternalService
    {
        void CreateCamera(Camera camera);
        void UpdateCamera(Camera camera);
        void RemoveCamera(string cameraId);
        bool UserWithNameExists(string username);
    }
}
