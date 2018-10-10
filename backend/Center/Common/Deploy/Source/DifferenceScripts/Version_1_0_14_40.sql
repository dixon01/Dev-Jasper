 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ActivityInstanceStateSet] DROP CONSTRAINT [FK_ActivityInstanceStateSet_ActivitySet]
GO
ALTER TABLE [dbo].[InfoLineTextActivitySet] DROP CONSTRAINT [FK_InfoLineTextActivitySet_ActivitySet]
GO
ALTER TABLE [dbo].[SwitchDisplayStateActivitySet] DROP CONSTRAINT [FK_SwitchDisplayStateActivitySet_ActivitySet]
GO
ALTER TABLE [dbo].[ActivityDisplayOnOffSet] DROP CONSTRAINT [FK_ActivityDisplayOnOffSet_ActivitySet]
GO
ALTER TABLE [dbo].[ActivityDeleteTripsSet] DROP CONSTRAINT [FK_ActivityDeleteTripsSet_ActivitySet]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [FK_ActivitySet_OperationSet]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [DF_ActivitySet_IsDeleted]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [PK_ActivitySet]
GO
DROP PROCEDURE [dbo].[Workflow_Insert]
GO
DROP PROCEDURE [dbo].[WorkflowInstance_Insert]
GO
DROP PROCEDURE [dbo].[Activity_Insert]
GO
DROP PROCEDURE [dbo].[InfoLineTextActivity_Insert]
GO
DROP PROCEDURE [dbo].[Unit_Insert]
GO
DROP PROCEDURE [dbo].[Unit_Update]
GO
ALTER TABLE [dbo].[UnitSet] DROP COLUMN [OperationStatus]
GO
CREATE TABLE [dbo].[TempActivitySet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[OperationId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ActivitySet_IsDeleted] DEFAULT ((0)),
	[RealTaskId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LastRealTaskCreationDateTime] [datetime] NOT NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempActivitySet] ON
INSERT INTO [dbo].[TempActivitySet] ([Id],[OperationId],[DateCreated],[DateModified],[IsDeleted],[RealTaskId],[LastRealTaskCreationDateTime]) SELECT [Id],[OperationId],[DateCreated],[DateModified],[IsDeleted],[RealTaskId],getdate() FROM [dbo].[ActivitySet]
SET IDENTITY_INSERT [dbo].[TempActivitySet] OFF
GO

DROP TABLE [dbo].[ActivitySet]
GO
EXEC sp_rename N'[dbo].[TempActivitySet]',N'ActivitySet', 'OBJECT'
GO


ALTER VIEW [dbo].[Activities]
AS
WITH ActivityStates([EmittedDateTime], [ActivityId])
AS
(
	SELECT  MAX([S].[EmittedDateTime]) AS [EmittedDateTime], [S].[ActivityId]
	FROM [ActivityInstanceStateSet] [S]
	GROUP BY [S].[ActivityId]
)
, ActivityStateValues([EmittedDateTime], [State], [ActivityId])
AS
(
	SELECT  [S].[EmittedDateTime], [S].[State], [S].[ActivityId]
	FROM [ActivityInstanceStateSet] [S]
	INNER JOIN [ActivityStates] [All] ON [All].[EmittedDateTime] = [S].[EmittedDateTime] AND [All].[ActivityId] = [S].[ActivityId]
)
SELECT     [A].[Id], [A].[OperationId], [A].[DateCreated], [A].[DateModified], [A].[RealTaskId], [A].[LastRealTaskCreationDateTime]
	, ISNULL(CAST([S].[State] AS int), 0) AS [CurrentState]
FROM         dbo.ActivitySet [A]
LEFT OUTER JOIN [ActivityStateValues] [S] ON [S].[ActivityId]=[A].[Id]
WHERE     (IsDeleted = 0)
GO
ALTER VIEW [dbo].[InfoLineTextActivities]
AS
SELECT     Base.Id, Base.OperationId, Base.DateCreated, Base.DateModified, [Base].[CurrentState], [Base].[RealTaskId], [Base].[LastRealTaskCreationDateTime], Derived.LineNumber, Derived.DestinationId, Derived.DisplayText, 
                      Derived.ExpirationDate, Derived.InfoRowId, Derived.Blink, Derived.DisplayedScreenSide, Derived.Alignment, Derived.Font
FROM         dbo.InfoLineTextActivitySet AS Derived INNER JOIN
                      dbo.Activities AS Base ON Derived.Id = Base.Id


;
GO
ALTER VIEW [dbo].[Operations]
AS
WITH ErrorActivities([State], [OperationId])
AS
(
	SELECT 1, [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=10
)
, RevokingActivities([State], [OperationId])
AS
(
	SELECT 2, [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=8
)
, ActiveActivities([State], [OperationId])
AS
(
	SELECT 3, [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=6
	GROUP BY [A].[OperationId]
)
, TransmittingActivities([Count], [State], [OperationId]) -- TODO
AS
(
	SELECT COUNT([A].[Id]) AS [Count], 8 AS [State], [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=9 AND 0=1
	GROUP BY [A].[OperationId]
)
, TransmittedActivities([Count], [State], [OperationId]) -- TODO
AS
(
	SELECT COUNT([A].[Id]) AS [Count], 8 AS [State], [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=9 AND 0=1
	GROUP BY [A].[OperationId]
)
, ScheduledActivities([Count], [State], [OperationId]) -- TODO
AS
(
	SELECT COUNT([A].[Id]) AS [Count], 8 AS [State], [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=9 AND 0=1
	GROUP BY [A].[OperationId]
)
, EndedActivities([Count], [State], [OperationId])
AS
(
	SELECT COUNT([A].[Id]) AS [Count], 7 AS [State], [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=7
	GROUP BY [A].[OperationId]
)
, RevokedActivities([Count], [State], [OperationId])
AS
(
	SELECT COUNT([A].[Id]) AS [Count], 8 AS [State], [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=9
	GROUP BY [A].[OperationId]
)
, AggregatedActivities([Count], [OperationId])
AS
(
	SELECT COUNT(Id) AS [Count], [A].[OperationId]
	FROM [Activities] [A]
	GROUP BY [A].[OperationId]
)
SELECT [O].[Id]
      ,[O].[UserId]
      ,[O].[StartDate]
      ,[O].[Name]
      ,[O].[StopDate]
      ,[O].[StartExecutionDayMon]
      ,[O].[StartExecutionDayTue]
      ,[O].[StartExecutionDayWed]
      ,[O].[StartExecutionDayThu]
      ,[O].[StartExecutionDayFri]
      ,[O].[StartExecutionDaySat]
      ,[O].[StartExecutionDaySun]
      ,[O].[Repetition]
      ,[O].[DateCreated]
      ,[O].[DateModified]
      ,CASE
		WHEN [Error].[State] IS NOT NULL THEN [Error].[State]
		WHEN [Revoking].[State] IS NOT NULL THEN [Revoking].[State]
		WHEN [Active].[State] IS NOT NULL THEN [Active].[State]
		WHEN [A].[Count] > 0 AND [A].[Count] = [Revoked].[Count] THEN [Revoked].[State]
		WHEN [A].[Count] > 0 AND [A].[Count] = [Ended].[Count] THEN [Ended].[State]
		ELSE 0 END AS [OperationState]
      ,[O].[ExecutionOnceStartDateKind]
      ,[O].[ExecutionOnceStopDateKind]
      ,[O].[ExecutionWeeklyStartDate]
      ,[O].[ExecutionWeeklyStopDate]
      ,[O].[ExecutionWeeklyBeginTime]
      ,[O].[ExecutionWeeklyEndTime]
      ,[O].[ExecutionWeeklyStopDateKind]
      ,[O].[WeekRepetition]
      ,[O].[ActivityStatus]
      , [O].[RevokedOn]
      , [O].[IsRevoked]
      , [O].[RevokedBy]
      , [O].[FavoriteFlag]
      , [A].[Count] AS [ActivitiesCount]
  FROM [dbo].[OperationSet] [O]
  LEFT OUTER JOIN [ErrorActivities] [Error] ON [Error].[OperationId]=[O].[Id]
  LEFT OUTER JOIN [RevokingActivities] [Revoking] ON [Revoking].[OperationId]=[O].[Id]
  LEFT OUTER JOIN [ActiveActivities] [Active] ON [Active].[OperationId]=[O].[Id]
  LEFT OUTER JOIN [RevokedActivities] [Revoked] ON [Revoked].[OperationId]=[O].[Id]
  LEFT OUTER JOIN [EndedActivities] [Ended] ON [Ended].[OperationId]=[O].[Id]
  LEFT OUTER JOIN [AggregatedActivities] [A] ON [A].[OperationId]=[O].[Id]
WHERE     ([O].IsDeleted = 0)

;
GO
ALTER VIEW [dbo].[Units]
AS
WITH Map([Type], [Status])
AS
(
	SELECT 0,1
	UNION ALL
	SELECT 1,2
	UNION ALL
	SELECT 2,4
	UNION ALL
	SELECT 3,8
)
, AlarmStatuses([Status], [UnitId])
AS
(
	SELECT  SUM(DISTINCT ISNULL([M].[Status], 1)) AS [Status], [A].[UnitId]
	FROM [AlarmSet] [A]
	LEFT OUTER JOIN [Map] [M] ON [M].[Type] = [A].[Type]
	WHERE [A].[DateConfirmed] IS NULL
	GROUP BY [A].[UnitId]
)
, ErrorOperations([State], [UnitId])
AS
(
	SELECT 1, [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[ActivityStatus]=1
)
, ActiveOperations([State], [UnitId])
AS
(
	SELECT 3, [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[ActivityStatus]=3
)
, RevokedOperations([Count], [State], [UnitId])
AS
(
	SELECT COUNT(*) AS [Count], 8, [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[ActivityStatus]=8
	GROUP BY [A].[UnitId]
)
, [AggregatedOperations]([Count], [UnitId])
AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId]=[O].[Id]
	GROUP BY [A].[UnitId]
)
SELECT [U].[Id]
      ,[U].[TenantId]
      ,[U].[ProductTypeId]
      ,[U].[GroupId]
      ,[U].[LayoutId]
      ,[U].[StationId]
      ,[U].[Name]
      ,[U].[ShortName]
      ,[U].[SystemName]
      ,[U].[SerialNumber]
      ,[U].[Description]
      ,[U].[DateCreated]
      ,[U].[DateModified]
      ,[U].[NetworkAddress]
      ,[U].[IsOnline]
      ,[U].[LastSeenOnline]
      ,[U].[LocationName]
      ,[U].[CommunicationStatus]
      ,CASE
		WHEN [Active].[State] IS NOT NULL THEN [Active].[State]
		WHEN [Revoked].[State] IS NOT NULL THEN [Revoked].[State]
		WHEN [Operations].[Count] IS NOT NULL AND [Operations].[Count] > 0 AND [Revoked].[Count] = [Operations].[Count] THEN [Revoked].[State]
		ELSE 0 END AS [OperationStatus]
      ,[U].[LastRestartRequestDate]
      ,[U].[LastTimeSyncRequestDate]
      ,[U].[LastTimeSyncValue]
      ,[U].[TimeZoneInfoId]
      , [U].[GatewayAddress]
      , ISNULL(CAST([A].[Status] AS int), 0) AS [AlarmStatus]
      , ISNULL([Operations].[Count], 0) AS [OperationsCount]

FROM         [dbo].[UnitSet] [U]
LEFT OUTER JOIN [AlarmStatuses] [A] ON [A].[UnitId] = [U].[Id]
LEFT OUTER JOIN [ErrorOperations] [Error] ON [Error].[UnitId] = [U].[Id]
LEFT OUTER JOIN [RevokedOperations] [Revoked] ON [Revoked].[UnitId] = [U].[Id]
LEFT OUTER JOIN [ActiveOperations] [Active] ON [Active].[UnitId] = [U].[Id]
LEFT OUTER JOIN [AggregatedOperations] [Operations] ON [Operations].[UnitId] = [U].[Id]

WHERE     (IsDeleted = 0)
GO
ALTER TABLE [dbo].[ActivitySet] ADD CONSTRAINT [PK_ActivitySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[SwitchDisplayStateActivitySet] ADD CONSTRAINT [FK_SwitchDisplayStateActivitySet_ActivitySet] FOREIGN KEY
	(
		[Id]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[InfoLineTextActivitySet] ADD CONSTRAINT [FK_InfoLineTextActivitySet_ActivitySet] FOREIGN KEY
	(
		[Id]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ActivityDeleteTripsSet] ADD CONSTRAINT [FK_ActivityDeleteTripsSet_ActivitySet] FOREIGN KEY
	(
		[Id]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ActivityDisplayOnOffSet] ADD CONSTRAINT [FK_ActivityDisplayOnOffSet_ActivitySet] FOREIGN KEY
	(
		[Id]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ActivityInstanceStateSet] ADD CONSTRAINT [FK_ActivityInstanceStateSet_ActivitySet] FOREIGN KEY
	(
		[ActivityId]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ActivitySet] ADD CONSTRAINT [FK_ActivitySet_OperationSet] FOREIGN KEY
	(
		[OperationId]
	)
	REFERENCES [dbo].[OperationSet]
	(
		[Id]
	)
GO

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.14.40'
  ,@description = 'Edited views to support activity state tracking.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 14
  ,@versionRevision = 40
  ,@dateCreated = '2012-06-25T08:41:00.000'
GO