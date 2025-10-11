CREATE TABLE "Users" (
    "Id" varchar(255) not null,
    "Name" varchar(255) not null,
    "PasswordHash" varchar(255) null,-- when this properties are null then this means that the user uses an external authenticationprovider (for example OpenID) to login.
    "EMailAddress" varchar(255) null,
    "UserIsActivated" boolean not null,
    "UserIsLocked" boolean not null,
    "RegistrationMoment" timestamp with time zone not null,
    "TOTPActivated" boolean null,
    "TOTPSecretKey" varchar(255) null,
    unique("Name"),
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "AccessToken" (
    "Value" varchar(255) not null,
    "ExpiredMoment" timestamp with time zone not null,
    "UserId" varchar(255) not null,
    CONSTRAINT "PK_AccessToken" PRIMARY KEY ("Value"),
    CONSTRAINT "FK_AccessToken_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users"("Id")
);

CREATE TABLE "RefreshToken" (
    "Value" varchar(255) not null,
    "ExpiredMoment" timestamp with time zone not null,
    "UserId" varchar(255) not null,
    CONSTRAINT "PK_RefreshToken" PRIMARY KEY ("Value"),
    CONSTRAINT "FK_RefreshToken_UserId" FOREIGN KEY ("UserId") REFERENCES "Users"("Id")
);

CREATE TABLE "Roles" (
    "Id" varchar(255) not null,
    "Name" varchar(255) not null,
    CONSTRAINT "PK_Roles" PRIMARY KEY ("Id"),
    unique("Name")
);

CREATE TABLE "User_Roles" (
    "UserId" varchar(255) not null,
    "RoleId" varchar(255) not null,
    CONSTRAINT "FK_User_Roles_UserId" FOREIGN KEY ("UserId") REFERENCES "Users"("Id"),
    CONSTRAINT "FK_User_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Roles"("Id"),
    CONSTRAINT "PK_Role" PRIMARY KEY ("UserId", "RoleId")
);

CREATE TABLE "Role_InheritedRoles" (
    "RoleId" varchar(255) not null,
    "InheritedRoleId" varchar(255) not null,
    CONSTRAINT "FK_Role_InheritedRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Roles"("Id"),
    CONSTRAINT "FK_Role_InheritedRoles_InheritedRoleId" FOREIGN KEY ("InheritedRoleId") REFERENCES "Roles"("Id"),
    CONSTRAINT "PK_Role_InheritedRoles" PRIMARY KEY ("RoleId", "InheritedRoleId")
);


CREATE TABLE "Cameras" (
    "Id" VARCHAR(256) not null,
    "Name" VARCHAR(256) not null,
    "StreamURL" VARCHAR(256) not null,
    "IsONVIFCamera" boolean not null,
    "Certificate" VARCHAR(8192) null,
    "RecordMode" smallint not null,
    "Enabled" boolean not null,
    CONSTRAINT "PK_Cameras" PRIMARY KEY ("Id")
);
