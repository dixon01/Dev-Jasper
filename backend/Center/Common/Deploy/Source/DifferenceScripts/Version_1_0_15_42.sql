 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[TimeTableEntrySet] DROP CONSTRAINT [FK_TimeTableEntrySet_RealtimeDataSet]
GO
ALTER TABLE [dbo].[TimeTableEntrySet] DROP CONSTRAINT [FK_TimeTableEntrySet_ScheduledDataSet]
GO
ALTER TABLE [dbo].[TimeTableEntrySet] DROP CONSTRAINT [FK_TimeTableEntrySet_UnitSet]
GO
ALTER TABLE [dbo].[RealtimeDataSet] DROP CONSTRAINT [FK_RealtimeDataSet_ItcsProviders]
GO
DROP TABLE [dbo].[TimeTableEntrySet]
GO
DROP TABLE [dbo].[RealtimeDataSet]
GO
DROP TABLE [dbo].[ScheduledDataSet]
GO
CREATE TABLE [dbo].[VdvMessageSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UnitId] [int] NOT NULL,
	[ItcsIdentifier] [int] NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LineTextId] [int] NOT NULL,
	[PlatformTextId] [int] NOT NULL,
	[Destination1TextId] [int] NOT NULL,
	[Destination2TextId] [int] NOT NULL,
	[ScheduledArrivalTime] [int] NOT NULL,
	[ScheduledDepartureTime] [int] NOT NULL,
	[EstimatedArrivalTime] [int] NULL,
	[EstimatedDepartureTime] [int] NULL,
	[TripId] [int] NOT NULL,
	[CleardownReference] [int] NULL,
	[ContainsRealtime] [bit] NOT NULL,
	[IsAtStation] [bit] NOT NULL,
	[TrafficJamIndicator] [int] NULL,
	[RblLine] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RblDirection] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[IsRealTimeMessage] [bit] NOT NULL,
	[ProviderId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_VdvMessageSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_VdvMessageSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[VdvMessages]
AS
SELECT     Id, ItcsIdentifier, Description, LineTextId, PlatformTextId, Destination1TextId, Destination2TextId, ScheduledArrivalTime, ScheduledDepartureTime, 
                      EstimatedArrivalTime, EstimatedDepartureTime, TripId, CleardownReference, ContainsRealtime, IsAtStation, TrafficJamIndicator, IsRealTimeMessage, ProviderId, 
                      DateCreated, DateModified, RblLine, RblDirection, UnitId
FROM         dbo.VdvMessageSet
WHERE     (IsDeleted = 0)
GO
ALTER TABLE [dbo].[VdvMessageSet] ADD CONSTRAINT [FK_VdvMessageSet_ItcsProviders] FOREIGN KEY
	(
		[ProviderId]
	)
	REFERENCES [dbo].[ItcsProviders]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[VdvMessageSet] ADD CONSTRAINT [FK_VdvMessageSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[VdvMessageSet] ADD CONSTRAINT [FK_VdvMessageSet_VdvMessageSet] FOREIGN KEY
	(
		[Id]
	)
	REFERENCES [dbo].[VdvMessageSet]
	(
		[Id]
	)
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.15.42'
  ,@description = 'Add VdvMessageSet and view. Delete tables TimeTableEntrySet, RealtimeDataSet and ScheduledDataSet.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 15
  ,@versionRevision = 42
  ,@dateCreated = '2012-07-04T16:47:00.000'
GO