USE [Gorba_CenterControllersMetabase]
GO

ALTER DATABASE [Gorba_CenterControllersMetabase] SET allow_snapshot_isolation ON
GO
ALTER DATABASE [Gorba_CenterControllersMetabase] SET READ_COMMITTED_SNAPSHOT ON;
GO


--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterControllersMetabase].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.5.201410.3'
  ,@description = 'Updated snapshot isolation settings.'
  ,@versionMajor = 1
  ,@versionMinor = 5
  ,@versionBuild = 201410
  ,@versionRevision = 3
  ,@dateCreated = '2014-05-19T09:00:00.000'
GO