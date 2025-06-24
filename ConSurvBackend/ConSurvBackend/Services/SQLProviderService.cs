using GRYLibrary.Core.Misc;

namespace ConSurvBackend.Core.Services
{
    public class SQLProviderService : AbstractSQLProvider, ISQLProvider
    {
        public SQLProviderService() : base("ConSurvBackend.Core.Resources.Database.SQL") { }

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

        public string GetScriptGetAllAccessTokenForUser()
        {
            return this.LoadSQLScript("GetAllAccessTokenForUser");
        }

        public string GetScriptGetAllCameras()
        {
            return this.LoadSQLScript("GetAllCameras");
        }

        public string GetScriptGetAllRoles()
        {
            return this.LoadSQLScript("GetAllRoles");
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

        public string GetScriptRoleExists()
        {
            return this.LoadSQLScript("RoleExists");
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
