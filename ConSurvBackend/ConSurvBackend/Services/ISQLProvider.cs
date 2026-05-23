namespace ConSurvBackend.Core.Services
{
    /// <summary>
    /// Provides database-engine-specific SQL scripts used by the persistence layer.
    /// Each method returns a parameterized SQL script string ready to be executed via a <see cref="System.Data.Common.DbCommand"/>.
    /// </summary>
    public interface ISQLProvider
    {
        /// <summary>Returns the SQL script that assigns a role to a user.</summary>
        string GetScriptAddRoleToUser();

        /// <summary>Returns the SQL script that inserts a new user record.</summary>
        string GetScriptAddUser();

        /// <summary>Returns the SQL script that retrieves all access tokens belonging to a specific user.</summary>
        string GetScriptGetAllAccessTokenForUser();

        /// <summary>Returns the SQL script that retrieves all roles.</summary>
        string GetScriptGetAllRoles();

        /// <summary>Returns the SQL script that retrieves a user by their id.</summary>
        string GetScriptGetUserById();

        /// <summary>Returns the SQL script that retrieves a role by its name.</summary>
        string GetScriptGetRoleByName();

        /// <summary>Returns the SQL script that retrieves a user by their username.</summary>
        string GetScriptGetUserByName();

        /// <summary>Returns the SQL script that inserts a new role record.</summary>
        string GetScriptInsertRole();

        /// <summary>Returns the SQL script that checks whether a role with a given name exists.</summary>
        string GetScriptRoleExists();

        /// <summary>Returns the SQL script that updates an existing role record.</summary>
        string GetScriptUpdateRole();

        /// <summary>Returns the SQL script that checks whether a user has a specific role.</summary>
        string GetScriptUserHasRole();

        /// <summary>Returns the SQL script that checks whether a user with a given id exists.</summary>
        string GetScriptUserWithIdExists();

        /// <summary>Returns the SQL script that checks whether a user with a given name exists.</summary>
        string GetScriptUserWithNameExists();

        /// <summary>Returns the SQL script that retrieves all camera records.</summary>
        string GetScriptGetAllCameras();

        /// <summary>Returns the SQL script that inserts a new camera record.</summary>
        string GetScriptCreateCamera();

        /// <summary>Returns the SQL script that deletes a camera record by its id.</summary>
        string GetScriptRemoveCamera();

        /// <summary>Returns the SQL script that updates an existing camera record.</summary>
        string GetScriptUpdateCamera();

        /// <summary>Returns the SQL script that resets (truncates or drops and recreates) the database schema.</summary>
        string GetScriptResetDatabase();

        /// <summary>Returns the SQL script that checks whether an entity with a given id is a camera.</summary>
        string GetScriptIsCamera();

        /// <summary>Returns the SQL script that inserts a new access token record.</summary>
        string GetScriptAddAccessToken();

        /// <summary>Returns the SQL script that retrieves an access token by its value.</summary>
        string GetScriptGetAccessToken();

        /// <summary>Returns the SQL script that retrieves all transitively inherited role ids for a given role.</summary>
        string GetScriptGetAllInheritedRoleIds();

        /// <summary>Returns the SQL script that retrieves a role by its id.</summary>
        string GetScriptGetRoleById();

        /// <summary>Returns the SQL script that retrieves the directly inherited role ids for a given role.</summary>
        string GetScriptGetDirectlyInheritedRoleIds();

        /// <summary>Returns the SQL script that inserts directly inherited role relationships for a role.</summary>
        string GetScriptAddDirectlyInheritedRoles();

        /// <summary>Returns the SQL script that deletes all directly inherited role relationships for a role.</summary>
        string GetScriptDeleteDirectlyInheritedRoles();
    }
}
