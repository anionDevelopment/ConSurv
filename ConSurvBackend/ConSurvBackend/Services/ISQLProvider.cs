namespace ConSurvBackend.Core.Services
{
    public interface ISQLProvider
    {
        string GetScriptAddRoleToUser();
        string GetScriptAddUser();
        string GetScriptGetAllAccessTokenForUser();
        string GetScriptGetAllRoles();
        string GetScriptGetUserById();
        string GetScriptGetRoleByName();
        string GetScriptGetUserByName();
        string GetScriptInsertRole();
        string GetScriptRoleExists();
        string GetScriptUpdateRole();
        string GetScriptUserHasRole();
        string GetScriptUserWithIdExists();
        string GetScriptUserWithNameExists();
        string GetScriptGetAllCameras();
        string GetScriptCreateCamera();
        string GetScriptResetDatabase();
        string GetScriptIsCamera();
        string GetScriptAddAccessToken();
        string GetScriptGetAccessToken();
        string GetScriptGetAllInheritedRoleIds();
        string GetScriptGetRoleById();
    }
}
