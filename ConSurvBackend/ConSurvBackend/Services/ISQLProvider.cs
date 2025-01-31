namespace ConSurvBackend.Core.Services
{
    public interface ISQLProvider
    {
        string GetScriptAddRoleToUser();
        string GetScriptAddUser();
        string GetScriptGetAllAccessTokenForUser();
        string GetScriptGetAllRoles();
        string GetScriptGetUserById();
        string GetScriptGetUserByName();
        string GetScriptInsertRole();
        string GetScriptRoleExists();
        string GetScriptUpdateRole();
        string GetScriptUserHasRole();
        string GetScriptUserWithIdExists();
        string GetScriptUserWithNameExists();
    }
}
