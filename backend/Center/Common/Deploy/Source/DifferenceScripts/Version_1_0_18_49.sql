 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[AnnouncementTextSet] DROP CONSTRAINT [FK_AnnouncementTextSet_AnnouncementActivitySet]
GO
ALTER TABLE [dbo].[InfoLineTextActivitySet] DROP CONSTRAINT [FK_InfoLineTextActivitySet_ActivitySet]
GO
DROP VIEW [dbo].[InfoLineTextActivities]
GO
DROP TABLE [dbo].[InfoLineTextActivitySet]
GO
DROP TABLE [dbo].[AnnouncementActivitySet]
GO
ALTER TABLE [dbo].[ItcsProviders] ADD 
[ReferenceId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[ActivityDeleteTripsSet] DROP COLUMN [Trips]
GO
ALTER TABLE [dbo].[ActivityDeleteTripsSet] ADD 
[TripId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[LineId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ItcsProviderId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ItcsDirectionId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
CREATE TABLE [dbo].[ActivityBusLineOnOffSet]
(
	[Id] [int] NOT NULL,
	[Activate] [bit] NOT NULL,
	[Line] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LineColumn] [bit] NOT NULL,
	[DestinationColumn] [bit] NOT NULL,
	[LaneColumn] [bit] NOT NULL,
	[TimeColumn] [bit] NOT NULL,
	[SpecialText] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT [PK_ActivityBusLineOnOffSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ActivityInfoTextSet]
(
	[Id] [int] NOT NULL,
	[LineNumber] [int] NOT NULL,
	[DisplayText] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DestinationId] [int] NOT NULL,
	[ExpirationDate] [datetime] NOT NULL,
	[InfoRowId] [smallint] NOT NULL,
	[Blink] [bit] NOT NULL,
	[DisplayedScreenSide] [smallint] NOT NULL,
	[Alignment] [smallint] NOT NULL,
	[Font] [smallint] NULL,
	CONSTRAINT [PK_InfoLineTextActivitySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ActivityAnnouncementTextSet]
(
	[Id] [int] NOT NULL,
	[Interval] [time](7) NOT NULL,
	[Repetition] [int] NOT NULL,
	CONSTRAINT [PK_AnnouncementActivitySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
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
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[ClearOperations]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    
    DELETE FROM ActivityTaskSet
    DELETE FROM ActivityInstanceSet
    
    DELETE FROM ActivityDeleteTripsSet
    DELETE FROM ActivityDisplayOnOffSet
    DELETE FROM AnnouncementTextSet
    DELETE FROM ActivityAnnouncementTextSet
    DELETE FROM ActivityInfoTextSet
    DELETE FROM ActivitySet
    
    DELETE FROM AssociationUnitOperationSet
    
    DELETE FROM OperationSet
    
    UPDATE [UnitSet]
    SET [OperationStatus] = 0,
     [TotalOperationsCount] = 0,
    [ErrorOperationsCount] = 0,
    [RevokingOperationsCount] = 0,
    [ActiveOperationsCount] = 0,
    [TransmittingOperationsCount] = 0,
    [TransmittedOperationsCount] = 0,
    [ScheduledOperationsCount] = 0,
    [EndedOperationsCount] = 0,
    [RevokedOperationsCount] = 0
END
GO
CREATE VIEW [dbo].[ActivitiesInfoText]
AS
SELECT     Base.Id, Base.OperationId, Base.DateCreated, Base.DateModified, [Base].[CurrentState], [Base].[RealTaskId], [Base].[LastRealTaskCreationDateTime], Derived.LineNumber, Derived.DestinationId, Derived.DisplayText, 
                      Derived.ExpirationDate, Derived.InfoRowId, Derived.Blink, Derived.DisplayedScreenSide, Derived.Alignment, Derived.Font
FROM         dbo.ActivityInfoTextSet AS Derived INNER JOIN
                      dbo.Activities AS Base ON Derived.Id = Base.Id


;
GO
ALTER TABLE [dbo].[AnnouncementTextSet] ADD CONSTRAINT [FK_AnnouncementTextSet_AnnouncementActivitySet] FOREIGN KEY
	(
		[AnnouncementId]
	)
	REFERENCES [dbo].[ActivityAnnouncementTextSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ActivityBusLineOnOffSet] ADD CONSTRAINT [FK_ActivityBusLineOnOffSet_ActivitySet] FOREIGN KEY
	(
		[Id]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ActivityInfoTextSet] ADD CONSTRAINT [FK_InfoLineTextActivitySet_ActivitySet] FOREIGN KEY
	(
		[Id]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO




--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.18.49'
  ,@description = 'Fixed Announcement, Line on/off and Delete trip activity tables. Other minor fixes.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 18
  ,@versionRevision = 49
  ,@dateCreated = '2012-08-15T08:47:00.000'
GO