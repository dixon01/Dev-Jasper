USE [Gorba_CenterOnline]
GO

/*
 * It is possible to delete both ItcsRealtimeDataSet and ItcsTimeTableEntrySet during maintenance.
 * For this reason, there won't be any data migration.
*/


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ItcsRealtimeDataSet_ItcsTimeTableEntrySet]') AND parent_object_id = OBJECT_ID(N'[dbo].[ItcsRealtimeDataSet]'))
ALTER TABLE [dbo].[ItcsRealtimeDataSet] DROP CONSTRAINT [FK_ItcsRealtimeDataSet_ItcsTimeTableEntrySet]
GO

USE [Gorba_CenterOnline]
GO

/****** Object:  Table [dbo].[ItcsRealtimeDataSet]    Script Date: 03/15/2013 11:21:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItcsRealtimeDataSet]') AND type in (N'U'))
DROP TABLE [dbo].[ItcsRealtimeDataSet]
GO

USE [Gorba_CenterOnline]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ItcsTimeTableEntrySet_ItcsDisplayAreas]') AND parent_object_id = OBJECT_ID(N'[dbo].[ItcsTimeTableEntrySet]'))
ALTER TABLE [dbo].[ItcsTimeTableEntrySet] DROP CONSTRAINT [FK_ItcsTimeTableEntrySet_ItcsDisplayAreas]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ItcsTimeTableEntrySet_IsDeleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ItcsTimeTableEntrySet] DROP CONSTRAINT [DF_ItcsTimeTableEntrySet_IsDeleted]
END

GO

USE [Gorba_CenterOnline]
GO

/****** Object:  Table [dbo].[ItcsTimeTableEntrySet]    Script Date: 03/15/2013 11:23:05 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItcsTimeTableEntrySet]') AND type in (N'U'))
DROP TABLE [dbo].[ItcsTimeTableEntrySet]
GO



USE [Gorba_CenterOnline]
GO

/****** Object:  Table [dbo].[ItcsTimeTableEntrySet]    Script Date: 03/15/2013 11:26:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ItcsTimeTableEntrySet](
	[Id] [uniqueidentifier] NOT NULL,
	[ItcsDisplayAreaId] [int] NOT NULL,
	[OperationalDay] [datetime2](7) NOT NULL,
	[LineId] [varchar](160) NULL,
	[DirectionId] [varchar](160) NULL,
	[TripId] [varchar](160) NULL,
	[StopSequenceCounter] [int] NOT NULL,
	[LastUpdated] [datetime2](7) NOT NULL,
	[ValidUntil] [datetime2](7) NOT NULL,
	[LineText] [varchar](160) NULL,
	[DirectionTexts] [varchar](160) NULL,
	[ScheduledArrival] [datetime2](7) NULL,
	[ScheduledDeparture] [datetime2](7) NULL,
	[TimeStamp] [datetime2](7) NOT NULL,
	[TripVehicleId] [varchar](160) NULL,
	[TripLineNumber] [int] NULL,
	[TripBlockNumber] [int] NULL,
	[TripRunNumber] [int] NULL,
	[TripDepartureStopLong] [varchar](160) NULL,
	[TripDestinationStopLong] [varchar](160) NULL,
	[TripDepartureStop] [varchar](160) NULL,
	[TripDestinationStop] [varchar](160) NULL,
	[TripDepartureTimeStartStop] [datetime2](7) NULL,
	[TripArrivalTimeDestinationStop] [datetime2](7) NULL,
	[TripOperator] [varchar](160) NULL,
	[TripProductId] [varchar](160) NULL,
	[TripServiceAttributes] [varchar](max) NULL,
	[IsTripDeleted] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ItcsTimeTableEntrySet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ItcsTimeTableEntrySet]  WITH CHECK ADD  CONSTRAINT [FK_ItcsTimeTableEntrySet_ItcsDisplayAreas] FOREIGN KEY([ItcsDisplayAreaId])
REFERENCES [dbo].[ItcsDisplayAreaSet] ([Id])
GO

ALTER TABLE [dbo].[ItcsTimeTableEntrySet] CHECK CONSTRAINT [FK_ItcsTimeTableEntrySet_ItcsDisplayAreas]
GO

ALTER TABLE [dbo].[ItcsTimeTableEntrySet] ADD  CONSTRAINT [DF_ItcsTimeTableEntrySet_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

USE [Gorba_CenterOnline]
GO

/****** Object:  Table [dbo].[ItcsRealtimeDataSet]    Script Date: 03/15/2013 11:27:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ItcsRealtimeDataSet](
	[ItcsTimeTableEntryId] [uniqueidentifier] NOT NULL,
	[EstimatedArrival] [datetime2](7) NULL,
	[EstimatedDeparture] [datetime2](7) NULL,
	[RealArrival] [datetime2](7) NULL,
	[RealDeparture] [datetime2](7) NULL,
	[IsAtStation] [bit] NOT NULL,
	[DepartureNoticeId] [varchar](160) NULL,
	[ContainsRealtime] [bit] NOT NULL,
	[QueueIndicator] [bit] NOT NULL,
	[TrainsetId] [varchar](160) NULL,
	[TrainsetPosition] [int] NULL,
	[TrainsetNumOfTrips] [int] NULL,
	[HasDisruption] [bit] NOT NULL,
	[DestinationStop] [varchar](160) NOT NULL,
	[TripSpecialText] [varchar](160) NULL,
	[SpeechOutputs] [varchar](max) NULL,
	[StopId] [varchar](160) NULL,
	[StopPositionText] [varchar](160) NULL,
	[VehicleNumbers] [varchar](max) NULL,
 CONSTRAINT [PK_ItcsRealtimeDataSet] PRIMARY KEY CLUSTERED 
(
	[ItcsTimeTableEntryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ItcsRealtimeDataSet]  WITH CHECK ADD  CONSTRAINT [FK_ItcsRealtimeDataSet_ItcsTimeTableEntrySet] FOREIGN KEY([ItcsTimeTableEntryId])
REFERENCES [dbo].[ItcsTimeTableEntrySet] ([Id])
GO

ALTER TABLE [dbo].[ItcsRealtimeDataSet] CHECK CONSTRAINT [FK_ItcsRealtimeDataSet_ItcsTimeTableEntrySet]
GO





--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.1.32.61'
  ,@description = 'Changing identifiers for ItcsTimeTableEntrySet and ItcsRealtimeDataSet from int to uniqueidentifier to support bulk inserts.'
  ,@versionMajor = 1
  ,@versionMinor = 1
  ,@versionBuild = 32
  ,@versionRevision = 61
  ,@dateCreated = '2013-03-15T11:30:00.000'
GO