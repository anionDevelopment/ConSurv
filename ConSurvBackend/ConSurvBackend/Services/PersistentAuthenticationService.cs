using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ConSurvBackend.Core.Services
{
    internal class PersistentAuthenticationService : IAuthenticationService<User>
    {
        private readonly IAuthenticationServicePersistence<User> _Persistence;

        public PersistentAuthenticationService(IAuthenticationServicePersistence<User> _Persistence)
        {
            this._Persistence = _Persistence;
        }

        public bool AccessTokenIsValid(string accessToken)
        {
            throw new NotImplementedException();
        }

        public void AddRole(string roleName)
        {
            this._Persistence.AddRole(new Role()
            {
                Id = Guid.NewGuid().ToString(),
                Name = roleName,
            });
        }

        public void AddUser(User user)
        {
            this._Persistence.AddUser(user);
        }

        public void AddUserTyped(User user)
        {
            this.AddUser(user);
        }

        public void EnsureRoleDoesNotExist(string roleName)
        {
            if (!this.RoleExists(roleName))
            {
                throw new NotImplementedException();
            }
        }

        public void EnsureRoleExists(string roleName)
        {
            if (!this.RoleExists(roleName))
            {
                this.AddRole(roleName);
            }
        }

        public void EnsureUserDoesNotHaveRole(string userId, string roleId)
        {
            if (this.UserHasRole(userId, roleId))
            {
                throw new NotImplementedException();
            }
        }

        public void EnsureUserHasRole(string userId, string roleId)
        {
            if (!this.UserHasRole(userId, roleId))
            {
                this._Persistence.AddRoleToUser(userId, roleId);
            }
        }

        public ISet<User> GetAllUser()
        {
            return this._Persistence.GetAllUsers().Values.ToHashSet();
        }

        public ISet<User> GetAllUserTyped()
        {
            return this.GetAllUser();
        }

        public Role GetRoleByName(string roleName)
        {
            return this._Persistence.GetRoleByName(roleName);
        }

        public ISet<string> GetRolesOfUser(string userId)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string userId)
        {
            return this.GetUserById(userId);
        }

        public User GetUserByAccessToken(string accessToken)
        {
            return this._Persistence.GetUserByAccessToken(accessToken);
        }

        public User GetUserById(string userId)
        {
            return this._Persistence.GetUserById(userId);
        }

        public User GetUserByName(string userName)
        {
            return this._Persistence.GetUserByName(userName);
        }

        public User GetUserByNameTyped(string name)
        {
            return this._Persistence.GetUserByName(name);
        }

        public string GetUserName(string accessToken)
        {
            return this._Persistence.GetUserByAccessToken(accessToken).Name;
        }

        public User GetUserTyped(string userId)
        {
            return this.GetUser(userId);
        }

        public string Hash(string password)
        {
            string result = GUtilities.ByteArrayToHexString(new SHA256().Hash(GUtilities.StringToByteArray(password)));
            return result;
        }

        public AccessToken Login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public void Logout(string accessToken)
        {
            throw new NotImplementedException();
        }

        public void Logout(ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public void LogoutEverywhere(string userId)
        {
            throw new NotImplementedException();
        }

        public void RemoveUser(string userId)
        {
            throw new NotImplementedException();
        }

        public bool RoleExists(string roleName)
        {
            return this._Persistence.RoleExists(roleName);
        }

        public void UpdateRole(Role role)
        {
            this._Persistence.UpdateRole(role);
        }

        public void UpdateUser(User user)
        {
            this._Persistence.UpdateUser(user);
        }

        public bool UserExists(string userId)
        {
            return this._Persistence.UserWithIdExists(userId);
        }

        public bool UserExistsByName(string username)
        {
            return this._Persistence.UserWithNameExists(username);
        }

        public bool UserHasRole(string userId, string roleId)
        {
            return this._Persistence.UserHasRole(userId, roleId);
        }

        public bool UserWithNameExists(string username)
        {
            return this._Persistence.UserWithNameExists(username);
        }
    }
}
