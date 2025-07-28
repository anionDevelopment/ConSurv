using ConSurvBackend.Core.Configuration;
using ConSurvBackend.Core.Database;
using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.RecordModes;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Trans;
using GRYLibrary.Core.APIServer.Settings.Configuration;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sprache;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using GUtilities = GRYLibrary.Core.Misc.Utilities;

namespace ConSurvBackend.Core.Misc
{
    public abstract class GenericDatabasePersistence : IPersistence, IInitializable
    {
        private DatabaseContext _DatabaseContext;
        private readonly ISQLProvider _SQLProvider;
        private static readonly object _Lock = new object();
        private readonly Semaphore _Semaphore = new Semaphore();
        private readonly IGRYLog _Log;
        private readonly ITimeService _TimeService;
        private readonly IDatabaseManager _DatabaseManager;
        private readonly IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> _PersistedAPIServerConfiguration;
        private readonly DbContextOptions<DatabaseContext> _Options;
        public bool IsInitialized { get; private set; }
        public GenericDatabasePersistence(DbContextOptions<DatabaseContext> options, ITimeService timeService, IDatabaseManager databaseManager, IGRYLog log, ISQLProvider sqlProvider, IPersistedAPIServerConfiguration<CodeUnitSpecificConfiguration> persistedAPIServerConfiguration)
        {
            this._TimeService = timeService;
            this._Log = log;
            this._Options = options;
            this._SQLProvider = sqlProvider;
            this._DatabaseManager = databaseManager;
            this._PersistedAPIServerConfiguration = persistedAPIServerConfiguration;
        }

        public void Initialize()
        {
            this._DatabaseContext = new DatabaseContext(_Options, _Log, _TimeService, _DatabaseManager, _PersistedAPIServerConfiguration);
            if (!IsInitialized)
            {
                _DatabaseContext.Initialize();
                IsInitialized = true;
            }
        }

        public abstract DbParameter GetParameter(string parameterName, object? value, Type type);
        public DbParameter GetParameter(string parameterName, object value)
        {
            GUtilities.AssertCondition(value != null, $"value for parameter {parameterName} is null, so a speicfic type for it must be set.");
            return this.GetParameter(parameterName, value, value.GetType());
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
        public void RunTransaction(params Action<DbCommand>[] actions)
        {
            this.RunTransaction(actions.Select<Action<DbCommand>, Func<DbCommand, object>>(action => (command) =>
            {
                action(command);
                return null;
            }
            ).ToArray());
        }

        public T[] RunTransaction<T>(params Func<DbCommand, T>[] functions)
        {
            List<T> results = new List<T>();
            this.AccessDatabase(context =>
            {
                DbConnection connection = context.Connection;
                GUtilities.AssertNotNull(connection, nameof(connection));
                using DbTransaction transaction = connection.BeginTransaction();
                bool commit = true;
                try
                {
                    foreach (Func<DbCommand, T> function in functions)
                    {
                        using (DbCommand cmd = connection.CreateCommand())
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
                command.Parameters.Add(this.GetParameter("Id", camera.Id));
                command.Parameters.Add(this.GetParameter("Name", camera.Name));
                command.Parameters.Add(this.GetParameter("StreamURL", camera.VideoInformation.StreamURL));
                command.Parameters.Add(this.GetParameter("IsONVIFCamera", camera.VideoInformation.IsONVIFCamera));
                command.Parameters.Add(this.GetParameter("Certificate", camera.VideoInformation.Certificate, typeof(string)));
                command.Parameters.Add(this.GetParameter("RecordMode", RecordMode.ToNumber(camera.RecordMode.GetType())));
                command.Parameters.Add(this.GetParameter("Enabled", camera.Enabled));
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
            IDictionary<string, Camera> roles = this.RunTransaction((command) =>
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
                            Certificate = DBUtilities.GetValue<string>(reader, 4, true),
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
                cmd.Parameters.Add(this.GetParameter("UserName", userName));
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
            ISet<Role> roles = this.RunTransaction((command) =>
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
            this.RunTransaction((command) =>
            {
                command.CommandText = this._SQLProvider.GetScriptInsertRole();
                command.Parameters.Add(this.GetParameter("Id", role.Id));
                command.Parameters.Add(this.GetParameter("Name", role.Name));
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
                cmd.Parameters.Add(this.GetParameter("Id", role.Id));
                cmd.Parameters.Add(this.GetParameter("Name", role.Name));
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
            this.RunTransaction((command) =>
            {
                command.CommandText = this._SQLProvider.GetScriptAddUser();
                command.Parameters.Add(this.GetParameter("Id", user.Id));
                command.Parameters.Add(this.GetParameter("Name", user.Name));
                command.Parameters.Add(this.GetParameter("PasswordHash", user.PasswordHash));
                command.Parameters.Add(this.GetParameter("EMailAddress", user.EMailAddress));
                command.Parameters.Add(this.GetParameter("UserIsActivated", user.UserIsActivated));
                command.Parameters.Add(this.GetParameter("UserIsLocked", user.UserIsLocked));
                command.Parameters.Add(this.GetParameter("RegistrationMoment", user.RegistrationMoment));
                command.Parameters.Add(this.GetParameter("TOTPActivated", user.TOTP.IsActicated));
                command.Parameters.Add(this.GetParameter("TOTPSecretKey", user.TOTP.SecretKey));
                command.ExecuteNonQuery();
            });
        }

        public bool UserWithIdExists(string userId)
        {
            return this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptUserWithIdExists();
                cmd.Parameters.Add(this.GetParameter(nameof(userId), userId));
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            })[0];
        }

        public User GetUserById(string userId)
        {
            User result = this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetUserById();
                cmd.Parameters.Add(this.GetParameter("Id", userId));
                using DbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    User user = new User();
                    user.Id = userId;
                    user.Name = reader.GetString(1);
                    user.PasswordHash = reader.GetString(2);
                    user.EMailAddress = DBUtilities.ConvertValue<string>(reader["EMailAddress"]);
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
                cmd.Parameters.Add(this.GetParameter("UserId", user.Id));
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
            User result = this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetUserByName();
                cmd.Parameters.Add(this.GetParameter("Name", userName));
                using DbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    User user = new User();
                    user.Id = reader.GetString(0);
                    user.Name = reader.GetString(1);
                    user.PasswordHash = reader.GetString(2);
                    user.EMailAddress = DBUtilities.ConvertValue<string>(reader["EMailAddress"]);
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
                cmd.Parameters.Add(this.GetParameter("RoleName", roleName));
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            })[0];
        }

        public void AddRoleToUser(string userId, string roleId)
        {
            this.RunTransaction((command) =>
            {
                this._Log.Log($"Adding role '{roleId}' to user '{userId}'", LogLevel.Information);
                command.CommandText = this._SQLProvider.GetScriptAddRoleToUser();
                command.Parameters.Add(this.GetParameter("UserId", userId));
                command.Parameters.Add(this.GetParameter("RoleId", roleId));
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
                cmd.Parameters.Add(this.GetParameter("UserId", userId));
                cmd.Parameters.Add(this.GetParameter("RoleId", roleId));
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
            Role result = this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetRoleByName();
                cmd.Parameters.Add(this.GetParameter("Name", roleName));
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
            this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptResetDatabase();
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            });
        }

        public bool IsCamera(string id)
        {
            return this.RunTransaction((cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptIsCamera();
                cmd.Parameters.Add(this.GetParameter("CameraId", id));
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

    }
}
