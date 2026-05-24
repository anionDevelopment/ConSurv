using ConSurvBackend.Core.Services;
using GRYLibrary.Core.Misc;

namespace ConSurvBackend.Core.Miscellaneous
{
    /// <summary>
    /// Base class for database-type-specific SQL providers that load SQL scripts from embedded
    /// resources located under <c>ConSurvBackend.Core.Resources.Database.{databaseType}.Statements</c>.
    /// </summary>
    public abstract class SQLProvider : AbstractSQLProvider, ISQLProvider
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SQLProvider"/> and configures the embedded-resource
        /// namespace for the given database type.
        /// </summary>
        /// <param name="databaseType">
        /// The database-type folder name (e.g. <c>MariaDB</c>, <c>PostgreSQL</c>) used to build the
        /// embedded-resource path.
        /// </param>
        public SQLProvider(string databaseType) : base($"ConSurvBackend.Core.Resources.Database.{databaseType}.Statements") { }

        public string GetScriptAddAccessToken()
        {
            return this.LoadSQLScript("AddAccessToken");
        }

        public string GetScriptAddRoleToUser()
        {
            return this.LoadSQLScript("AddRoleToUser");
        }

        public string GetScriptAddUser()
        {
            return this.LoadSQLScript("AddUser");
        }

        public string GetScriptCreateCamera()
        {
            return this.LoadSQLScript("CreateCamera");
        }

        public string GetScriptRemoveCamera()
        {
            return this.LoadSQLScript("RemoveCamera");
        }

        public string GetScriptUpdateCamera()
        {
            return this.LoadSQLScript("UpdateCamera");
        }

        public string GetScriptGetAccessToken()
        {
            return this.LoadSQLScript("GetAccessToken");
        }

        public string GetScriptGetAllAccessTokenForUser()
        {
            return this.LoadSQLScript("GetAllAccessTokenForUser");
        }

        public string GetScriptGetAllCameras()
        {
            return this.LoadSQLScript("GetAllCameras");
        }

        public string GetScriptGetAllInheritedRoleIds()
        {
            return this.LoadSQLScript("GetAllInheritedRoleIds");
        }

        public string GetScriptGetAllRoles()
        {
            return this.LoadSQLScript("GetAllRoles");
        }

        public string GetScriptGetDirectlyInheritedRoleIds()
        {
            return this.LoadSQLScript("GetDirectlyInheritedRoleIds");
        }

        public string GetScriptGetRoleById()
        {
            return this.LoadSQLScript("GetRoleById");
        }

        public string GetScriptGetRoleByName()
        {
            return this.LoadSQLScript("GetRoleByName");
        }

        public string GetScriptGetUserById()
        {
            return this.LoadSQLScript("GetUserById");
        }

        public string GetScriptGetUserByName()
        {
            return this.LoadSQLScript("GetUserByName");
        }

        public string GetScriptInsertRole()
        {
            return this.LoadSQLScript("InsertRole");
        }

        public string GetScriptIsCamera()
        {
            return this.LoadSQLScript("IsCamera");
        }

        public string GetScriptResetDatabase()
        {
            return this.LoadSQLScript("ResetDatabase");
        }

        public string GetScriptRoleExists()
        {
            return this.LoadSQLScript("RoleExists");
        }

        public string GetScriptAddDirectlyInheritedRoles()
        {
            return this.LoadSQLScript("AddDirectlyInheritedRoles");
        }

        public string GetScriptDeleteDirectlyInheritedRoles()
        {
            return this.LoadSQLScript("DeleteDirectlyInheritedRoles");
        }

        public string GetScriptUpdateRole()
        {
            return this.LoadSQLScript("UpdateRole");
        }

        public string GetScriptUserHasRole()
        {
            return this.LoadSQLScript("UserHasRole");
        }

        public string GetScriptUserWithIdExists()
        {
            return this.LoadSQLScript("UserWithIdExists");
        }

        public string GetScriptUserWithNameExists()
        {
            return this.LoadSQLScript("UserWithNameExists");
        }
    }
}
