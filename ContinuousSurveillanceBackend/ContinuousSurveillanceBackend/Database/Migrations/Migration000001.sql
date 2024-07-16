-- MariaDB-syntax

CREATE TABLE `CurrencyTypes` (
    `Id` tinyint unsigned NOT NULL,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_CurrencyTypes` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

INSERT 
    INTO CurrencyTypes (Id, `Name`) VALUES
        (1, 'Fiat'),
        (2, 'ISIN'),
        (3, 'Crypto');
