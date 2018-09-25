 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ActivityInstanceStateSet] ALTER COLUMN [EmittedDateTime] [datetime2](7) NOT NULL
GO
ALTER VIEW [dbo].[Operations]
AS
WITH CreatedActivities([Count], [State], [OperationId])
AS
(
	SELECT COUNT(*) AS [Count], 1 AS [State], [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=1
	GROUP BY [A].[OperationId]
)
, SchedulingActivities([Count], [OperationId]) -- TODO
AS
(
	SELECT COUNT([A].[Id]) AS [Count], [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=2
	GROUP BY [A].[OperationId]
)
, ScheduledActivities([Count], [State], [OperationId]) -- TODO
AS
(
	SELECT COUNT([A].[Id]) AS [Count], 6 AS [State], [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=3
	GROUP BY [A].[OperationId]
)
, TransmittingActivities([Count], [State], [OperationId]) -- TODO
AS
(
	SELECT COUNT([A].[Id]) AS [Count], 4 AS [State], [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=4
	GROUP BY [A].[OperationId]
)
, TransmittedActivities([Count], [State], [OperationId]) -- TODO
AS
(
	SELECT COUNT([A].[Id]) AS [Count], 5 AS [State], [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=5
	GROUP BY [A].[OperationId]
)
, ActiveActivities([State], [OperationId])
AS
(
	SELECT 3, [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=6
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
, RevokingActivities([State], [OperationId])
AS
(
	SELECT 2, [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=8
)
, RevokedActivities([Count], [State], [OperationId])
AS
(
	SELECT COUNT([A].[Id]) AS [Count], 8 AS [State], [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=9
	GROUP BY [A].[OperationId]
)
, ErrorActivities([State], [OperationId])
AS
(
	SELECT 1, [A].[OperationId]
	FROM [Activities] [A]
	WHERE [A].[CurrentState]=10
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
		WHEN [A].[Count] > 0 AND ([Transmitting].[State] IS NOT NULL OR [A].[Count] > [Transmitted].[Count]) THEN [Transmitting].[State]
		WHEN [A].[Count] > 0 AND [A].[Count] = [Transmitted].[Count] THEN [Revoked].[State]
		WHEN [A].[Count] > 0 AND ([A].[Count] = [Created].[Count] OR [A].[Count] = [Scheduling].[Count] OR [A].[Count] = [Scheduled].[Count]) THEN [Scheduled].[State]
		WHEN [A].[Count] > 0 AND [A].[Count] = [Ended].[Count] THEN [Ended].[State]
		WHEN [A].[Count] > 0 AND [A].[Count] = [Revoked].[Count] THEN [Revoked].[State]
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
  LEFT OUTER JOIN [TransmittingActivities] [Transmitting] ON [Transmitting].[OperationId]=[O].[Id]
  LEFT OUTER JOIN [TransmittedActivities] [Transmitted] ON [Transmitted].[OperationId]=[O].[Id]
  LEFT OUTER JOIN [CreatedActivities] [Created] ON [Created].[OperationId]=[O].[Id]
  LEFT OUTER JOIN [SchedulingActivities] [Scheduling] ON [Scheduling].[OperationId]=[O].[Id]
  LEFT OUTER JOIN [ScheduledActivities] [Scheduled] ON [Scheduled].[OperationId]=[O].[Id]
  LEFT OUTER JOIN [EndedActivities] [Ended] ON [Ended].[OperationId]=[O].[Id]
  LEFT OUTER JOIN [RevokedActivities] [Revoked] ON [Revoked].[OperationId]=[O].[Id]
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
, RevokingOperations([State], [UnitId])
AS
(
	SELECT 2, [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[ActivityStatus]=2
)
, ActiveOperations([State], [UnitId])
AS
(
	SELECT 3, [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[ActivityStatus]=3
)
, TransmittingOperations([Count], [State], [UnitId])
AS
(
	SELECT COUNT(*) AS [Count], 4, [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[ActivityStatus]=4
	GROUP BY [A].[UnitId]
)
, TransmittedOperations([Count], [State], [UnitId])
AS
(
	SELECT COUNT(*) AS [Count], 5, [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[ActivityStatus]=5
	GROUP BY [A].[UnitId]
)
, ScheduledOperations([Count], [State], [UnitId])
AS
(
	SELECT COUNT(*) AS [Count], 6, [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[ActivityStatus]=6
	GROUP BY [A].[UnitId]
)
, EndedOperations([Count], [State], [UnitId])
AS
(
	SELECT COUNT(*) AS [Count], 7, [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[ActivityStatus]=7
	GROUP BY [A].[UnitId]
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
		WHEN [Error].[State] IS NOT NULL THEN [Error].[State]
		WHEN [Revoking].[State] IS NOT NULL THEN [Revoking].[State]
		WHEN [Active].[State] IS NOT NULL THEN [Active].[State]
		WHEN ([Transmitting].[State] IS NOT NULL OR [Operations].[Count] > [Transmitted].[Count]) THEN [Transmitting].[State]
		WHEN [Operations].[Count] IS NOT NULL AND [Transmitted].[Count] = [Operations].[Count] THEN [Transmitted].[State]
		WHEN [Operations].[Count] IS NOT NULL AND [Operations].[Count] = [Scheduled].[Count] THEN [Scheduled].[State]
		WHEN [Operations].[Count] IS NOT NULL AND [Operations].[Count] = [Ended].[Count] THEN [Ended].[State]
		WHEN [Operations].[Count] IS NOT NULL AND [Operations].[Count] = [Revoked].[Count] THEN [Revoked].[State]
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
LEFT OUTER JOIN [RevokingOperations] [Revoking] ON [Revoking].[UnitId] = [U].[Id]
LEFT OUTER JOIN [ActiveOperations] [Active] ON [Active].[UnitId] = [U].[Id]
LEFT OUTER JOIN [TransmittingOperations] [Transmitting] ON [Transmitting].[UnitId] = [U].[Id]
LEFT OUTER JOIN [TransmittedOperations] [Transmitted] ON [Transmitted].[UnitId] = [U].[Id]
LEFT OUTER JOIN [ScheduledOperations] [Scheduled] ON [Scheduled].[UnitId] = [U].[Id]
LEFT OUTER JOIN [EndedOperations] [Ended] ON [Ended].[UnitId] = [U].[Id]
LEFT OUTER JOIN [RevokedOperations] [Revoked] ON [Revoked].[UnitId] = [U].[Id]
LEFT OUTER JOIN [AggregatedOperations] [Operations] ON [Operations].[UnitId] = [U].[Id]

WHERE     (IsDeleted = 0)
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.15.43'
  ,@description = 'Updated Operations and Units views.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 15
  ,@versionRevision = 43
  ,@dateCreated = '2012-07-09T08:47:00.000'
GO