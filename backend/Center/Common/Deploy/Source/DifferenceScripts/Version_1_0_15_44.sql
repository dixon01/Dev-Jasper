 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ItcsProviders] ALTER COLUMN [Properties] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.15.44'
  ,@description = 'ItcsProviders.Properties changes from xml to varchar.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 15
  ,@versionRevision = 44
  ,@dateCreated = '2012-07-10T08:47:00.000'
GO
