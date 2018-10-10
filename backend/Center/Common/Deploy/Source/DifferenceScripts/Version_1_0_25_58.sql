 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ItcsTimeTableEntrySet] DROP COLUMN [DirectionText]
GO
ALTER TABLE [dbo].[ItcsTimeTableEntrySet] ADD 
[DirectionTexts] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[ActivitySet] ADD 
[RemovedOn] [datetime2](7) NULL,
[IsRemoved] AS CONVERT(bit, (case when [RemovedOn] IS NULL then (0) else (1) end)) PERSISTED
GO
ALTER TABLE [dbo].[ActivityInstanceSet] ADD 
[RemovedOn] [datetime2](7) NULL,
[IsRemoved] AS CONVERT(bit, (case when [RemovedOn] IS NULL then (0) else (1) end)) PERSISTED
GO
ALTER TABLE [dbo].[OperationSet]
   DROP COLUMN [IsRevoked]
GO
ALTER TABLE [dbo].[OperationSet] ADD
[IsRevoked] AS CONVERT(bit, (case when [RevokedOn] IS NULL then (0) else (1) end)) PERSISTED
GO
ALTER VIEW [dbo].[ItcsTimeTableEntries]
AS
SELECT [Id]
      ,[ItcsDisplayAreaId]
      ,[OperationalDay]
      ,[LineId]
      ,[DirectionId]
      ,[TripId]
      ,[StopSequenceCounter]
      ,[LastUpdated]
      ,[ValidUntil]
      ,[LineText]
      ,[DirectionTexts]
      ,[ScheduledArrival]
      ,[ScheduledDeparture]
	  ,[IsTripDeleted]
  FROM [Gorba_CenterOnline].[dbo].[ItcsTimeTableEntrySet]
GO
ALTER VIEW [dbo].[ProductTypes]
AS
SELECT [Id]
      ,[UnitTypeId]
      ,[Name]
      ,[Revision]
      ,[Description]
      ,[IsDefault]
      ,[DateCreated]
      ,[DateModified]
      ,[Properties]
  FROM [Gorba_CenterOnline].[dbo].[ProductTypeSet]
WHERE     (IsDeleted = 0)
GO
ALTER VIEW [dbo].[ItcsRealtimeData]
AS
SELECT [ItcsTimeTableEntryId]
      ,[EstimatedArrival]
      ,[EstimatedDeparture]
      ,[RealArrival]
      ,[RealDeparture]
      ,[IsAtStation]
      ,[CleardownReference]
      ,[ContainsRealtime]
      ,[InCongestion]
      ,[PlatformText]
      ,[PlatformId]
	  ,[HasDisruption]
  FROM [Gorba_CenterOnline].[dbo].[ItcsRealtimeDataSet]
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
      ,[SchedulingActivityInstancesCount]
      ,[TotalActivityInstancesCount]
      ,[RemovedOn]
      ,[IsRemoved]
  FROM [Gorba_CenterOnline].[dbo].[ActivitySet]
WHERE     (IsDeleted = 0)
GO
ALTER VIEW [dbo].[ActivitiesInfoText]
AS
SELECT     Base.Id, Base.OperationId, Base.DateCreated, Base.DateModified,
			[Base].[CurrentState], [Base].[RealTaskId],
			[Base].[LastRealTaskCreationDateTime],
			[Base].[RemovedOn], [Base].[IsRemoved],
			Derived.LineNumber, Derived.DestinationId, Derived.DisplayText,
			Derived.ExpirationDate, Derived.InfoRowId, Derived.Blink,
			Derived.DisplayedScreenSide, Derived.Alignment, Derived.Font
FROM         dbo.ActivityInfoTextSet AS Derived INNER JOIN
                      dbo.Activities AS Base ON Derived.Id = Base.Id
GO



--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.25.58'
  ,@description = 'Changed computed column OperationSet.IsRevoked to be a bit instad of an integer. Added Removed properties to ActivitySet and ActivityInstanceSet.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 25
  ,@versionRevision = 58
  ,@dateCreated = '2012-11-21T11:30:00.000'
GO