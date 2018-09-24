 

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
CREATE TABLE [dbo].[ItcsTextMappingSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ItcsProviderId] [int] NOT NULL,
	[ProductTypeId] [int] NULL,
	[Type] [smallint] NOT NULL,
	[SourceText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[MappedText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TtsText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LastUsed] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ItcsTextMappingSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_ItcsTextMappingSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ItcsLineSpecialTextSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ItcsDisplayAreaId] [int] NOT NULL,
	[ValidUntil] [datetime2](7) NOT NULL,
	[LineId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[DirectionId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LineSpecialText] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Priority] [smallint] NULL,
	[SpeechOutput] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ItcsLineSpecialTextSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_ItcsLineSpecialTextSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TempItcsRealtimeDataSet]
(
	[ItcsTimeTableEntryId] [int] NOT NULL,
	[EstimatedArrival] [datetime2](7) NULL,
	[EstimatedDeparture] [datetime2](7) NULL,
	[RealArrival] [datetime2](7) NULL,
	[RealDeparture] [datetime2](7) NULL,
	[IsAtStation] [bit] NOT NULL,
	[DepartureNoticeId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ContainsRealtime] [bit] NOT NULL,
	[QueueIndicator] [bit] NOT NULL,
	[TrainsetId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TrainsetPosition] [int] NULL,
	[TrainsetNumOfTrips] [int] NULL,
	[HasDisruption] [bit] NOT NULL,
	[DestinationStop] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[TripSpecialText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SpeechOutputs] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[StopId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[StopPositionText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VehicleNumbers] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

) ON [PRIMARY]
GO

INSERT INTO [dbo].[TempItcsRealtimeDataSet] ([ItcsTimeTableEntryId],[EstimatedArrival],[EstimatedDeparture],[RealArrival],[RealDeparture],[IsAtStation],[ContainsRealtime],[HasDisruption],[QueueIndicator],[DepartureNoticeId],[DestinationStop], [StopId], [StopPositionText]) SELECT [ItcsTimeTableEntryId],[EstimatedArrival],[EstimatedDeparture],[RealArrival],[RealDeparture],[IsAtStation],[ContainsRealtime],[HasDisruption],[InCongestion],[CleardownReference],'',[PlatformId],[PlatformText] FROM [dbo].[ItcsRealtimeDataSet]
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
	[DirectionTexts] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ScheduledArrival] [datetime2](7) NULL,
	[ScheduledDeparture] [datetime2](7) NULL,
	[TimeStamp] [datetime2](7) NOT NULL,
	[TripVehicleId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TripLineNumber] [int] NULL,
	[TripBlockNumber] [int] NULL,
	[TripRunNumber] [int] NULL,
	[TripDepartureStopLong] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TripDestinationStopLong] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TripDepartureStop] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TripDestinationStop] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TripDepartureTimeStartStop] [datetime2](7) NULL,
	[TripArrivalTimeDestinationStop] [datetime2](7) NULL,
	[TripOperator] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TripProductId] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TripServiceAttributes] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsTripDeleted] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ItcsTimeTableEntrySet_IsDeleted] DEFAULT ((0))

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempItcsTimeTableEntrySet] ON
INSERT INTO [dbo].[TempItcsTimeTableEntrySet] ([Id],[ItcsDisplayAreaId],[OperationalDay],[LineId],[DirectionId],[TripId],[StopSequenceCounter],[LastUpdated],[ValidUntil],[LineText],[DirectionTexts],[ScheduledArrival],[ScheduledDeparture],[IsTripDeleted],[TimeStamp],[IsDeleted]) SELECT [Id],[ItcsDisplayAreaId],[OperationalDay],[LineId],[DirectionId],[TripId],[StopSequenceCounter],[LastUpdated],[ValidUntil],[LineText],[DirectionTexts],[ScheduledArrival],[ScheduledDeparture],[IsTripDeleted],getdate(),((0)) FROM [dbo].[ItcsTimeTableEntrySet]
SET IDENTITY_INSERT [dbo].[TempItcsTimeTableEntrySet] OFF
GO

DROP TABLE [dbo].[ItcsTimeTableEntrySet]
GO
EXEC sp_rename N'[dbo].[TempItcsTimeTableEntrySet]',N'ItcsTimeTableEntrySet', 'OBJECT'
GO

ALTER VIEW [dbo].[ItcsTimeTableEntries]
AS
SELECT     Id, ItcsDisplayAreaId, OperationalDay, LineId, DirectionId, TripId, StopSequenceCounter, LastUpdated, ValidUntil, LineText, DirectionTexts, ScheduledArrival, 
                      ScheduledDeparture, IsTripDeleted, TimeStamp, TripVehicleId, TripLineNumber, TripBlockNumber, TripRunNumber, TripDepartureStopLong, TripDestinationStopLong, 
                      TripDepartureStop, TripDestinationStop, TripDepartureTimeStartStop, TripArrivalTimeDestinationStop, TripOperator, TripProductId, TripServiceAttributes
FROM         dbo.ItcsTimeTableEntrySet
WHERE     (IsDeleted = 0)
GO

ALTER VIEW [dbo].[ItcsRealtimeData]
AS
SELECT     ItcsTimeTableEntryId, EstimatedArrival, EstimatedDeparture, RealArrival, RealDeparture, IsAtStation, ContainsRealtime, HasDisruption, DepartureNoticeId, 
                      QueueIndicator, TrainsetId, TrainsetPosition, TrainsetNumOfTrips, DestinationStop, TripSpecialText, SpeechOutputs, StopId, StopPositionText, VehicleNumbers
FROM         dbo.ItcsRealtimeDataSet
GO
CREATE VIEW [dbo].[ItcsTextMappings]
AS
SELECT     Id, ItcsProviderId, ProductTypeId, Type, SourceText, MappedText, TtsText, LastUsed
FROM         dbo.ItcsTextMappingSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[ItcsLineSpecialTexts]
AS
SELECT     Id, ItcsDisplayAreaId, ValidUntil, LineId, DirectionId, LineSpecialText, Priority, SpeechOutput
FROM         dbo.ItcsLineSpecialTextSet
WHERE     (IsDeleted = 0)
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
ALTER TABLE [dbo].[ItcsLineSpecialTextSet] ADD CONSTRAINT [FK_ItcsLineSpecialTextSet_ItcsDisplayAreas] FOREIGN KEY
	(
		[ItcsDisplayAreaId]
	)
	REFERENCES [dbo].[ItcsDisplayAreas]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ItcsTextMappingSet] ADD CONSTRAINT [FK_ItcsTextMappingSet_ItcsProviders] FOREIGN KEY
	(
		[ItcsProviderId]
	)
	REFERENCES [dbo].[ItcsProviders]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ItcsTextMappingSet] ADD CONSTRAINT [FK_ItcsTextMappingSet_ProductTypeSet] FOREIGN KEY
	(
		[ProductTypeId]
	)
	REFERENCES [dbo].[ProductTypeSet]
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
   @name = 'Version 1.1.29.59'
  ,@description = 'Rename columns in ItcsRealtimeSet to match Vdv spec. Add missing fields to ItcsTimeTableEntrySet and ItcsRealtimeSet and update the views. Add tables ItcsTextMappingSet, ItcsLineSpecialTextSet and views.'
  ,@versionMajor = 1
  ,@versionMinor = 1
  ,@versionBuild = 29
  ,@versionRevision = 59
  ,@dateCreated = '2013-01-28T11:30:00.000'
GO
