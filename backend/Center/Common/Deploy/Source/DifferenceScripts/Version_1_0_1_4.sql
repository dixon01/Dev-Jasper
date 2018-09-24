 

USE Gorba_CenterOnline
GO

CREATE TABLE [dbo].[InfoLineTextSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ItcsStationId] [int] NOT NULL,
	[LineNumber] [int] NOT NULL,
	[DestinationId] [int] NOT NULL,
	[DisplayText] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExpirationTime] [datetime] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_InfoLineTextSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_InfoLineTextSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TimeTableEntrySet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UnitId] [int] NOT NULL,
	[ScheduledDataId] [int] NOT NULL,
	[RealtimeDataId] [int] NOT NULL,
	[ItcsRef] [int] NOT NULL,
	[ItcsStationId] [int] NOT NULL,
	[ItcsLineRef] [int] NULL,
	[DirectionRef] [int] NULL,
	[LaneRef] [int] NULL,
	[TripInfo] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ValidUntil] [datetime] NULL,
	[DriveId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[VisitNumber] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[OperationalDay] [datetime] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_TimeTableEntrySet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_TimeTableEntrySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TimeTableEntrySet] ON [dbo].[TimeTableEntrySet]
(
	[DriveId] ASC,
	[VisitNumber] ASC,
	[OperationalDay] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[RealtimeSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[EstimatedArrival] [datetime] NOT NULL,
	[EstimatedDeparture] [datetime] NOT NULL,
	[RealArrival] [datetime] NOT NULL,
	[RealDeparture] [datetime] NOT NULL,
	[VehicleAtStation] [bit] NULL,
	[CleardownRef] [int] NULL,
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

CREATE TABLE [dbo].[StationSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Number] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[UtcOffset] [datetime] NOT NULL,
	[DayLightSaving] [bit] NOT NULL CONSTRAINT [DF_StationSet_DayLightSaving] DEFAULT ((0)),
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_StationSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_StationSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ScheduledSet]
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

CREATE VIEW [dbo].[Realtimes]
AS
SELECT     Id, EstimatedArrival, EstimatedDeparture, RealArrival, RealDeparture, VehicleAtStation, CleardownRef, RealTimeData, CongestionIndicator, DateCreated, 
                      DateModified
FROM         dbo.RealtimeSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[InfoLineTexts]
AS
SELECT     Id, ItcsStationId, LineNumber, DestinationId, DisplayText, ExpirationTime, DateCreated, DateModified
FROM         dbo.InfoLineTextSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[Stations]
AS
SELECT     Id, Name, Number, Latitude, Longitude, UtcOffset, DayLightSaving, DateCreated, DateModified
FROM         dbo.StationSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[Scheduleds]
AS
SELECT     Id, LineNumber, LineText, DirectionNumber, DirectionText, LaneText, ScheduledArrival, ScheduledDeparture, DateCreated, DateModified
FROM         dbo.ScheduledSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[TimeTableEntries]
AS
SELECT     Id, UnitId, ScheduledDataId, RealtimeDataId, ItcsRef, ItcsStationId, ItcsLineRef, DirectionRef, LaneRef, TripInfo, ValidUntil, DriveId, VisitNumber, OperationalDay, 
                      DateCreated, DateModified
FROM         dbo.TimeTableEntrySet
WHERE     (IsDeleted = 0)
GO
ALTER TABLE [dbo].[TimeTableEntrySet] ADD CONSTRAINT [FK_TimeTableEntrySet_RealtimeSet] FOREIGN KEY
	(
		[RealtimeDataId]
	)
	REFERENCES [dbo].[RealtimeSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[TimeTableEntrySet] ADD CONSTRAINT [FK_TimeTableEntrySet_ScheduledSet] FOREIGN KEY
	(
		[ScheduledDataId]
	)
	REFERENCES [dbo].[ScheduledSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[TimeTableEntrySet] ADD CONSTRAINT [FK_TimeTableEntrySet_StationSet] FOREIGN KEY
	(
		[ItcsStationId]
	)
	REFERENCES [dbo].[StationSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[TimeTableEntrySet] ADD CONSTRAINT [FK_TimeTableEntrySet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[InfoLineTextSet] ADD CONSTRAINT [FK_InfoLineTextSet_StationSet] FOREIGN KEY
	(
		[ItcsStationId]
	)
	REFERENCES [dbo].[StationSet]
	(
		[Id]
	)
GO

-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 7.12.11 10:10
-- Description:	Adds a TimeTableEntry to the system
-- =============================================
CREATE PROCEDURE [dbo].[TimeTableEntry_Insert] 
	(
	@unitId int,
	@scheduledDataId int,
	@realtimeDataId int,
	@itcsRef int,
	@itcsStationId int,
	@itcsLineRef int,
	@directionRef int = NULL,
	@laneRef int = NULL,
	@tripInfo varchar(100) = NULL,
	@validUntil datetime = NULL,
	@driveId varchar(100),
	@visitNumber varchar(100),
	@operationalDay datetime,
	@dateCreated datetime = NULL
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION TimeTableEntry_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [TimeTableEntrySet]
			([UnitId]
			, [ScheduledDataId]
			, [RealtimeDataId]
			, [ItcsRef]
			, [ItcsStationId]
			, [ItcsLineRef]
			, [DirectionRef]
			, [LaneRef]
			, [TripInfo]
			, [ValidUntil]
			, [DriveId]
			, [VisitNumber]
			, [OperationalDay]
			, [DateCreated]
			)
			VALUES
			(@unitId ,
	@scheduledDataId ,
	@realtimeDataId ,
	@itcsRef ,
	@itcsStationId ,
	@itcsLineRef ,
	@directionRef ,
	@laneRef ,
	@tripInfo ,
	@validUntil ,
	@driveId ,
	@visitNumber ,
	@operationalDay ,
	@dateCreated )
			
			DECLARE @id int = SCOPE_IDENTITY()
			SELECT @id
			
			COMMIT TRAN -- Transaction Success!

	END TRY

	BEGIN CATCH


		IF @@TRANCOUNT > 0
			BEGIN
				ROLLBACK TRAN --RollBack in case of Error
			END
			
			DECLARE @message varchar(500) = ERROR_MESSAGE()
			DECLARE @severity int = ERROR_SEVERITY()
			RAISERROR(@message, @severity, 1)

	END CATCH
	END
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 7.12.11 08:55
-- Description:	Adds a realtime entry to the system
-- =============================================
CREATE PROCEDURE [dbo].[Realtime_Insert] 
	(@estimatedArrival datetime,
	@estimatedDeparture datetime,
	@realArrival datetime,
	@realDeparture datetime,
	@vehicleAtStation bit = 0,
	@cleardownRef int,
	@realTimeData bit = 0,
	@congestionIndicator bit = 0,
	@dateCreated datetime = NULL
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Realtime_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [RealtimeSet]
			([EstimatedArrival]
			, [EstimatedDeparture]
			, [RealArrival]
			, [RealDeparture]
			, [VehicleAtStation]
			, [CleardownRef]
			, [RealTimeData]
			, [CongestionIndicator]
			, [DateCreated]
			)
			VALUES
			(@estimatedArrival ,
	@estimatedDeparture ,
	@realArrival ,
	@realDeparture ,
	@vehicleAtStation ,
	@cleardownRef ,
	@realTimeData ,
	@congestionIndicator ,
	@dateCreated )
			
			DECLARE @id int = SCOPE_IDENTITY()
			SELECT @id
			
			COMMIT TRAN -- Transaction Success!

	END TRY

	BEGIN CATCH


		IF @@TRANCOUNT > 0
			BEGIN
				ROLLBACK TRAN --RollBack in case of Error
			END
			
			DECLARE @message varchar(500) = ERROR_MESSAGE()
			DECLARE @severity int = ERROR_SEVERITY()
			RAISERROR(@message, @severity, 1)

	END CATCH
	END
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 7.12.11 08:50
-- Description:	Adds an info line text to the system
-- =============================================
CREATE PROCEDURE [dbo].[InfoLineText_Insert] 
	(@itcsStationId int,
	@lineNumber int,
	@destinationId int,
	@displayText varchar(100),
	@expirationTime datetime,
	@dateCreated datetime
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION InfoLineText_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [InfoLineTextSet]
			([ItcsStationId]
			, [LineNumber]
			, [DestinationId]
			, [DisplayText]
			, [ExpirationTime]
			, [DateCreated]
			)
			VALUES
			(@itcsStationId ,
	@lineNumber ,
	@destinationId ,
	@displayText ,
	@expirationTime ,
	@dateCreated)
			
			DECLARE @id int = SCOPE_IDENTITY()
			SELECT @id
			
			COMMIT TRAN -- Transaction Success!

	END TRY

	BEGIN CATCH


		IF @@TRANCOUNT > 0
			BEGIN
				ROLLBACK TRAN --RollBack in case of Error
			END
			
			DECLARE @message varchar(500) = ERROR_MESSAGE()
			DECLARE @severity int = ERROR_SEVERITY()
			RAISERROR(@message, @severity, 1)

	END CATCH
	END
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 7.12.11 09:20
-- Description:	Adds a station to the system
-- =============================================
CREATE PROCEDURE [dbo].[Station_Insert] 
	(@name varchar(100),
	@number varchar(100),
	@latitude float,
	@longitude float,
	@utcOffset datetime,
	@daylightSaving bit = 0,
	@dateCreated datetime = NULL
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Station_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [StationSet]
			([Name]
			, [Number]
			, [Latitude]
			, [Longitude]
			, [UtcOffset]
			, [DayLightSaving]
			, [DateCreated]
			)
			VALUES
			(@name ,
	@number ,
	@latitude ,
	@longitude ,
	@utcOffset ,
	@daylightSaving ,
	@dateCreated)
			
			DECLARE @id int = SCOPE_IDENTITY()
			SELECT @id
			
			COMMIT TRAN -- Transaction Success!

	END TRY

	BEGIN CATCH


		IF @@TRANCOUNT > 0
			BEGIN
				ROLLBACK TRAN --RollBack in case of Error
			END
			
			DECLARE @message varchar(500) = ERROR_MESSAGE()
			DECLARE @severity int = ERROR_SEVERITY()
			RAISERROR(@message, @severity, 1)

	END CATCH
	END
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 7.12.11 09:20
-- Description:	Adds a scheduled entry to the system
-- =============================================
CREATE PROCEDURE [dbo].[Scheduled_Insert] 
	(@lineNumber varchar(100),
	@lineText varchar(100),
	@directionNumber varchar(100),
	@directionText varchar(100),
	@laneText varchar(100),
	@scheduledArrival datetime,
	@scheduledDeparture datetime,
	@dateCreated datetime = NULL
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Scheduled_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [ScheduledSet]
			([LineNumber]
			, [LineText]
			, [DirectionNumber]
			, [DirectionText]
			, [LaneText]
			, [ScheduledArrival]
			, [ScheduledDeparture]
			, [DateCreated]
			)
			VALUES
			(@lineNumber ,
	@lineText ,
	@directionNumber ,
	@directionText ,
	@laneText ,
	@scheduledArrival ,
	@scheduledDeparture ,
	@dateCreated )
			
			DECLARE @id int = SCOPE_IDENTITY()
			SELECT @id
			
			COMMIT TRAN -- Transaction Success!

	END TRY

	BEGIN CATCH


		IF @@TRANCOUNT > 0
			BEGIN
				ROLLBACK TRAN --RollBack in case of Error
			END
			
			DECLARE @message varchar(500) = ERROR_MESSAGE()
			DECLARE @severity int = ERROR_SEVERITY()
			RAISERROR(@message, @severity, 1)

	END CATCH
	END
GO




-- =============================================================
-- Versioning
-- =============================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.1.4'
  ,@description = 'Added tables, views and SPs for Itcs timetable, realtime, scheduled, station and infolinetext.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 1
  ,@versionRevision = 4
  ,@dateCreated = '2011-12-12T16:45:00.000'
GO
