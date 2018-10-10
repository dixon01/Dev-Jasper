 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ItcsRealtimeDataSet] DROP CONSTRAINT [FK_ItcsRealtimeDataSet_ItcsTimeTableEntrySet]
GO
ALTER TABLE [dbo].[ItcsTimeTableEntrySet] DROP CONSTRAINT [FK_ItcsTimeTableEntrySet_ItcsDisplayAreas]
GO
ALTER TABLE [dbo].[ItcsRealtimeDataSet] DROP CONSTRAINT [PK_ItcsRealtimeDataSet]
GO
ALTER TABLE [dbo].[ItcsTimeTableEntrySet] DROP CONSTRAINT [PK_ItcsTimeTableEntrySet]
GO
UPDATE [dbo].[ItcsProviders] SET [ReferenceId] = 0 WHERE [ReferenceId] IS NULL
GO
ALTER TABLE [dbo].[ItcsProviders] ALTER COLUMN [ReferenceId] [int] NOT NULL
GO
CREATE TABLE [dbo].[TempItcsRealtimeDataSet]
(
	[ItcsTimeTableEntryId] [int] NOT NULL,
	[EstimatedArrival] [datetime2](7) NOT NULL,
	[EstimatedDeparture] [datetime2](7) NOT NULL,
	[RealArrival] [datetime2](7) NOT NULL,
	[RealDeparture] [datetime2](7) NOT NULL,
	[IsAtStation] [bit] NOT NULL,
	[CleardownReference] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ContainsRealtime] [bit] NOT NULL,
	[InCongestion] [bit] NOT NULL,
	[PlatformText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PlatformId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[HasDisruption] [bit] NOT NULL,

) ON [PRIMARY]
GO

INSERT INTO [dbo].[TempItcsRealtimeDataSet] ([ItcsTimeTableEntryId],[EstimatedArrival],[EstimatedDeparture],[RealArrival],[RealDeparture],[IsAtStation],[CleardownReference],[ContainsRealtime],[InCongestion],[PlatformText],[PlatformId],[HasDisruption]) SELECT [ItcsTimeTableEntryId],[EstimatedArrival],[EstimatedDeparture],[RealArrival],[RealDeparture],[IsAtStation],[CleardownReference],[ContainsRealtime],[InCongestion],[PlatformText],[PlatformId],0 FROM [dbo].[ItcsRealtimeDataSet]
DROP TABLE [dbo].[ItcsRealtimeDataSet]
GO
EXEC sp_rename N'[dbo].[TempItcsRealtimeDataSet]',N'ItcsRealtimeDataSet', 'OBJECT'
GO


CREATE TABLE [dbo].[TempItcsTimeTableEntrySet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ItcsDisplayAreaId] [int] NOT NULL,
	[OperationalDay] [datetime2](7) NOT NULL,
	[LineId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DirectionId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TripId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[StopSequenceCounter] [int] NOT NULL,
	[LastUpdated] [datetime2](7) NOT NULL,
	[ValidUntil] [datetime2](7) NOT NULL,
	[LineText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DirectionText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ScheduledArrival] [datetime2](7) NULL,
	[ScheduledDeparture] [datetime2](7) NULL,
	[IsTripDeleted] [bit] NOT NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempItcsTimeTableEntrySet] ON
INSERT INTO [dbo].[TempItcsTimeTableEntrySet] ([Id],[ItcsDisplayAreaId],[OperationalDay],[LineId],[DirectionId],[TripId],[StopSequenceCounter],[LastUpdated],[ValidUntil],[LineText],[DirectionText],[ScheduledArrival],[ScheduledDeparture],[IsTripDeleted]) SELECT [Id],[ItcsDisplayAreaId],[OperationalDay],[LineId],[DirectionId],[TripId],[StopSequenceCounter],[LastUpdated],[ValidUntil],[LineText],[DirectionText],[ScheduledArrival],[ScheduledDeparture],0 FROM [dbo].[ItcsTimeTableEntrySet]
SET IDENTITY_INSERT [dbo].[TempItcsTimeTableEntrySet] OFF
GO

DROP TABLE [dbo].[ItcsTimeTableEntrySet]
GO
EXEC sp_rename N'[dbo].[TempItcsTimeTableEntrySet]',N'ItcsTimeTableEntrySet', 'OBJECT'
GO


ALTER VIEW [dbo].[ItcsRealtimeData]
AS
SELECT     ItcsTimeTableEntryId, EstimatedArrival, EstimatedDeparture, RealArrival, RealDeparture, IsAtStation, CleardownReference, ContainsRealtime, InCongestion, 
                      PlatformText, PlatformId, HasDisruption
FROM         dbo.ItcsRealtimeDataSet
GO

ALTER VIEW [dbo].[ItcsTimeTableEntries]
AS
SELECT     Id, ItcsDisplayAreaId, OperationalDay, LineId, DirectionId, TripId, StopSequenceCounter, LastUpdated, ValidUntil, LineText, DirectionText, ScheduledArrival, 
                      ScheduledDeparture, IsTripDeleted
FROM         dbo.ItcsTimeTableEntrySet
GO
ALTER TABLE [dbo].[ItcsRealtimeDataSet] ADD CONSTRAINT [PK_ItcsRealtimeDataSet] PRIMARY KEY CLUSTERED
	(
		[ItcsTimeTableEntryId] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ItcsTimeTableEntrySet] ADD CONSTRAINT [PK_ItcsTimeTableEntrySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ItcsRealtimeDataSet] ADD CONSTRAINT [FK_ItcsRealtimeDataSet_ItcsTimeTableEntrySet] FOREIGN KEY
	(
		[ItcsTimeTableEntryId]
	)
	REFERENCES [dbo].[ItcsTimeTableEntrySet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ItcsTimeTableEntrySet] ADD CONSTRAINT [FK_ItcsTimeTableEntrySet_ItcsDisplayAreas] FOREIGN KEY
	(
		[ItcsDisplayAreaId]
	)
	REFERENCES [dbo].[ItcsDisplayAreas]
	(
		[Id]
	)
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.23.56'
  ,@description = 'Added flag HasDisruption to ItcsRealtimeDataSet. Change ReferenceId of ItcsProviders from varchar to int. Remove IsDeleted from ItcsRealtimeDataSet and add IsTripDeleted to ItcsTimeTableEntrySet. Update related views.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 23
  ,@versionRevision = 56
  ,@dateCreated = '2012-10-29T11:30:00.000'
GO