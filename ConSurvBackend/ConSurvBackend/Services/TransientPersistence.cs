using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Trans;
using System;
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
            this._Cameras[camera.Id] = camera;
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

        public (bool, Exception?) IsAvailable()
        {
            return (true, null);
        }

        public bool UserWithNameExists(string username)
        {
            return this._TransientAuthenticationServicePersistence.UserWithNameExists(username);
        }

        public IDictionary<string, Camera> GetAllCameras()
        {
            return this._Cameras;
        }

        public IDictionary<string, User> GetAllUsers()
        {
            return this._TransientAuthenticationServicePersistence.GetAllUsers();
        }

        public ISet<Role> GetAllRoles()
        {
            return this._TransientAuthenticationServicePersistence.GetAllRoles();
        }

        public void AddRole(Role role)
        {
            this._TransientAuthenticationServicePersistence.AddRole(role);
        }

        public void UpdateRole(Role role)
        {
            this._TransientAuthenticationServicePersistence.UpdateRole(role);
        }

        public void DeleteRoleByName(string roleName)
        {
            this._TransientAuthenticationServicePersistence.DeleteRoleByName(roleName);
        }

        public bool AccessTokenExists(string accessToken, out User? user)
        {
            return this._TransientAuthenticationServicePersistence.AccessTokenExists(accessToken, out user);
        }

        public void AddUser(User newUser)
        {
            this._TransientAuthenticationServicePersistence.AddUser(newUser);
        }

        public bool UserWithIdExists(string userId)
        {
            return this._TransientAuthenticationServicePersistence.UserWithIdExists(userId);
        }

        public User GetUserById(string userId)
        {
            return this._TransientAuthenticationServicePersistence.GetUserById(userId);
        }

        public User GetUserByName(string userName)
        {
            return this._TransientAuthenticationServicePersistence.GetUserByName(userName);
        }

        public void RemoveUser(string userId)
        {
            this._TransientAuthenticationServicePersistence.RemoveUser(userId);
        }

        public bool RoleExists(string roleName)
        {
            return this._TransientAuthenticationServicePersistence.RoleExists(roleName);
        }

        public void AddRoleToUser(string userId, string roleId)
        {
            this._TransientAuthenticationServicePersistence.AddRoleToUser(userId, roleId);
        }

        public void RemoveRoleFromUser(string userId, string roleId)
        {
            this._TransientAuthenticationServicePersistence.RemoveRoleFromUser(userId, roleId);
        }

        public bool UserHasRole(string userId, string roleId)
        {
            return this._TransientAuthenticationServicePersistence.UserHasRole(userId, roleId);
        }

        public User GetUserByAccessToken(string accessToken)
        {
            return this._TransientAuthenticationServicePersistence.GetUserByAccessToken(accessToken);
        }

        public void UpdateUser(User user)
        {
            this._TransientAuthenticationServicePersistence.UpdateUser(user);
        }

        public AccessToken GetAccessToken(string accessToken)
        {
            return this._TransientAuthenticationServicePersistence.GetAccessToken(accessToken);
        }

        public void AddAccessToken(AccessToken newAccessToken)
        {
            this._TransientAuthenticationServicePersistence.AddAccessToken(newAccessToken);
        }

        public void RemoveAccessToken(string accessToken)
        {
            this._TransientAuthenticationServicePersistence.RemoveAccessToken(accessToken);
        }

        public ISet<AccessToken> GetAllAccessTokenOfUser(string userId)
        {
            return this._TransientAuthenticationServicePersistence.GetAllAccessTokenOfUser(userId);
        }

        public Role GetRoleByName(string roleName)
        {
            return this._TransientAuthenticationServicePersistence.GetRoleByName(roleName);
        }

        public bool IsCamera(string id)
        {
            return this._Cameras.ContainsKey(id);
        }

        public Role GetRoleById(string roleId)
        {
            throw new NotImplementedException();
        }
    }
}
