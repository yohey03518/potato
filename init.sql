CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Executions` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `OrderId` char(36) COLLATE ascii_general_ci NOT NULL,
    `Price` decimal(18,4) NOT NULL,
    `Quantity` int NOT NULL,
    `ExecutedAt` datetime(6) NOT NULL,
    `Fee` decimal(18,4) NOT NULL,
    `Tax` decimal(18,4) NOT NULL,
    CONSTRAINT `PK_Executions` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `KLines` (
    `Symbol` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `StartTime` datetime(6) NOT NULL,
    `Open` decimal(18,4) NOT NULL,
    `High` decimal(18,4) NOT NULL,
    `Low` decimal(18,4) NOT NULL,
    `Close` decimal(18,4) NOT NULL,
    `Volume` bigint NOT NULL,
    `SMA20` decimal(18,4) NOT NULL,
    CONSTRAINT `PK_KLines` PRIMARY KEY (`Symbol`, `StartTime`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `MarketTicks` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Symbol` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Price` decimal(18,4) NOT NULL,
    `Volume` bigint NOT NULL,
    `Timestamp` datetime(6) NOT NULL,
    CONSTRAINT `PK_MarketTicks` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Orders` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Symbol` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Side` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Type` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Price` decimal(18,4) NOT NULL,
    `Quantity` int NOT NULL,
    `Status` longtext CHARACTER SET utf8mb4 NOT NULL,
    `QueuePositionVolume` bigint NOT NULL,
    `FilledVolume` int NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_Orders` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Watchlists` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `TradeDate` datetime(6) NOT NULL,
    `Symbol` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Direction` int NOT NULL,
    `BasePrice` decimal(18,4) NOT NULL,
    `MA20_Day` decimal(18,4) NOT NULL,
    CONSTRAINT `PK_Watchlists` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE UNIQUE INDEX `IX_Watchlists_Symbol_TradeDate` ON `Watchlists` (`Symbol`, `TradeDate`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260114162828_InitialCreate', '8.0.11');

COMMIT;

