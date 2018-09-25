 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[TimeTableEntrySet] DROP CONSTRAINT [FK_TimeTableEntrySet_StationSet]
GO
ALTER TABLE [dbo].[TimeTableEntrySet] DROP CONSTRAINT [FK_TimeTableEntrySet_RealtimeSet]
GO
ALTER TABLE [dbo].[TimeTableEntrySet] DROP CONSTRAINT [FK_TimeTableEntrySet_ScheduledSet]
GO
ALTER TABLE [dbo].[TimeTableEntrySet] DROP CONSTRAINT [DF_TimeTableEntrySet_IsDeleted]
GO
DROP PROCEDURE [dbo].[Station_Insert]
GO
DROP PROCEDURE [dbo].[Scheduled_Insert]
GO
DROP PROCEDURE [dbo].[TimeTableEntry_Insert]
GO
DROP PROCEDURE [dbo].[Realtime_Insert]
GO
DROP VIEW [dbo].[Realtimes]
GO
DROP VIEW [dbo].[Scheduleds]
GO
DROP VIEW [dbo].[TimeTableEntries]
GO
DROP INDEX [IX_TimeTableEntrySet] ON [dbo].[TimeTableEntrySet]
GO
DROP TABLE [dbo].[ScheduledSet]
GO
DROP TABLE [dbo].[RealtimeSet]
GO
ALTER TABLE [dbo].[TimeTableEntrySet] ALTER COLUMN [DriveId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[TimeTableEntrySet] DROP COLUMN [ItcsRef],[ItcsStationId],[ItcsLineRef],[DirectionRef],[LaneRef],[IsDeleted]
GO
ALTER TABLE [dbo].[TimeTableEntrySet] ADD 
[ItcsReference] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[LineReference] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[LaneReference] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DirectionReference] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[TimeTableEntrySet] ALTER COLUMN [VisitNumber] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
CREATE TABLE [dbo].[RealtimeDataSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ProviderId] [int] NOT NULL,
	[EstimatedArrival] [datetime] NOT NULL,
	[EstimatedDeparture] [datetime] NOT NULL,
	[RealArrival] [datetime] NOT NULL,
	[RealDeparture] [datetime] NOT NULL,
	[VehicleAtStation] [bit] NULL,
	[CleardownReference] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[RealTimeData] [bit] NOT NULL CONSTRAINT [DF_RealtimeSet_RealTimeData] DEFAULT ((1)),
	[CongestionIndicator] [bit] NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_RealtimeSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_RealtimeSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ScheduledDataSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[LineNumber] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LineText] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DirectionNumber] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[DirectionText] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LaneText] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ScheduledArrival] [datetime] NOT NULL,
	[ScheduledDeparture] [datetime] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ScheduledSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_ScheduledSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TimeTableEntrySet] ADD CONSTRAINT [FK_TimeTableEntrySet_ScheduledDataSet] FOREIGN KEY
	(
		[ScheduledDataId]
	)
	REFERENCES [dbo].[ScheduledDataSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[TimeTableEntrySet] ADD CONSTRAINT [FK_TimeTableEntrySet_RealtimeDataSet] FOREIGN KEY
	(
		[RealtimeDataId]
	)
	REFERENCES [dbo].[RealtimeDataSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[RealtimeDataSet] ADD CONSTRAINT [FK_RealtimeDataSet_ItcsProviders] FOREIGN KEY
	(
		[ProviderId]
	)
	REFERENCES [dbo].[ItcsProviders]
	(
		[Id]
	)
GO

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.15.41'
  ,@description = 'Edited views to support activity state tracking.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 15
  ,@versionRevision = 41
  ,@dateCreated = '2012-06-29T08:41:00.000'
GO