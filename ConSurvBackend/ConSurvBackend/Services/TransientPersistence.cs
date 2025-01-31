using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Trans;
using System.Collections.Generic;
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

        public IDictionary<string, Camera> GetAllCameras()
        {
            return this._Cameras;
        }

        public IDictionary<string, Model.User> GetAllUsers()
        {
            throw new System.NotImplementedException();
        }

        public ISet<Role> GetAllRoles()
        {
            throw new System.NotImplementedException();
        }

        public void AddRole(Role role)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateRole(Role role)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteRoleByName(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public bool AccessTokenExists(string accessToken, out Model.User? user)
        {
            throw new System.NotImplementedException();
        }

        public void AddUser(Model.User newUser)
        {
            throw new System.NotImplementedException();
        }

        public bool UserWithIdExists(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Model.User GetUserById(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Model.User GetUserByName(string userName)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveUser(string userId)
        {
            throw new System.NotImplementedException();
        }

        public bool RoleExists(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public void AddRoleToUser(string userId, string roleId)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveRoleFromUser(string userId, string roleId)
        {
            throw new System.NotImplementedException();
        }

        public bool UserHasRole(string userId, string roleId)
        {
            throw new System.NotImplementedException();
        }

        public Model.User GetUserByAccessToken(string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateUser(Model.User user)
        {
            throw new System.NotImplementedException();
        }

        public AccessToken GetAccessToken(string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public void AddAccessToken(string userId, AccessToken newAccessToken)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAccessToken(string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public ISet<AccessToken> GetAllAccessTokenOfUser(string userId)
        {
            throw new System.NotImplementedException();
        }
    }
}
