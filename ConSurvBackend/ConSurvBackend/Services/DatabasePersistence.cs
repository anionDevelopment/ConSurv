using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.RecordModes;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using Role = GRYLibrary.Core.APIServer.CommonDBTypes.Role;

namespace ConSurvBackend.Core.Services
{
    public sealed class DatabasePersistence : IPersistence
    {
        private readonly ISQLProvider _SQLProvider;
        private static readonly object _Lock = new object();
        private readonly Semaphore _Semaphore = new Semaphore();
        private readonly ITimeService _TimeService;
        private readonly IGRYLog _Log;
        private readonly IConSurvDatabaseInteractor _Database;
        public DatabasePersistence(IConSurvDatabaseInteractor database, ITimeService timeService, IGRYLog log)
        {
            this._TimeService = timeService;
            this._Database = database;
            this._Log = log;
            this._SQLProvider = database.GetSQLProvider();
        }

        #region AccessDatabase
        protected void AccessDatabase(Action<IConSurvDatabaseInteractor> action)
        {
            lock (_Lock)
            {
                DBUtilities.AccessDatabase<IConSurvDatabaseInteractor>(this._Database, action);
            }
        }

        protected T AccessDatabase<T>(Func<IConSurvDatabaseInteractor, T> function)
        {
            lock (_Lock)
            {
                return DBUtilities.AccessDatabase<T, IConSurvDatabaseInteractor>(this._Database, function);
            }
        }
        protected void RunTransaction(string nameOfAction, params Action<DbCommand>[] actions)
        {
            lock (_Lock)
            {
                DBUtilities.RunTransaction<IConSurvDatabaseInteractor>(nameOfAction, this._Log, this._Database, actions);
            }
        }

        protected T?[] RunTransaction<T>(string nameOfAction, params Func<DbCommand, T?>[] functions)
        {
            lock (_Lock)
            {
                return DBUtilities.RunTransaction<T, IConSurvDatabaseInteractor>(nameOfAction, this._Log, this._Database, functions);
            }
        }

        #endregion

        public void CreateCamera(Camera camera)
        {
            this.RunTransaction(nameof(CreateCamera),(command) =>
            {
                command.CommandText = this._SQLProvider.GetScriptCreateCamera();
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Id", camera.Id));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Name", camera.Name));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("StreamURL", camera.VideoInformation.StreamURL,typeof(string)));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("IsONVIFCamera", camera.VideoInformation.IsONVIFCamera));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Certificate", camera.VideoInformation.Certificate, typeof(string)));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("RecordMode", RecordMode.ToNumber(camera.RecordMode.GetType())));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Enabled", camera.Enabled));
                command.ExecuteNonQuery();
            });
        }

        public void RemoveCamera(string cameraId)
        {
            throw new NotImplementedException();
        }

        public void UpdateCamera(Camera camera)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, Camera> GetAllCameras()
        {
            IDictionary<string, Camera> roles = this.RunTransaction(nameof(GetAllCameras),(command) =>
            {
                IDictionary<string, Camera> cameraDictionary = new Dictionary<string, Camera>();
                command.CommandText = this._SQLProvider.GetScriptGetAllCameras();
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        string name = reader.GetString(1);
                        Camera camera = new Camera(id, name);
                        camera.VideoInformation = new VideoInformation()
                        {
                            StreamURL = reader.GetString(2),
                            IsONVIFCamera = reader.GetBoolean(3),
                            Certificate = DBUtilities.GetNullableValue<string>(reader, 4),
                        };
                        camera.RecordMode = RecordMode.FromNumberToInstance(reader.GetByte(5));
                        camera.Enabled = reader.GetBoolean(6);
                        cameraDictionary[id] = camera;
                    }
                    reader.Close();
                    return cameraDictionary;
                }
                ;
            })[0];
            return roles;
        }

        public (bool, Exception?) IsAvailable()
        {
            bool result = this._Database.GetGenericDatabaseInteractor().TryGetConnection(out _, out Exception? e);
            return (result, e);
        }

        public void Dispose()
        {
           //TODO
        }

        public bool UserWithNameExists(string userName)
        {
            return this.RunTransaction(nameof(UserWithNameExists),(cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptUserWithNameExists();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("UserName", userName));
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            })[0];
        }

        public IDictionary<string, User> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public ISet<Role> GetAllRoles()
        {
            ISet<Role> roles = this.RunTransaction(nameof(GetAllRoles),(command) =>
            {
                ISet<Role> rolesInternal = new HashSet<Role>();
                command.CommandText = this._SQLProvider.GetScriptGetAllRoles();
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        string name = reader.GetString(1);
                        rolesInternal.Add(new Role() { Id = id, Name = name });
                    }
                    reader.Close();
                    return rolesInternal;
                }
                ;
            })[0];
            foreach (Role role in roles)
            {
                this.EnrichWithInheritedRoles(role);
            }
            return roles;
        }

        private void EnrichWithInheritedRoles(Role role)
        {
            //TODO load inherited roles transitively
        }

        public void AddRole(Role role)
        {
            this.RunTransaction(nameof(AddRole),(command) =>
            {
                command.CommandText = this._SQLProvider.GetScriptInsertRole();
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Id", role.Id));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Name", role.Name));
                command.ExecuteNonQuery();
            }, (command) =>
            {
                //TODO add inherited roles
            });
        }

        public void UpdateRole(Role role)
        {
            this.RunTransaction(nameof(UpdateRole),(cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptUpdateRole();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Id", role.Id));
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Name", role.Name));
                using DbDataReader reader = cmd.ExecuteReader();
            }, (cmd) =>
            {
                //TODO update inherited roles
            });
        }

        public void DeleteRoleByName(string roleName)
        {
            throw new NotImplementedException();
        }

        public bool AccessTokenExists(string accessToken, out User user)
        {
            throw new NotImplementedException();
        }

        public void AddUser(User user)
        {
            this.RunTransaction(nameof(AddUser),(command) =>
            {
                command.CommandText = this._SQLProvider.GetScriptAddUser();
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Id", user.Id));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Name", user.Name));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("PasswordHash", user.PasswordHash));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("EMailAddress", user.EMailAddress));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("UserIsActivated", user.UserIsActivated));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("UserIsLocked", user.UserIsLocked));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("RegistrationMoment", user.RegistrationMoment));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("TOTPActivated", user.TOTP.IsActicated));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("TOTPSecretKey", user.TOTP.SecretKey));
                command.ExecuteNonQuery();
            });
        }

        public bool UserWithIdExists(string userId)
        {
            return this.RunTransaction(nameof(UserWithIdExists),(cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptUserWithIdExists();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter(nameof(userId), userId));
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            })[0];
        }

        public User GetUserById(string userId)
        {
            User result = this.RunTransaction(nameof(GetUserById),(cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetUserById();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Id", userId));
                using DbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    User user = new User();
                    user.Id = userId;
                    user.Name = reader.GetString(1);
                    user.PasswordHash = reader.GetString(2);
                    user.EMailAddress = DBUtilities.GetNullableValue<string>(reader, 3);
                    user.UserIsActivated = reader.GetBoolean(4);
                    user.UserIsLocked = reader.GetBoolean(5);
                    user.RegistrationMoment = reader.GetDateTime(6);
                    return user;
                }
                else
                {
                    throw new KeyNotFoundException($"No user found with id '{userId}'");
                }
            })[0];
            this.EnrichWhichAccessToken(result);
            this.EnrichWhichTOTPToken(result);
            return result;
        }
        private void EnrichWhichTOTPToken(User result)
        {
            //TODO
        }

        private void EnrichWhichAccessToken(User user)
        {
            this.RunTransaction(nameof(EnrichWhichAccessToken),(cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetAllAccessTokenForUser();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("UserId", user.Id));
                using DbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        user.AccessToken.Add(new AccessToken()
                        {
                            Value = reader.GetString(0),
                            ExpiredMoment = reader.GetDateTime(1),
                            OwnerUserId = user.Id
                        });
                    }
                }
            });
        }

        public User GetUserByName(string userName)
        {
            User result = this.RunTransaction(nameof(GetUserByName),(cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetUserByName();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Name", userName));
                using DbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    User user = new User();
                    user.Id = reader.GetString(0);
                    user.Name = reader.GetString(1);
                    user.PasswordHash = reader.GetString(2);
                    user.EMailAddress = DBUtilities.GetNullableValue<string>(reader, 3);
                    user.UserIsActivated = reader.GetBoolean(4);
                    user.UserIsLocked = reader.GetBoolean(5);
                    user.RegistrationMoment = reader.GetDateTime(6);
                    return user;
                }
                else
                {
                    throw new KeyNotFoundException($"No user found with username '{userName}'");
                }
            })[0];
            this.EnrichWhichAccessToken(result);
            this.EnrichWhichTOTPToken(result);
            return result;
        }

        public void RemoveUser(string userId)
        {
            throw new NotImplementedException();
        }

        public bool RoleExists(string roleName)
        {
            return this.RunTransaction(nameof(RoleExists),(cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptRoleExists();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("RoleName", roleName));
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            })[0];
        }

        public void AddRoleToUser(string userId, string roleId)
        {
            this.RunTransaction(nameof(AddRoleToUser),(command) =>
            {
                this._Log.Log($"Adding role '{roleId}' to user '{userId}'", LogLevel.Information);
                command.CommandText = this._SQLProvider.GetScriptAddRoleToUser();
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("UserId", userId));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("RoleId", roleId));
                command.ExecuteNonQuery();
            });
        }

        public void RemoveRoleFromUser(string userId, string roleId)
        {
            throw new NotImplementedException();
        }

        public bool UserHasRole(string userId, string roleId)
        {
            return this.RunTransaction(nameof(UserHasRole),(cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptUserHasRole();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("UserId", userId));
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("RoleId", roleId));
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            })[0];
        }

        public User GetUserByAccessToken(string accessToken)
        {
            return this.GetUserById(this.GetAccessToken(accessToken).OwnerUserId);
        }

        public void UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public AccessToken GetAccessToken(string accessToken)
        {
            throw new NotImplementedException();
        }

        public void AddAccessToken(string userId, AccessToken newAccessToken)
        {
            throw new NotImplementedException();
        }

        public void RemoveAccessToken(string accessToken)
        {
            throw new NotImplementedException();
        }

        public ISet<AccessToken> GetAllAccessTokenOfUser(string userId)
        {
            throw new NotImplementedException();
        }

        public Role GetRoleByName(string roleName)
        {
            Role result = this.RunTransaction(nameof(GetRoleByName),(cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetRoleByName();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Name", roleName));
                using DbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    Role role = new Role();
                    role.Id = roleName;
                    role.Name = reader.GetString(0);
                    this.EnrichWithInheritedRoles(role);
                    return role;
                }
                else
                {
                    throw new KeyNotFoundException($"No user found with username '{roleName}'");
                }
            })[0];
            return result;
        }

        public void Reset()
        {
            this.RunTransaction(nameof(Reset),(cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptResetDatabase();
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            });
        }

        public bool IsCamera(string id)
        {
            return this.RunTransaction(nameof(IsCamera),(cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptIsCamera();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Id", id));
                using DbDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    return reader.GetInt32(0) == 1;
                }
                else
                {
                    return false;
                }
            })[0];
        }


        public Role GetRoleById(string roleId)
        {
            throw new NotImplementedException();
        }
    }
}
