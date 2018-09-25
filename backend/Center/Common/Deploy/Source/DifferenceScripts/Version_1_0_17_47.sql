 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[WorkflowInstanceSet] DROP CONSTRAINT [FK_WorkflowInstanceSet_UnitSet]
GO
ALTER TABLE [dbo].[AssociationUnitOperationSet] DROP CONSTRAINT [FK_AssociationUnitOperationSet_UnitSet]
GO
ALTER TABLE [dbo].[SwitchDisplayStateActivitySet] DROP CONSTRAINT [FK_SwitchDisplayStateActivitySet_ActivitySet]
GO
ALTER TABLE [dbo].[ActivityDisplayOnOffSet] DROP CONSTRAINT [FK_ActivityDisplayOnOffSet_ActivitySet]
GO
ALTER TABLE [dbo].[InfoLineTextActivitySet] DROP CONSTRAINT [FK_InfoLineTextActivitySet_ActivitySet]
GO
ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [FK_AlarmSet_UnitSet]
GO
ALTER TABLE [dbo].[AssociationUnitLineSet] DROP CONSTRAINT [FK_AssociationUnitLineSet_UnitSet]
GO
ALTER TABLE [dbo].[AssociationUnitStationSet] DROP CONSTRAINT [FK_AssociationUnitStationSet_UnitSet]
GO
ALTER TABLE [dbo].[AssociationUnitOperationSet] DROP CONSTRAINT [FK_AssociationUnitOperationSet_OperationSet]
GO
ALTER TABLE [dbo].[ReferenceTextSet] DROP CONSTRAINT [FK_ReferenceTextSet_UnitSet]
GO
ALTER TABLE [dbo].[ActivityDeleteTripsSet] DROP CONSTRAINT [FK_ActivityDeleteTripsSet_ActivitySet]
GO
ALTER TABLE [dbo].[ActivityInstanceStateSet] DROP CONSTRAINT [FK_ActivityInstanceStateSet_ActivitySet]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_UnitGroupSet]
GO
ALTER TABLE [dbo].[VdvMessageSet] DROP CONSTRAINT [FK_VdvMessageSet_UnitSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_LayoutSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_ProductTypeSet1]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_StationSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_TenanttSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [FK_UnitSet_ProductTypeSet]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [FK_ActivitySet_OperationSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [FK_OperationSet_UserSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionOnceStopDateKind]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayThu]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayTue]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDaySun]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayMon]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDaySat]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_IsDeleted]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_Repetition]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_OperationStatus]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionOnceStartDateKind]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayFri]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayWed]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [DF_ActivitySet_IsDeleted]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [DF_UnitSet_IsDeleted]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [DF_UnitSet_IsOnline]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_WeekRepetition]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionWeeklyStartDate]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionWeeklyStopDateKind]
GO
ALTER TABLE [dbo].[ActivityInstanceStateSet] DROP CONSTRAINT [PK_ActivityInstanceStateSet]
GO
ALTER TABLE [dbo].[UnitSet] DROP CONSTRAINT [PK_UnitSet]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [PK_ActivitySet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [PK_OperationSet]
GO
DROP INDEX [IX_UnitSetUniqueName] ON [dbo].[UnitSet]
GO
DROP INDEX [IX_OperationSetOperationStatus] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_OperationSetName] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_OperationSetUserId] ON [dbo].[OperationSet]
GO
ALTER TABLE [dbo].[ItcsDisplayAreas] ADD 
[Properties] [xml] NULL
GO
CREATE TABLE [dbo].[ActivityTaskSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ActivityId] [int] NOT NULL,
	[UnitId] [int] NOT NULL,
	[TaskIdentifier] [uniqueidentifier] NOT NULL,
	[State] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL CONSTRAINT [DF_ActivityInstanceSet_DateCreated] DEFAULT (getutcdate()),
	[DateModified] [datetime2](7) NULL,
	CONSTRAINT [PK_ActivityInstanceSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [IX_ActivityInstanceSet_TaskIdentifier] UNIQUE NONCLUSTERED
	(
		[TaskIdentifier] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ActivityInstanceSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UnitId] [int] NOT NULL,
	[ActivityId] [int] NOT NULL,
	[State] [int] NOT NULL,
	[TaskIdentifier] [uniqueidentifier] NULL,
	[DateCreated] [datetime2](7) NOT NULL CONSTRAINT [DF_ActivityInstanceSet_DateCreated_1] DEFAULT (getutcdate()),
	[DateModified] [datetime2](7) NULL,
	CONSTRAINT [PK_ActivityInstanceSet_1] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TempActivityInstanceStateSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ActivityId] [int] NOT NULL,
	[UnitId] [int] NOT NULL,
	[State] [int] NOT NULL,
	[EmittedDateTime] [datetime2](7) NOT NULL,
	[TaskIdentifier] [uniqueidentifier] NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempActivityInstanceStateSet] ON
INSERT INTO [dbo].[TempActivityInstanceStateSet] ([Id],[ActivityId],[State],[EmittedDateTime],[UnitId]) SELECT [Id],[ActivityId],[State],[EmittedDateTime],0 FROM [dbo].[ActivityInstanceStateSet]
SET IDENTITY_INSERT [dbo].[TempActivityInstanceStateSet] OFF
GO

DROP TABLE [dbo].[ActivityInstanceStateSet]
GO
EXEC sp_rename N'[dbo].[TempActivityInstanceStateSet]',N'ActivityInstanceStateSet', 'OBJECT'
GO


CREATE TABLE [dbo].[TempActivitySet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[OperationId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ActivitySet_IsDeleted] DEFAULT ((0)),
	[RealTaskId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LastRealTaskCreationDateTime] [datetime] NOT NULL,
	[CurrentState] [int] NOT NULL,
	[ErrorActivityInstancesCount] [int] NOT NULL,
	[RevokingActivityInstancesCount] [int] NOT NULL,
	[ActiveActivityInstancesCount] [int] NOT NULL,
	[TransmittingActivityInstancesCount] [int] NOT NULL,
	[TransmittedActivityInstancesCount] [int] NOT NULL,
	[ScheduledActivityInstancesCount] [int] NOT NULL,
	[EndedActivityInstancesCount] [int] NOT NULL,
	[RevokedActivityInstancesCount] [int] NOT NULL,
	[CreatedActivityInstancesCount] [int] NOT NULL,
	[SchedulingActivityInstancesCount] [int] NOT NULL,
	[TotalActivityInstancesCount] [int] NOT NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempActivitySet] ON
INSERT INTO [dbo].[TempActivitySet] ([Id],[OperationId],[DateCreated],[DateModified],[IsDeleted],[RealTaskId],[LastRealTaskCreationDateTime],[CurrentState],[ErrorActivityInstancesCount],[RevokingActivityInstancesCount],[ActiveActivityInstancesCount],[TransmittingActivityInstancesCount],[TransmittedActivityInstancesCount],[ScheduledActivityInstancesCount],[EndedActivityInstancesCount],[RevokedActivityInstancesCount],[CreatedActivityInstancesCount],[SchedulingActivityInstancesCount],[TotalActivityInstancesCount]) SELECT [Id],[OperationId],[DateCreated],[DateModified],[IsDeleted],[RealTaskId],[LastRealTaskCreationDateTime],0,0,0,0,0,0,0,0,0,0,0,0 FROM [dbo].[ActivitySet]
SET IDENTITY_INSERT [dbo].[TempActivitySet] OFF
GO

DROP TABLE [dbo].[ActivitySet]
GO
EXEC sp_rename N'[dbo].[TempActivitySet]',N'ActivitySet', 'OBJECT'
GO


CREATE TABLE [dbo].[TempUnitSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[ProductTypeId] [int] NOT NULL,
	[GroupId] [int] NULL,
	[LayoutId] [int] NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ShortName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SystemName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SerialNumber] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_UnitSet_IsDeleted] DEFAULT ((0)),
	[NetworkAddress] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsOnline] [bit] NOT NULL CONSTRAINT [DF_UnitSet_IsOnline] DEFAULT ((0)),
	[LastSeenOnline] [datetime] NULL,
	[LocationName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CommunicationStatus] [int] NOT NULL,
	[LastRestartRequestDate] [datetime] NULL,
	[LastTimeSyncRequestDate] [datetime] NULL,
	[LastTimeSyncValue] [datetime] NULL,
	[TimeZoneInfoId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ErrorOperationsCount] [int] NOT NULL,
	[StationId] [int] NULL,
	[RevokingOperationsCount] [int] NOT NULL,
	[GatewayAddress] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ActiveOperationsCount] [int] NOT NULL,
	[TransmittingOperationsCount] [int] NOT NULL,
	[TransmittedOperationsCount] [int] NOT NULL,
	[ScheduledOperationsCount] [int] NOT NULL,
	[EndedOperationsCount] [int] NOT NULL,
	[RevokedOperationsCount] [int] NOT NULL,
	[TotalOperationsCount] [int] NOT NULL,
	[OperationStatus] [int] NOT NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempUnitSet] ON
INSERT INTO [dbo].[TempUnitSet] ([Id],[TenantId],[ProductTypeId],[GroupId],[LayoutId],[Name],[ShortName],[SystemName],[SerialNumber],[Description],[DateCreated],[DateModified],[IsDeleted],[NetworkAddress],[IsOnline],[LastSeenOnline],[LocationName],[CommunicationStatus],[LastRestartRequestDate],[LastTimeSyncRequestDate],[LastTimeSyncValue],[TimeZoneInfoId],[StationId],[GatewayAddress],[ErrorOperationsCount],[RevokingOperationsCount],[ActiveOperationsCount],[TransmittingOperationsCount],[TransmittedOperationsCount],[ScheduledOperationsCount],[EndedOperationsCount],[RevokedOperationsCount],[TotalOperationsCount],[OperationStatus]) SELECT [Id],[TenantId],[ProductTypeId],[GroupId],[LayoutId],[Name],[ShortName],[SystemName],[SerialNumber],[Description],[DateCreated],[DateModified],[IsDeleted],[NetworkAddress],[IsOnline],[LastSeenOnline],[LocationName],[CommunicationStatus],[LastRestartRequestDate],[LastTimeSyncRequestDate],[LastTimeSyncValue],[TimeZoneInfoId],[StationId],[GatewayAddress],0,0,0,0,0,0,0,0,0,0 FROM [dbo].[UnitSet]
SET IDENTITY_INSERT [dbo].[TempUnitSet] OFF
GO

DROP TABLE [dbo].[UnitSet]
GO
EXEC sp_rename N'[dbo].[TempUnitSet]',N'UnitSet', 'OBJECT'
GO


CREATE TABLE [dbo].[TempOperationSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[StopDate] [datetime] NULL,
	[StartExecutionDayMon] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDayMon] DEFAULT ((0)),
	[StartExecutionDayTue] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDayTue] DEFAULT ((0)),
	[StartExecutionDayWed] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDayWed] DEFAULT ((0)),
	[StartExecutionDayThu] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDayThu] DEFAULT ((0)),
	[StartExecutionDayFri] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDayFri] DEFAULT ((0)),
	[StartExecutionDaySat] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDaySat] DEFAULT ((0)),
	[StartExecutionDaySun] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDaySun] DEFAULT ((0)),
	[Repetition] [int] NOT NULL CONSTRAINT [DF_OperationSet_Repetition] DEFAULT ((0)),
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_OperationSet_IsDeleted] DEFAULT ((0)),
	[OperationState] [int] NOT NULL CONSTRAINT [DF_OperationSet_OperationStatus] DEFAULT ((0)),
	[ExecutionOnceStartDateKind] [int] NOT NULL CONSTRAINT [DF_OperationSet_ExecutionOnceStartDateKind] DEFAULT ((0)),
	[ExecutionOnceStopDateKind] [int] NOT NULL CONSTRAINT [DF_OperationSet_ExecutionOnceStopDateKind] DEFAULT ((0)),
	[ExecutionWeeklyStartDate] [datetime] NULL CONSTRAINT [DF_OperationSet_ExecutionWeeklyStartDate] DEFAULT ((0)),
	[ExecutionWeeklyStopDate] [datetime] NULL,
	[ExecutionWeeklyBeginTime] [datetime] NULL,
	[ExecutionWeeklyEndTime] [datetime] NULL,
	[ExecutionWeeklyStopDateKind] [int] NOT NULL CONSTRAINT [DF_OperationSet_ExecutionWeeklyStopDateKind] DEFAULT ((0)),
	[WeekRepetition] [int] NOT NULL CONSTRAINT [DF_OperationSet_WeekRepetition] DEFAULT ((0)),
	[ActivityStatus] [int] NOT NULL,
	[RevokedOn] [datetime] NULL,
	[IsRevoked] AS (case when [RevokedOn] IS NULL then (0) else (1) end) PERSISTED,
	[RevokedBy] [int] NULL,
	[FavoriteFlag] [int] NOT NULL,
	[ErrorActivitiesCount] [int] NOT NULL,
	[RevokingActivitiesCount] [int] NOT NULL,
	[ActiveActivitiesCount] [int] NOT NULL,
	[TransmittingActivitiesCount] [int] NOT NULL,
	[TransmittedActivitiesCount] [int] NOT NULL,
	[ScheduledActivitiesCount] [int] NOT NULL,
	[EndedActivitiesCount] [int] NOT NULL,
	[RevokedActivitiesCount] [int] NOT NULL,
	[TotalActivitiesCount] [int] NOT NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempOperationSet] ON
INSERT INTO [dbo].[TempOperationSet] ([Id],[UserId],[StartDate],[Name],[StopDate],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[DateCreated],[DateModified],[IsDeleted],[OperationState],[ExecutionOnceStartDateKind],[ExecutionOnceStopDateKind],[ExecutionWeeklyStartDate],[ExecutionWeeklyStopDate],[ExecutionWeeklyBeginTime],[ExecutionWeeklyEndTime],[ExecutionWeeklyStopDateKind],[WeekRepetition],[ActivityStatus],[RevokedOn],[RevokedBy],[FavoriteFlag],[ErrorActivitiesCount],[RevokingActivitiesCount],[ActiveActivitiesCount],[TransmittingActivitiesCount],[TransmittedActivitiesCount],[ScheduledActivitiesCount],[EndedActivitiesCount],[RevokedActivitiesCount],[TotalActivitiesCount]) SELECT [Id],[UserId],[StartDate],[Name],[StopDate],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[DateCreated],[DateModified],[IsDeleted],[OperationState],[ExecutionOnceStartDateKind],[ExecutionOnceStopDateKind],[ExecutionWeeklyStartDate],[ExecutionWeeklyStopDate],[ExecutionWeeklyBeginTime],[ExecutionWeeklyEndTime],[ExecutionWeeklyStopDateKind],[WeekRepetition],[ActivityStatus],[RevokedOn],[RevokedBy],[FavoriteFlag],0,0,0,0,0,0,0,0,0 FROM [dbo].[OperationSet]
SET IDENTITY_INSERT [dbo].[TempOperationSet] OFF
GO

DROP TABLE [dbo].[OperationSet]
GO
EXEC sp_rename N'[dbo].[TempOperationSet]',N'OperationSet', 'OBJECT'
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
, [Operations_Aggregated] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	GROUP BY [A].[UnitId]
)
, [Operations_Error] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState]=1
	GROUP BY [A].[UnitId]
)
, [Operations_Revoking] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState]=2
	GROUP BY [A].[UnitId]
)
, [Operations_Active] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 3
	GROUP BY [A].[UnitId]
)
, [Operations_Transmitting] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 4
	GROUP BY [A].[UnitId]
)
, [Operations_Transmitted] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 5
	GROUP BY [A].[UnitId]
)
, [Operations_Scheduled] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 6
	GROUP BY [A].[UnitId]
)
, [Operations_Ended] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 7
	GROUP BY [A].[UnitId]
)
, [Operations_Revoked] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 8
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
      , [U].[OperationStatus]
      ,[U].[LastRestartRequestDate]
      ,[U].[LastTimeSyncRequestDate]
      ,[U].[LastTimeSyncValue]
      ,[U].[TimeZoneInfoId]
      , [U].[GatewayAddress]
      , ISNULL(CAST([A].[Status] AS int), 0) AS [AlarmStatus]
      , [U].[ErrorOperationsCount]
      , [U].[RevokingOperationsCount]
      , [U].[ActiveOperationsCount]
      , [U].[TransmittingOperationsCount]
      , [U].[TransmittedOperationsCount]
      , [U].[ScheduledOperationsCount]
      , [U].[EndedOperationsCount]
      , [U].[RevokedOperationsCount]
      , [U].[TotalOperationsCount]

FROM         [dbo].[UnitSet] [U]
LEFT OUTER JOIN [AlarmStatuses] [A] ON [A].[UnitId] = [U].[Id]

WHERE     [U].[IsDeleted] = 0
GO
ALTER VIEW [dbo].[Operations]
AS
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
      , 0 AS [OperationState]
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
      , [O].[TotalActivitiesCount] AS [ActivitiesCount]
      , [O].[TotalActivitiesCount]
      , [O].[ErrorActivitiesCount]
      , [O].[RevokingActivitiesCount]
      , [O].[ActiveActivitiesCount]
      , [O].[TransmittingActivitiesCount]
      , [O].[TransmittedActivitiesCount]
      , [O].[ScheduledActivitiesCount]
      , [O].[EndedActivitiesCount]
      , [O].[RevokedActivitiesCount]
  FROM [dbo].[OperationSet] [O]
WHERE     ([O].IsDeleted = 0)

;
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
CREATE UNIQUE NONCLUSTERED INDEX [IX_UnitSetUniqueName] ON [dbo].[UnitSet]
(
	[TenantId] ASC,
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OperationSetUserId] ON [dbo].[OperationSet]
(
	[UserId] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OperationSetName] ON [dbo].[OperationSet]
(
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OperationSetOperationStatus] ON [dbo].[OperationSet]
(
	[OperationState] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE VIEW [dbo].[ActivityTasks]
AS
WITH ActivityStates([EmittedDateTime], [TaskIdentifier])
AS
(
	SELECT  MAX([S].[EmittedDateTime]) AS [EmittedDateTime], [S].[TaskIdentifier]
	FROM [ActivityInstanceStateSet] [S]
	GROUP BY [S].[TaskIdentifier]
)
, ActivityStateValues([EmittedDateTime], [State], [TaskIdentifier])
AS
(
	SELECT  [S].[EmittedDateTime], [S].[State], [S].[TaskIdentifier]
	FROM [ActivityInstanceStateSet] [S]
	INNER JOIN [ActivityStates] [All] ON [All].[EmittedDateTime] = [S].[EmittedDateTime] AND [All].[TaskIdentifier] = [S].[TaskIdentifier]
)
SELECT     [I].[Id],[I].[TaskIdentifier] AS [TaskIdentifier], [I].[ActivityId] AS [ActivityId], [I].[UnitId] AS [UnitId], ISNULL(CAST([V].[State] AS int), 0) AS [State]
FROM         dbo.ActivityTaskSet [I]
INNER JOIN [ActivityStateValues] [V] ON [V].[TaskIdentifier]=[I].[TaskIdentifier]
GO
ALTER TABLE [dbo].[ActivityInstanceStateSet] ADD CONSTRAINT [PK_ActivityInstanceStateSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ActivitySet] ADD CONSTRAINT [PK_ActivitySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [PK_UnitSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[OperationSet] ADD CONSTRAINT [PK_OperationSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [FK_AlarmSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
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
ALTER TABLE [dbo].[AssociationUnitLineSet] ADD CONSTRAINT [FK_AssociationUnitLineSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[AssociationUnitStationSet] ADD CONSTRAINT [FK_AssociationUnitStationSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[AssociationUnitOperationSet] ADD CONSTRAINT [FK_AssociationUnitOperationSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
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
ALTER TABLE [dbo].[ActivityInstanceStateSet] ADD CONSTRAINT [FK_ActivityInstanceStateSet_ActivitySet] FOREIGN KEY
	(
		[ActivityId]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[AssociationUnitOperationSet] ADD CONSTRAINT [FK_AssociationUnitOperationSet_OperationSet] FOREIGN KEY
	(
		[OperationId]
	)
	REFERENCES [dbo].[OperationSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ActivityInstanceStateSet] ADD CONSTRAINT [FK_ActivityInstanceStateSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ActivityInstanceSet] ADD CONSTRAINT [FK_ActivityInstanceSet_ActivitySet] FOREIGN KEY
	(
		[ActivityId]
	)
	REFERENCES [dbo].[ActivitySet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ActivityInstanceSet] ADD CONSTRAINT [FK_ActivityInstanceSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
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
ALTER TABLE [dbo].[WorkflowInstanceSet] ADD CONSTRAINT [FK_WorkflowInstanceSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
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
ALTER TABLE [dbo].[ReferenceTextSet] ADD CONSTRAINT [FK_ReferenceTextSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_UnitGroupSet] FOREIGN KEY
	(
		[GroupId]
	)
	REFERENCES [dbo].[UnitGroupSet]
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
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_ProductTypeSet] FOREIGN KEY
	(
		[ProductTypeId]
	)
	REFERENCES [dbo].[ProductTypeSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_TenanttSet] FOREIGN KEY
	(
		[TenantId]
	)
	REFERENCES [dbo].[TenantSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_ProductTypeSet1] FOREIGN KEY
	(
		[ProductTypeId]
	)
	REFERENCES [dbo].[ProductTypeSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_LayoutSet] FOREIGN KEY
	(
		[LayoutId]
	)
	REFERENCES [dbo].[LayoutSet]
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
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_StationSet] FOREIGN KEY
	(
		[StationId]
	)
	REFERENCES [dbo].[StationSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[OperationSet] ADD CONSTRAINT [FK_OperationSet_UserSet] FOREIGN KEY
	(
		[UserId]
	)
	REFERENCES [dbo].[UserSet]
	(
		[Id]
	)
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ClearUnits]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DELETE FROM Units
END
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ClearOperations]
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
    DELETE FROM AnnouncementActivitySet
    DELETE FROM InfoLineTextActivitySet
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




--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.17.47'
  ,@description = 'Added the ClearOperations SP, refactored the status management in Units, Operations and Activities. Added the Activity instances.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 17
  ,@versionRevision = 47
  ,@dateCreated = '2012-08-02T08:47:00.000'
GO