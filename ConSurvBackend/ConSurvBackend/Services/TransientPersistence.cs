using ConSurvBackend.Core.Model;
using ExtendedXmlSerializer;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Trans;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ConSurvBackend.Core.Services
{
    public sealed class TransientPersistence : IPersistence
    {
        private readonly IDictionary<string, Camera> _Cameras = new Dictionary<string, Camera>();
        private readonly IAuthenticationServicePersistence<User> _TransientAuthenticationServicePersistence;

        public TransientPersistence(IAuthenticationServicePersistence<User> transientAuthenticationServicePersistence)
        {
            this._TransientAuthenticationServicePersistence = transientAuthenticationServicePersistence;
            this._Cameras = new Dictionary<string, Camera>();
            this.Initialize();
        }

        private void Initialize()
        {
            this.Reset();
        }

        public void Reset()
        {
            this._Cameras.Clear();
        }

        public void CreateCamera(Camera camera)
        {
            this._Cameras[camera.Id]=camera;
        }

        public void RemoveCamera(string cameraId)
        {
            this._Cameras.Remove(cameraId);
        }

        public void UpdateCamera(Camera camera)
        {
          
            this._Cameras[camera.Id] = camera;
        }

        public void Dispose()
        {
            GUtilities.NoOperation();
        }

        public bool IsAvailable()
        {
            return true;
        }

        public bool UserWithNameExists(string username)
        {
         return this._TransientAuthenticationServicePersistence.UserWithNameExists(username);
        }
    }
}
