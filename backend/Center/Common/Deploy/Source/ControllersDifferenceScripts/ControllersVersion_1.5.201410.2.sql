USE [Gorba_CenterControllers]
GO

ALTER DATABASE [Gorba_CenterControllers] SET allow_snapshot_isolation ON
GO
ALTER DATABASE [Gorba_CenterControllers] SET READ_COMMITTED_SNAPSHOT ON;
GO




--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterControllers].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.5.201410.2'
  ,@description = 'Updated snapshot isolation settings.'
  ,@versionMajor = 1
  ,@versionMinor = 5
  ,@versionBuild = 201410
  ,@versionRevision = 2
  ,@dateCreated = '2014-05-19T09:00:00.000'
GO