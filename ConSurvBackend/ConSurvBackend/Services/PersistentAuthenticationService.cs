using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.Crypto;
using GRYLibrary.Core.Exceptions;
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
        private readonly ITimeService _TimeService;
        private readonly IServerLog _Log;

        public PersistentAuthenticationService(ITimeService timeService, IAuthenticationServicePersistence<User> persistence, IServerLog log)
        {
            this._Persistence = persistence;
            this._TimeService = timeService;
            this._Log = log;
        }

        public bool AccessTokenIsValid(string accessToken)
        {
            AccessToken token = this._Persistence.GetAccessToken(accessToken);
            DateTimeOffset now = this._TimeService.GetCurrentTimeInUTCAsDateTimeOffset();
            this._Log.Logger.Log($"Checked if access token {accessToken} is valid. Expired moment: {GRYLibrary.Core.Misc.Utilities.FormatTimestamp(token.ExpiredMoment, false)}; now: {GRYLibrary.Core.Misc.Utilities.FormatTimestamp(now, false)}");
            return now < token.ExpiredMoment;
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
            if (!this._Persistence.UserWithNameExists(userName))
            {
                throw new InvalidCredentialsException();
            }
            this._Persistence.GetUserByName(userName);
            User user = this.GetUserByNameTyped(userName);
            if (this.Hash(password) != user.PasswordHash)
            {
                throw new InvalidCredentialsException();
            }
            if (user.UserIsLocked)
            {
                throw new NotAuthorizedException($"User '{userName}' is locked.");
            }
            AccessToken newAccessToken = new AccessToken();
            newAccessToken.Value = Guid.NewGuid().ToString();
            newAccessToken.ExpiredMoment = this._TimeService.GetCurrentTimeInUTCAsDateTimeOffset().AddDays(1);//TODO make this configurable
            newAccessToken.OwnerUserId = user.Id;
            this._Persistence.AddAccessToken(newAccessToken);
            user.AccessToken.Add(newAccessToken);
            return newAccessToken;
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

        public string GetBaseRoleOfAllUser()
        {
            throw new NotImplementedException();
        }

        public ClaimsPrincipal GetPrincipal(string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}
