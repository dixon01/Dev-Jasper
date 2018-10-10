/*
 * LEF 15.05.14
 * WARNING!!! always backup the database before executing the script
 * Adding persistent information for current scheduling in ActivityInstance.
 * Added start information to ActivityInstance.
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
	DateStarted datetime2(7) NULL,
	IsStarted  AS (CONVERT([bit],case when [DateStarted] IS NULL then (0) else (1) end,0)) PERSISTED ,
	CurrentSchedulingStart datetime2(7) NULL,
	CurrentSchedulingEnd datetime2(7) NULL,
	CurrentSchedulingTransmission datetime2(7) NULL
GO
ALTER TABLE dbo.ActivityInstanceSet SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
	
	--=============================================================
	-- Versioning
	--=============================================================
	DECLARE @RC int

	EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
	   @name = 'Version 1.5.201410.68'
	  ,@description = 'Added Start and Scheduling information to ActivityInstance.'
	  ,@versionMajor = 1
	  ,@versionMinor = 5	
	  ,@versionBuild = 201410
	  ,@versionRevision = 68
	  ,@dateCreated = '2014-05-15T09:00:00.000'
GO

