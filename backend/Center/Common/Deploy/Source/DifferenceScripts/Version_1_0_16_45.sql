 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[VdvMessageSet] DROP CONSTRAINT [FK_VdvMessageSet_ItcsProviders]
GO
ALTER TABLE [dbo].[VdvMessageSet] DROP CONSTRAINT [FK_VdvMessageSet_UnitSet]
GO
ALTER TABLE [dbo].[VdvMessageSet] DROP CONSTRAINT [FK_VdvMessageSet_VdvMessageSet]
GO
ALTER TABLE [dbo].[VdvMessageSet] DROP CONSTRAINT [DF_VdvMessageSet_IsDeleted]
GO
ALTER TABLE [dbo].[VdvMessageSet] DROP CONSTRAINT [PK_VdvMessageSet]
GO
CREATE TABLE [dbo].[ReferenceTextSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UnitId] [int] NOT NULL,
	[ReferenceTextId] [bigint] NOT NULL,
	[FontNumber] [tinyint] NOT NULL,
	[ReferenceType] [int] NOT NULL,
	[DisplayText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TtsText] [varchar] (160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ValidUntil] [datetime2](7) NULL,
	[DateCreated] [datetime2](7) NOT NULL CONSTRAINT [DF_ReferenceTextSet_DateCreated] DEFAULT (getutcdate()),
	[DateModified] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
	CONSTRAINT [PK_ReferenceTextSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TempVdvMessageSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UnitId] [int] NOT NULL,
	[ItcsIdentifier] [int] NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LineTextId] [bigint] NOT NULL,
	[PlatformTextId] [bigint] NOT NULL,
	[Destination1TextId] [bigint] NOT NULL,
	[Destination2TextId] [bigint] NOT NULL,
	[ScheduledArrivalTime] [bigint] NOT NULL,
	[ScheduledDepartureTime] [bigint] NOT NULL,
	[EstimatedArrivalTime] [bigint] NULL,
	[EstimatedDepartureTime] [bigint] NULL,
	[TripId] [bigint] NOT NULL,
	[CleardownReference] [int] NULL,
	[ContainsRealtime] [bit] NOT NULL,
	[IsAtStation] [bit] NOT NULL,
	[TrafficJamIndicator] [int] NULL,
	[RblLine] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RblDirection] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[IsRealTimeMessage] [bit] NOT NULL,
	[ProviderId] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL CONSTRAINT [DF_VdvMessageSet_DateCreated] DEFAULT (getutcdate()),
	[DateModified] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_VdvMessageSet_IsDeleted] DEFAULT ((0)),
	[ValidUntil] [datetime2](7) NOT NULL,
	[StopSequenceCounter] [int] NOT NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempVdvMessageSet] ON
INSERT INTO [dbo].[TempVdvMessageSet] ([Id],[UnitId],[ItcsIdentifier],[Description],[LineTextId],[PlatformTextId],[Destination1TextId],[Destination2TextId],[ScheduledArrivalTime],[ScheduledDepartureTime],[EstimatedArrivalTime],[EstimatedDepartureTime],[TripId],[CleardownReference],[ContainsRealtime],[IsAtStation],[TrafficJamIndicator],[RblLine],[RblDirection],[IsRealTimeMessage],[ProviderId],[DateCreated],[DateModified],[IsDeleted],[ValidUntil],[StopSequenceCounter]) SELECT [Id],[UnitId],[ItcsIdentifier],[Description],[LineTextId],[PlatformTextId],[Destination1TextId],[Destination2TextId],[ScheduledArrivalTime],[ScheduledDepartureTime],[EstimatedArrivalTime],[EstimatedDepartureTime],[TripId],[CleardownReference],[ContainsRealtime],[IsAtStation],[TrafficJamIndicator],[RblLine],[RblDirection],[IsRealTimeMessage],[ProviderId],[DateCreated],[DateModified],[IsDeleted],getdate(),0 FROM [dbo].[VdvMessageSet]
SET IDENTITY_INSERT [dbo].[TempVdvMessageSet] OFF
GO

DROP TABLE [dbo].[VdvMessageSet]
GO
EXEC sp_rename N'[dbo].[TempVdvMessageSet]',N'VdvMessageSet', 'OBJECT'
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
      , ISNULL(CASE
		WHEN [Error].[State] IS NOT NULL THEN [Error].[State]
		WHEN [Revoking].[State] IS NOT NULL THEN [Revoking].[State]
		WHEN [Active].[State] IS NOT NULL THEN [Active].[State]
		WHEN [A].[Count] > 0 AND ([Transmitting].[State] IS NOT NULL OR [A].[Count] > [Transmitted].[Count]) THEN [Transmitting].[State]
		WHEN [A].[Count] > 0 AND [A].[Count] = [Transmitted].[Count] THEN [Revoked].[State]
		WHEN [A].[Count] > 0 AND ([A].[Count] = [Created].[Count] OR [A].[Count] = [Scheduling].[Count] OR [A].[Count] = [Scheduled].[Count]) THEN [Scheduled].[State]
		WHEN [A].[Count] > 0 AND [A].[Count] = [Ended].[Count] THEN [Ended].[State]
		WHEN [A].[Count] > 0 AND [A].[Count] = [Revoked].[Count] THEN [Revoked].[State]
		ELSE 0 END, 0) AS [OperationState]
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

WHERE     [U].[IsDeleted] = 0
GO
ALTER TABLE [dbo].[VdvMessageSet] ADD CONSTRAINT [PK_VdvMessageSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ReferenceTextSet] ADD CONSTRAINT [FK_ReferenceTextSet_UnitSet] FOREIGN KEY
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

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.16.45'
  ,@description = 'Changes in Vdv messages and reference texts. Fixed bug in the Operations view.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 16
  ,@versionRevision = 45
  ,@dateCreated = '2012-07-19T08:47:00.000'
GO
