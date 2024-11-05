-- MariaDB-syntax
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `User` (
    `Id` VARCHAR(256) not null,
    `Name` VARCHAR(256) not null,
    `PasswordHash` VARCHAR(256) not null,
    CONSTRAINT `PK_User` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Cameras` (
    `Id` VARCHAR(256) not null,
    `Name` VARCHAR(256) not null,
    `Url` VARCHAR(256) not null,
    CONSTRAINT `PK_Cameras` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;
