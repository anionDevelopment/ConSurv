-- MariaDB-syntax

CREATE TABLE `User` (
    `Id` VARCHAR(256) NOT NULL,
    `Name` VARCHAR(256) NOT NULL,
    `PasswordHash` NOT NULL,
    CONSTRAINT `PK_User` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Cameras` (
    `Id` VARCHAR(256) NOT NULL,
    `Name` VARCHAR(256) NOT NULL,
    `Url` VARCHAR(256) NOT NULL,
    CONSTRAINT `PK_Cameras` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;
