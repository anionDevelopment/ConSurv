using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.RecordModes;
using GRYLibrary.Core.APIServer.CommonAuthenticationTypes;
using GRYLibrary.Core.APIServer.CommonDBTypes;
using GRYLibrary.Core.APIServer.Services.Database;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Logger;
using GRYLibrary.Core.APIServer.Utilities;
using GRYLibrary.Core.APIServer.Utilities.InitializationStates;
using GRYLibrary.Core.Logging.GRYLogger;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Role = GRYLibrary.Core.APIServer.CommonDBTypes.Role;

namespace ConSurvBackend.Core.Services
{
    public sealed class DatabasePersistence : IInitializable, IPersistence
    {
        private readonly ISQLProvider _SQLProvider;
        private static readonly object _Lock = new object();
        private readonly ITimeService _TimeService;
        private readonly IGRYLog _Log;
        internal readonly IConSurvDatabaseInteractor _Database;
        private readonly IDatabasePersistenceConfiguration _Configuration;
        public InitializationState InitializationState { get; private set; }

        public DatabasePersistence(IConSurvDatabaseInteractor database, ITimeService timeService, IServerLog log, IDatabasePersistenceConfiguration configuration)
        {
            this._TimeService = timeService;
            this._Database = database;
            this._Log = log.Logger;
            this._SQLProvider = database.GetSQLProvider();
            this.InitializationState = new Uninitialized();
            this._Configuration = configuration;
        }

        #region AccessDatabase
        /// <summary>
        /// Executes the given <paramref name="action"/> against the database interactor within an exclusive lock.
        /// </summary>
        /// <param name="action">The database operation to perform.</param>
        protected void AccessDatabase(Action<IConSurvDatabaseInteractor> action)
        {
            lock (_Lock)
            {
                DBUtilities.AccessDatabase<IConSurvDatabaseInteractor>(this._Database, action);
            }
        }

        /// <summary>
        /// Executes the given <paramref name="function"/> against the database interactor within an exclusive lock and returns its result.
        /// </summary>
        /// <typeparam name="T">The return type of the database operation.</typeparam>
        /// <param name="function">The database operation to perform.</param>
        /// <returns>The value returned by <paramref name="function"/>.</returns>
        protected T AccessDatabase<T>(Func<IConSurvDatabaseInteractor, T> function)
        {
            lock (_Lock)
            {
                return DBUtilities.AccessDatabase<T, IConSurvDatabaseInteractor>(this._Database, function);
            }
        }
        /// <summary>
        /// Executes one or more database commands within an exclusive lock, optionally wrapped in a transaction.
        /// </summary>
        /// <param name="nameOfAction">A label for the operation, used in log messages.</param>
        /// <param name="runTransactional">If <c>true</c>, all actions are executed in a single database transaction.</param>
        /// <param name="actions">The commands to execute in order.</param>
        protected void RunTransaction(string nameOfAction, bool runTransactional, params Action<DbCommand>[] actions)
        {
            lock (_Lock)
            {
                DBUtilities.RunTransaction<IConSurvDatabaseInteractor>(nameOfAction, this._Log, this._Database, runTransactional, actions);
            }
        }

        /// <summary>
        /// Executes one or more database functions within an exclusive lock, optionally wrapped in a transaction, and collects their return values.
        /// </summary>
        /// <typeparam name="T">The return type of each function.</typeparam>
        /// <param name="nameOfAction">A label for the operation, used in log messages.</param>
        /// <param name="runTransactional">If <c>true</c>, all functions are executed in a single database transaction.</param>
        /// <param name="functions">The functions to execute in order.</param>
        /// <returns>An array containing the return value of each function in the same order.</returns>
        protected T?[] RunTransaction<T>(string nameOfAction, bool runTransactional, params Func<DbCommand, T?>[] functions)
        {
            lock (_Lock)
            {
                return DBUtilities.RunTransaction<T, IConSurvDatabaseInteractor>(nameOfAction, this._Log, this._Database, runTransactional, functions);
            }
        }

        #endregion

        /// <inheritdoc />
        public void CreateCamera(Camera camera)
        {
            this.RunTransaction(nameof(CreateCamera), true, (command) =>
            {
                command.CommandText = this._SQLProvider.GetScriptCreateCamera();
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Id", camera.Id));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Name", camera.Name));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("StreamURL", camera.VideoInformation.StreamURL, typeof(string)));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("SupportsPTZViaONVIF", camera.VideoInformation.SupportsPTZViaONVIF));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Certificate", camera.VideoInformation.Certificate, typeof(string)));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("RecordMode", RecordMode.ToNumber(camera.RecordMode.GetType())));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Enabled", camera.Enabled));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("ONVIFUrl", camera.VideoInformation.ONVIFUrl, typeof(string)));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("ONVIFUsername", camera.VideoInformation.ONVIFUsername, typeof(string)));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("ONVIFPassword", camera.VideoInformation.ONVIFPassword, typeof(string)));
                command.ExecuteNonQuery();
            });
        }

        /// <inheritdoc />
        public void RemoveCamera(string cameraId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void UpdateCamera(Camera camera)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<string, Camera> GetAllCameras()
        {
            IDictionary<string, Camera> roles = this.RunTransaction(nameof(GetAllCameras), true, (command) =>
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
                            SupportsPTZViaONVIF = reader.GetBoolean(3),
                            Certificate = DBUtilities.GetNullableValue<string>(reader, 4),
                        };
                        camera.RecordMode = RecordMode.FromNumberToInstance(reader.GetByte(5));
                        camera.Enabled = reader.GetBoolean(6);
                        camera.VideoInformation.ONVIFUrl = DBUtilities.GetNullableValue<string>(reader, 7);
                        camera.VideoInformation.ONVIFUsername = DBUtilities.GetNullableValue<string>(reader, 8);
                        camera.VideoInformation.ONVIFPassword = DBUtilities.GetNullableValue<string>(reader, 9);
                        cameraDictionary[id] = camera;
                    }
                    reader.Close();
                    return cameraDictionary;
                }
                ;
            })[0];
            return roles;
        }

        /// <inheritdoc />
        public (bool, Exception?) IsAvailable()
        {
            bool result = this._Database.GetGenericDatabaseInteractor().TryGetConnection(out _, out Exception? e);
            return (result, e);
        }

        public void Dispose()
        {
            //TODO
        }

        /// <inheritdoc />
        public bool UserWithNameExists(string userName)
        {
            return this.RunTransaction(nameof(UserWithNameExists), true, (cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptUserWithNameExists();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("UserName", userName));
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            })[0];
        }

        /// <inheritdoc />
        public IDictionary<string, User> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ISet<Role> GetAllRoles()
        {
            ISet<Role> roles = this.RunTransaction(nameof(GetAllRoles), true, (command) =>
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
                this.EnrichWithDirectlyInheritedRoles(role);
            }
            return roles;
        }

        /// <inheritdoc />
        public void AddRole(Role role)
        {
            this.RunTransaction(nameof(AddRole), true, (command) =>
            {
                command.CommandText = this._SQLProvider.GetScriptInsertRole();
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Id", role.Id));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Name", role.Name));
                command.ExecuteNonQuery();
            }, (command) =>
            {
                //TODO check if inherited roles must be updated
            });
        }

        /// <inheritdoc />
        public void UpdateRole(Role role)
        {
            this.RunTransaction(nameof(UpdateRole), true, (cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptUpdateRole();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Id", role.Id));
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Name", role.Name));
                cmd.ExecuteNonQuery();
            }, (cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptDeleteDirectlyInheritedRoles();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("RoleId", role.Id));
                cmd.ExecuteNonQuery();
            }, (cmd) =>
            {
                if (role.DirectlyInheritedRoles.Any())
                {
                    List<string> insertLines = new List<string>();
                    uint index = 0;
                    cmd.Parameters.Clear();
                    foreach (Role directlyInheritedRole in role.DirectlyInheritedRoles)
                    {
                        insertLines.Add($"(@RoleId{index}, @InheritedRoleId{index})");//attention: this syntax must always be both: mariadb-compliant and postgresql-compliant
                        cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter($"RoleId{index}", role.Id));
                        cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter($"InheritedRoleId{index}", directlyInheritedRole.Id));
                        index++;
                    }
                    cmd.CommandText = this._SQLProvider.GetScriptAddDirectlyInheritedRoles().Replace("__generated__", string.Join(",\n        ", insertLines));
                    cmd.ExecuteNonQuery();
                }
            });
        }

        /// <inheritdoc />
        public void DeleteRoleByName(string roleName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool AccessTokenExists(string accessToken, out User user)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void AddUser(User user)
        {
            this.RunTransaction(nameof(AddUser), true, (command) =>
            {
                command.CommandText = this._SQLProvider.GetScriptAddUser();
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Id", user.Id));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Name", user.Name));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("PasswordHash", user.PasswordHash, typeof(string)));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("EMailAddress", user.EMailAddress, typeof(string)));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("UserIsActivated", user.UserIsActivated));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("UserIsLocked", user.UserIsLocked));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("RegistrationMoment", user.RegistrationMoment));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("TOTPActivated", user.TOTP?.IsActicated, typeof(bool)));
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("TOTPSecretKey", user.TOTP?.SecretKey, typeof(string)));
                command.ExecuteNonQuery();
            });
        }

        /// <inheritdoc />
        public bool UserWithIdExists(string userId)
        {
            return this.RunTransaction(nameof(UserWithIdExists), true, (cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptUserWithIdExists();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter(nameof(userId), userId));
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            })[0];
        }

        /// <inheritdoc />
        public User GetUserById(string userId)
        {
            User result = this.RunTransaction(nameof(GetUserById), true, (cmd) =>
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
            this.EnrichWithRoles(result);
            this.EnrichWithAccessToken(result);
            this.EnrichWithTOTPToken(result);
            return result;
        }

        private void EnrichWithRoles(User user)
        {
            ISet<Role> allRoles = this.GetAllRoles();
            foreach (Role role in allRoles)
            {
                if (this.UserHasRole(user.Id, role.Id))
                {
                    user.Roles.Add(role);
                }
            }
        }

        private void EnrichWithDirectlyInheritedRoles(Role role)
        {
            //TODO loading inherited roles is very inperformant currently, this should be optimized
            ISet<string> inheritedRoleIds = this.RunTransaction(nameof(EnrichWithDirectlyInheritedRoles), true, (command) =>
            {
                ISet<string> inheritedRoleIdsInternal = new HashSet<string>();
                command.CommandText = this._SQLProvider.GetScriptGetDirectlyInheritedRoleIds();
                command.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("RoleId", role.Id));
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inheritedRoleIdsInternal.Add(reader.GetString(0));
                    }
                    reader.Close();
                    return inheritedRoleIdsInternal;
                }
                ;
            })[0];
            foreach (string inheritedRoleId in inheritedRoleIds)
            {
                role.DirectlyInheritedRoles.Add(this.GetRoleById(inheritedRoleId));
            }
        }

        private void EnrichWithTOTPToken(User result)
        {
            //TODO
        }

        private void EnrichWithAccessToken(User user)
        {
            this.RunTransaction(nameof(EnrichWithAccessToken), true, (cmd) =>
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
            User result = this.RunTransaction(nameof(GetUserByName), true, (cmd) =>
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
            this.EnrichWithRoles(result);
            this.EnrichWithAccessToken(result);
            this.EnrichWithTOTPToken(result);
            return result;
        }

        public void RemoveUser(string userId)
        {
            throw new NotImplementedException();
        }

        public bool RoleExists(string roleName)
        {
            return this.RunTransaction(nameof(RoleExists), true, (cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptRoleExists();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("RoleName", roleName));
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            })[0];
        }

        public void AddRoleToUser(string userId, string roleId)
        {
            this.RunTransaction(nameof(AddRoleToUser), true, (command) =>
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
            return this.RunTransaction(nameof(UserHasRole), true, (cmd) =>
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
            return this.RunTransaction(nameof(GetAccessToken), true, (cmd) =>
             {
                 cmd.CommandText = this._SQLProvider.GetScriptGetAccessToken();
                 cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Value", accessToken));
                 using DbDataReader reader = cmd.ExecuteReader();
                 if (reader.HasRows)
                 {
                     reader.Read();
                     AccessToken accessTokenResult = new AccessToken();
                     accessTokenResult.Value = accessToken;
                     accessTokenResult.OwnerUserId = reader.GetString(0);
                     accessTokenResult.ExpiredMoment = reader.GetDateTime(1);
                     return accessTokenResult;
                 }
                 else
                 {
                     throw new KeyNotFoundException($"Accesstoken '{accessToken}' not found.");
                 }
             })[0];
        }

        public void AddAccessToken(AccessToken newAccessToken)
        {
            this.RunTransaction(nameof(AddAccessToken), true, (cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptAddAccessToken();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Value", newAccessToken.Value));
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("ExpiredMoment", newAccessToken.ExpiredMoment));
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("UserId", newAccessToken.OwnerUserId));
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            });
        }
        public void RemoveAccessToken(string accessToken)
        {
            throw new NotImplementedException();
        }

        public ISet<AccessToken> GetAllAccessTokenOfUser(string userId)
        {
            throw new NotImplementedException();
        }

        public Role GetRoleById(string roleId)
        {
            Role result = this.RunTransaction(nameof(GetRoleById), true, (cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetRoleById();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Id", roleId));
                using DbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    Role role = new Role();
                    role.Id = roleId;
                    role.Name = reader.GetString(0);
                    return role;
                }
                else
                {
                    throw new KeyNotFoundException($"No role found with id '{roleId}'");
                }
            })[0];
            this.EnrichWithDirectlyInheritedRoles(result);
            return result;
        }

        public Role GetRoleByName(string roleName)
        {
            Role result = this.RunTransaction(nameof(GetRoleByName), true, (cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptGetRoleByName();
                cmd.Parameters.Add(this._Database.GetGenericDatabaseInteractor().GetParameter("Name", roleName));
                using DbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    Role role = new Role();
                    role.Id = reader.GetString(0);
                    role.Name = roleName;
                    return role;
                }
                else
                {
                    throw new KeyNotFoundException($"No role found with rolename '{roleName}'");
                }
            })[0];
            this.EnrichWithDirectlyInheritedRoles(result);
            return result;
        }

        public void Reset()
        {
            this.RunTransaction(nameof(Reset), false, (cmd) =>
            {
                cmd.CommandText = this._SQLProvider.GetScriptResetDatabase();
                using DbDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            });
        }

        public bool IsCamera(string id)
        {
            return this.RunTransaction(nameof(IsCamera), true, (cmd) =>
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

        public void Initialize()
        {
            lock (_Lock)
            {
                try
                {
                    this.InitializationState = new Initializing();
                    this._Database.GetGenericDatabaseInteractor().DoAllMigrations(this._Database.GetAllMigrations(), this._TimeService);
                    this.InitializationState = new Initialized();
                }
                catch (Exception ex)
                {
                    this.InitializationState = new InitializationFailed();
                    this._Log.Log("Database-initialization failed.", ex);
                }
            }
        }

        public void WaitUntilAvailable(TimeSpan timeSpan)
        {
            this._Log.Log("Wait until database is available...");
            this._Log.Log($"Used connection-string: \"{this._Database.GetGenericDatabaseInteractor().EscapePasswordInConnectionString(this._Configuration.DatabaseConnectionString)}\"", LogLevel.Debug);
            this._Database.GetGenericDatabaseInteractor().WaitUntilAvailable(timeSpan);
        }
    }
}
