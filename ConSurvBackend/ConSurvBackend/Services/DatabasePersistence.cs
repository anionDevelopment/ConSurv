using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;
using GRYLibrary.Core.Misc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data;
using ConSurvBackend.Core.Database;
using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.Extensions.Logging;
using ConSurvBackend.Core.Model.RecordModes;

namespace ConSurvBackend.Core.Services
{
    public sealed class DatabasePersistence : IPersistence
    {
        private readonly DatabaseContext _DatabaseContext;
        private readonly ISQLProvider _SQLProvider;
        private static readonly object _Lock = new object();
        private readonly Semaphore _Semaphore = new Semaphore();
        private readonly IGRYLog _Log;

        public DatabasePersistence(DbContextOptions<DatabaseContext> options, IGeneralLogger logger, ITimeService timeService, IDatabaseManager databaseManager, ISQLProvider sQLProvider, IGRYLog log)
        {
            this._DatabaseContext = new DatabaseContext(options, logger, timeService, databaseManager);
            this._SQLProvider = sQLProvider;
            this._Log = log;
        }

        #region AccessDatabase
        private void AccessDatabase(Action<DatabaseContext> action)
        {
            this.AccessDatabase((database) =>
            {
                action(database);
                return new object();
            });
        }

        private T AccessDatabase<T>(Func<DatabaseContext, T> func)
        {
            lock (_Lock)
            {
                this._Semaphore.Lock();
                try
                {
                    return func(this._DatabaseContext);
                }
                finally
                {
                    this._Semaphore.Unlock();
                }
            }
        }
        public void RunTransaction(params Action<MySqlCommand>[] actions)
        {
            this.RunTransaction(actions.Select<Action<MySqlCommand>, Func<MySqlCommand, object>>(action => (command) =>
            {
                action(command);
                return null;
            }
            ).ToArray());
        }

        public T[] RunTransaction<T>(params Func<MySqlCommand, T>[] functions)
        {
            List<T> results = new List<T>();
            this.AccessDatabase(context =>
            {
                MySqlConnection connection = context.Connection;
                using MySqlTransaction transaction = connection.BeginTransaction();
                bool commit = true;
                try
                {
                    foreach (Func<MySqlCommand, T> function in functions)
                    {
                        using (MySqlCommand cmd = connection.CreateCommand())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandTimeout = 300;
                            cmd.Transaction = transaction;
                            try
                            {
                                T result = function(cmd);
                                results.Add(result);
                            }
                            catch
                            {
                                commit = false;
                                throw;
                            }
                        }
                        ;
                    }
                }
                finally
                {
                    if (commit)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
            });
            return results.ToArray();
        }
        #endregion

        public void CreateCamera(Camera camera)
        {
            this.RunTransaction((command) =>
            {
                command.CommandText = this._SQLProvider.GetScriptCreateCamera();
                command.Parameters.Add(new MySqlParameter("Id", camera.Id));
                command.Parameters.Add(new MySqlParameter("Name", camera.Name));
                command.Parameters.Add(new MySqlParameter("StreamURL", camera.VideoInformation.StreamURL));
                command.Parameters.Add(new MySqlParameter("IsONVIFCamera", camera.VideoInformation.IsONVIFCamera));
                command.Parameters.Add(new MySqlParameter("Certificate", camera.VideoInformation.Certificate));
                command.Parameters.Add(new MySqlParameter("RecordMode", RecordMode.ToNumber(camera.RecordMode.GetType())));
                command.Parameters.Add(new MySqlParameter("Enabled", camera.VideoInformation.Certificate));
                command.ExecuteNonQuery();
            });
        }

        public void RemoveCamera(string cameraId)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateCamera(Camera camera)
        {
            throw new System.NotImplementedException();
        }

        public IDictionary<string, Camera> GetAllCameras()
        {
            IDictionary<string, Camera> roles = this.RunTransaction((command) =>
            {
                IDictionary<string, Camera> cameraDictionary = new Dictionary<string, Camera>();
                command.CommandText = this._SQLProvider.GetScriptGetAllCameras();
                using (MySqlDataReader reader = command.ExecuteReader())
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
                            Certificate = reader.GetString(4),
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

        public bool IsAvailable()
        {
            return this.AccessDatabase((databaseContext) =>
            {
                try
                {
                    return databaseContext.Database.CanConnect();
                }
                catch
                {
                    return false;
                }
            });
        }

        public void Dispose()
        {
            this._DatabaseContext.Dispose();
        }

        public bool UserWithNameExists(string userName)
        {
            return this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptUserWithNameExists();
                cmd.Parameters.Add(new MySqlParameter("UserName", userName));
                using MySqlDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            })[0];
        }

        public IDictionary<string, User> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public ISet<Role> GetAllRoles()
        {
            ISet<Role> roles = this.RunTransaction((command) =>
            {
                ISet<Role> rolesInternal = new HashSet<Role>();
                command.CommandText = this._SQLProvider.GetScriptGetAllRoles();
                using (MySqlDataReader reader = command.ExecuteReader())
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
            this.RunTransaction((command) =>
            {
                command.CommandText = this._SQLProvider.GetScriptInsertRole();
                command.Parameters.Add(new MySqlParameter("Id", role.Id));
                command.Parameters.Add(new MySqlParameter("Name", role.Name));
                command.ExecuteNonQuery();
            }, (command) =>
            {
                //TODO add inherited roles
            });
        }

        public void UpdateRole(Role role)
        {
            this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptUpdateRole();
                cmd.Parameters.Add(new MySqlParameter("Id", role.Id));
                cmd.Parameters.Add(new MySqlParameter("Name", role.Name));
                using MySqlDataReader reader = cmd.ExecuteReader();
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
            this.RunTransaction((command) =>
            {
                command.CommandText = this._SQLProvider.GetScriptAddUser();
                command.Parameters.Add(new MySqlParameter("Id", user.Id));
                command.Parameters.Add(new MySqlParameter("Name", user.Name));
                command.Parameters.Add(new MySqlParameter("PasswordHash", user.PasswordHash));
                command.Parameters.Add(new MySqlParameter("EMailAddress", user.EMailAddress));
                command.Parameters.Add(new MySqlParameter("UserIsActivated", user.UserIsActivated));
                command.Parameters.Add(new MySqlParameter("UserIsLocked", user.UserIsLocked));
                command.Parameters.Add(new MySqlParameter("RegistrationMoment", user.RegistrationMoment));
                command.Parameters.Add(new MySqlParameter("TOTPActivated", user.TOTP.IsActicated));
                command.Parameters.Add(new MySqlParameter("TOTPSecretKey", user.TOTP.SecretKey));
                command.ExecuteNonQuery();
            });
        }

        public bool UserWithIdExists(string userId)
        {
            return this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptUserWithIdExists();
                cmd.Parameters.Add(new MySqlParameter(nameof(userId), userId));
                using MySqlDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            })[0];
        }

        public User GetUserById(string userId)
        {
            User result = this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetUserById();
                cmd.Parameters.Add(new MySqlParameter("Id", userId));
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    User user = new User();
                    user.Id = userId;
                    user.Name = reader.GetString(1);
                    user.PasswordHash = reader.GetString(2);
                    user.EMailAddress = this.ConvertValue<string>(reader["EMailAddress"]);
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
            this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetAllAccessTokenForUser();
                cmd.Parameters.Add(new MySqlParameter("UserId", user.Id));
                using MySqlDataReader reader = cmd.ExecuteReader();
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

        private T? ConvertValue<T>(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }
            else
            {
                return (T)value;
            }
        }

        public User GetUserByName(string userName)
        {
            User result = this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetUserByName();
                cmd.Parameters.Add(new MySqlParameter("Name", userName));
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    User user = new User();
                    user.Id = reader.GetString(0);
                    user.Name = reader.GetString(1);
                    user.PasswordHash = reader.GetString(2);
                    user.EMailAddress = this.ConvertValue<string>(reader["EMailAddress"]);
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
            return this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptRoleExists();
                cmd.Parameters.Add(new MySqlParameter("RoleName", roleName));
                using MySqlDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            })[0];
        }

        public void AddRoleToUser(string userId, string roleId)
        {
            this.RunTransaction((command) =>
            {
                this._Log.Log($"Adding role '{roleId}' to user '{userId}'", LogLevel.Information);
                command.CommandText = this._SQLProvider.GetScriptAddRoleToUser();
                command.Parameters.Add(new MySqlParameter("UserId", userId));
                command.Parameters.Add(new MySqlParameter("RoleId", roleId));
                command.ExecuteNonQuery();
            });
        }

        public void RemoveRoleFromUser(string userId, string roleId)
        {
            throw new NotImplementedException();
        }

        public bool UserHasRole(string userId, string roleId)
        {
            return this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptUserHasRole();
                cmd.Parameters.Add(new MySqlParameter("UserId", userId));
                cmd.Parameters.Add(new MySqlParameter("RoleId", roleId));
                using MySqlDataReader reader = cmd.ExecuteReader();
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
            Role result = this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetRoleByName();
                cmd.Parameters.Add(new MySqlParameter("Name", roleName));
                using MySqlDataReader reader = cmd.ExecuteReader();
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
    }
}
