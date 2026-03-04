using ConSurvBackend.Core.Services;
using GRYLibrary.Core.Misc;

namespace ConSurvBackend.Core.Miscellaneous
{
    public abstract class SQLProvider : AbstractSQLProvider, ISQLProvider
    {
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
