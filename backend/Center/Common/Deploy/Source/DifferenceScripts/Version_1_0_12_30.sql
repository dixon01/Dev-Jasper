 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ItcsFilters] DROP CONSTRAINT [FK_ItcsFilters_ItcsConfigurations]
GO
ALTER TABLE [dbo].[ItcsFilters] DROP CONSTRAINT [FK_ItcsFilters_DirectionReferences]
GO
ALTER TABLE [dbo].[UnitSet] ADD 
[GatewayAddress] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[ItcsFilters] DROP COLUMN [ItcsConfigurationId],[LineName],[DirectionId]
GO

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.12.30'
  ,@description = 'Minor changes.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 12
  ,@versionRevision = 30
  ,@dateCreated = '2012-05-21T07:10:00.000'
GO