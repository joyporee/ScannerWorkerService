USE master;
GO

IF EXISTS(SELECT TOP 1 1 FROM sys.databases WHERE name = 'ScannerWSDB')
BEGIN
	ALTER DATABASE ScannerWSDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE ScannerWSDB;
END
GO
CREATE DATABASE ScannerWSDB
GO

USE ScannerWSDB;
GO

DECLARE	@allow_drop			CHAR(1)
SET		@allow_drop = 'Y'

--------------------------------------------
-- ScanType
--------------------------------------------
IF OBJECT_ID('dbo.ScanType') IS NOT NULL AND @allow_drop = 'Y'
BEGIN
	DROP TABLE dbo.ScanType
	PRINT '<< Dropped table ScanType >>'
END

IF OBJECT_ID('dbo.ScanType') IS NULL
BEGIN
	CREATE TABLE dbo.ScanType (
		Id						INT,
		Description				VARCHAR(250),

		CONSTRAINT PK_ScanType PRIMARY KEY (Id)
	)

	GRANT INSERT, UPDATE, DELETE, SELECT ON ScanType TO public
	--Permission granting to a specific secuired UserGroup or at the moment to public
	
	PRINT '<< Created table ScanType >>'
END

--------------------------------------------
-- Users
--------------------------------------------
IF OBJECT_ID('dbo.Users') IS NOT NULL AND @allow_drop = 'Y'
BEGIN
	DROP TABLE dbo.Users
	PRINT '<< Dropped table Users >>'
END

IF OBJECT_ID('dbo.Users') IS NULL
BEGIN
	CREATE TABLE dbo.Users (
		Id						VARCHAR(30),
		CarrierId				VARCHAR(20),
		RunId					INT,

		CONSTRAINT PK_Users PRIMARY KEY (Id)
	)

	GRANT INSERT, UPDATE, DELETE, SELECT ON Users TO public
	--Permission granting to a specific secuired UserGroup or at the moment to public
	
	PRINT '<< Created table Users >>'
END

--------------------------------------------
-- Device
--------------------------------------------
IF OBJECT_ID('dbo.Device') IS NOT NULL AND @allow_drop = 'Y'
BEGIN
	DROP TABLE dbo.Device
	PRINT '<< Dropped table Device >>'
END

IF OBJECT_ID('dbo.Device') IS NULL
BEGIN
	CREATE TABLE dbo.Device (
		Id						INT,
		Name					VARCHAR(250),

		CONSTRAINT PK_Device PRIMARY KEY (Id)--,
		--CONSTRAINT FK_Tranaction FOREIGN KEY (id) REFERENCES ScanRecords(id)
	)

	GRANT INSERT, UPDATE, DELETE, SELECT ON Device TO public
	--Permission granting to a specific secuired UserGroup or at the moment to public
	
	PRINT '<< Created table Device >>'
END

--------------------------------------------
-- ScanRecords
--------------------------------------------
IF OBJECT_ID('dbo.ScanRecords') IS NOT NULL AND @allow_drop = 'Y'
BEGIN
	DROP TABLE dbo.ScanRecords
	PRINT '<< Dropped table ScanRecords >>'
END

IF OBJECT_ID('dbo.ScanRecords') IS NULL
BEGIN
	CREATE TABLE dbo.ScanRecords (
		EventId					BIGINT,
		ParcelId				BIGINT,
		ScanTypeId				INT,		
		CreatedDateTimeUtc		DATETIME		DEFAULT GETDATE(),
		StatusCode				VARCHAR(50),
		DeviceId				INT,
		UserId					VARCHAR(30),
		LastUpdateDateTimeUtc	DATETIME		NULL,		
		CONSTRAINT PK_ScanRecords PRIMARY KEY (EventId),
		CONSTRAINT FK_ScanType FOREIGN KEY (ScanTypeId) REFERENCES ScanType(Id),
		CONSTRAINT FK_Device FOREIGN KEY (DeviceId) REFERENCES Device(Id),
		CONSTRAINT FK_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
	)

	GRANT INSERT, UPDATE, DELETE, SELECT ON ScanRecords TO public
	--Permission granting to a specific secuired UserGroup or at the moment to public
	
	PRINT '<< Created table ScanRecords >>'
END

--------------------------------------------
-- ScanEventErrors
--------------------------------------------
IF OBJECT_ID('dbo.ScanEventErrors') IS NOT NULL AND @allow_drop = 'Y'
BEGIN
	DROP TABLE dbo.ScanEventErrors
	PRINT '<< Dropped table ScanEventErrors >>'
END

IF OBJECT_ID('dbo.ScanEventErrors') IS NULL
BEGIN
	CREATE TABLE dbo.ScanEventErrors (
		Id						BIGINT				IDENTITY(1,1),
		EventId					BIGINT,
		Message					VARCHAR(MAX),

		CONSTRAINT PK_TransactionError PRIMARY KEY (Id),
		CONSTRAINT FK_TranactionError FOREIGN KEY (EventId) REFERENCES ScanRecords(EventId)
	)

	GRANT INSERT, UPDATE, DELETE, SELECT ON ScanEventErrors TO public
	--Permission granting to a specific secuired UserGroup or at the moment to public
	
	PRINT '<< Created table ScanEventErrors >>'
END

--------------------------------------------
-- sy_options
--------------------------------------------
IF OBJECT_ID('dbo.sy_options') IS NOT NULL AND @allow_drop = 'Y'
BEGIN
	DROP TABLE dbo.sy_options
	PRINT '<< Dropped table sy_options >>'
END

IF OBJECT_ID('dbo.sy_options') IS NULL
BEGIN
	CREATE TABLE dbo.sy_options (
		sy_option				VARCHAR(30),
		sy_value				VARCHAR(30),
		sy_description			VARCHAR(250),

		CONSTRAINT PK_Options PRIMARY KEY (sy_option)
	)

	GRANT INSERT, UPDATE, DELETE, SELECT ON sy_options TO public
	--Permission granting to a specific secuired UserGroup or at the moment to public
	
	PRINT '<< Created table sy_options >>'
END


--------------------------------------------
-- event_tracker
--------------------------------------------
IF OBJECT_ID('dbo.event_tracker') IS NOT NULL AND @allow_drop = 'Y'
BEGIN
	DROP TABLE dbo.event_tracker
	PRINT '<< Dropped table event_tracker >>'
END

IF OBJECT_ID('dbo.event_tracker') IS NULL
BEGIN
	CREATE TABLE dbo.event_tracker (
		AppId					INT,
		LastEventId				BIGINT			DEFAULT 0,
		DateTimeLastUpdated		DATETIME		DEFAULT GETDATE(),
		Description				VARCHAR(MAX)	DEFAULT 'Initial system value'
	)

	GRANT INSERT, UPDATE, DELETE, SELECT ON event_tracker TO public
	--Permission granting to a specific secuired UserGroup or at the moment to public
	
	PRINT '<< Created table event_tracker >>'
END
-- DROP TABLE	event_tracker
-- DROP TABLE	sy_options
-- DROP TABLE	ScanType
-- DROP TABLE	ScanEventErrors
-- DROP TABLE	Device
-- DROP TABLE	ScanRecords
-- GO