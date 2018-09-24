/*
 * LEF 15.05.14
 * WARNING!!! always backup the database before executing the script
 * Changing Isolation options for databases.
 * Added stop information to ActivityInstance.
 * WARNING!!! Be sure that there are no active connections. You can restart the Sql server to kill all active connections.
 * 
*/

USE [Gorba_CenterOnline]


/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE [Gorba_CenterOnline].[dbo].[ActivityInstanceSet] ADD
	DateRevokeRequested datetime2(7) NULL,
	IsRevokeRequested  AS (CONVERT([bit],case when DateRevokeRequested IS NULL then (0) else (1) end,0)) PERSISTED ,
	DateCompleted datetime2(7) NULL,
	IsCompleted  AS (CONVERT([bit],case when DateCompleted IS NULL then (0) else (1) end,0)) PERSISTED
GO
ALTER TABLE dbo.ActivityInstanceSet SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

GO

ALTER DATABASE [Gorba_CenterOnline] SET allow_snapshot_isolation ON
GO
ALTER DATABASE [Gorba_CenterOnline] SET READ_COMMITTED_SNAPSHOT ON;
GO

	
	--=============================================================
	-- Versioning
	--=============================================================
	DECLARE @RC int

	EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
	   @name = 'Version 1.5.201410.69'
	  ,@description = 'Added information about revoking and completing activity instances.'
	  ,@versionMajor = 1
	  ,@versionMinor = 5
	  ,@versionBuild = 201410
	  ,@versionRevision = 69
	  ,@dateCreated = '2014-05-19T09:00:00.000'
GO

