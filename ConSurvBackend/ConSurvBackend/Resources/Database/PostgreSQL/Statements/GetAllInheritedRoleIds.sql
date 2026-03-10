WITH RECURSIVE RoleHierarchy AS (
    SELECT 
        r."InheritedRoleId"
    FROM "Role_InheritedRoles" r
    WHERE r."RoleId" = @RoleId

    UNION

    SELECT 
        r2."InheritedRoleId"
    FROM "Role_InheritedRoles" r2
    INNER JOIN RoleHierarchy rh 
        ON r2."RoleId" = rh."InheritedRoleId"
)
SELECT DISTINCT "InheritedRoleId"
FROM RoleHierarchy;
