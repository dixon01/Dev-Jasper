 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[AssociationUnitLineSet] DROP CONSTRAINT [FK_AssociationUnitLineSet_UnitSet]
GO
ALTER TABLE [dbo].[ItcsFilters] DROP CONSTRAINT [FK_ItcsFilters_LineReferences]
GO
ALTER TABLE [dbo].[AssociationUnitLineSet] DROP CONSTRAINT [FK_AssociationUnitLineSet_LineSet]
GO
ALTER TABLE [dbo].[LineReferences] DROP CONSTRAINT [FK_LineReferences_ItcsProviders]
GO
ALTER TABLE [dbo].[LineReferences] DROP CONSTRAINT [FK_LineReferences_LineSet]
GO
DROP VIEW [dbo].[AssociationsUnitLine]
GO
DROP VIEW [dbo].[Lines]
GO
DROP TABLE [dbo].[LineSet]
GO
DROP TABLE [dbo].[AssociationUnitLineSet]
GO
DROP TABLE [dbo].[LineReferences]
GO
ALTER TABLE [dbo].[ItcsFilters] DROP COLUMN [LineReferenceName],[Direction],[DirectionReferenceName],[LineId]
GO
ALTER TABLE [dbo].[ItcsFilters] ADD 
[LineReference] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DirectionText] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DirectionReference] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[LineText] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[LayoutSet] ALTER COLUMN [Definition] [xml] NULL
GO
ALTER VIEW [dbo].[Activities]
AS
SELECT [Id]
      ,[OperationId]
      ,[DateCreated]
      ,[DateModified]
      ,[IsDeleted]
      ,[RealTaskId]
      ,[LastRealTaskCreationDateTime]
      ,[CurrentState]
      ,[ErrorActivityInstancesCount]
      ,[RevokingActivityInstancesCount]
      ,[ActiveActivityInstancesCount]
      ,[TransmittingActivityInstancesCount]
      ,[TransmittedActivityInstancesCount]
      ,[ScheduledActivityInstancesCount]
      ,[EndedActivityInstancesCount]
      ,[RevokedActivityInstancesCount]
      ,[CreatedActivityInstancesCount]
      , [SchedulingActivityInstancesCount]
      , [TotalActivityInstancesCount]
  FROM [Gorba_CenterOnline].[dbo].[ActivitySet]
WHERE     (IsDeleted = 0)
GO
ALTER TABLE [dbo].[LayoutSet] ADD CONSTRAINT [DF_LayoutSet_DateCreated] DEFAULT (getutcdate()) FOR [DateCreated]
GO




--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.17.48'
  ,@description = 'Activity status management changes.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 17
  ,@versionRevision = 48
  ,@dateCreated = '2012-08-06T08:47:00.000'
GO

