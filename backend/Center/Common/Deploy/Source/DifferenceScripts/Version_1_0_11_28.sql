 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ItcsFilterSet] DROP CONSTRAINT [FK_ItcsFilterSet_ItcsStationReferenceSet]
GO
ALTER TABLE [dbo].[ItcsFilterSet] DROP CONSTRAINT [FK_ItcsFilterSet_ItcsConfigurationSet]
GO
ALTER TABLE [dbo].[ItcsFilterSet] DROP CONSTRAINT [FK_ItcsFilterSet_StopPointSet]
GO
ALTER TABLE [dbo].[AssociationUnitOperationSet] DROP CONSTRAINT [FK_AssociationUnitOperationSet_OperationSet]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [FK_ActivitySet_OperationSet]
GO
ALTER TABLE [dbo].[ItcsStationReferenceSet] DROP CONSTRAINT [FK_ItcsStationReferenceSet_ItcsProviderSet]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] DROP CONSTRAINT [FK_ItcsConfiguration_ProtocolConfiguration]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [FK_OperationSet_UserSet]
GO
ALTER TABLE [dbo].[ProtocolConfigurationSet] DROP CONSTRAINT [FK_ProtocolConfiguration_ProtocolType]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionWeeklyStartDate]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayMon]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayTue]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDaySat]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_WeekRepetition]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionWeeklyStopDateKind]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_IsDeleted]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_Repetition]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionOnceStartDateKind]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_OperationStatus]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDaySun]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayThu]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayWed]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionOnceStopDateKind]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayFri]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] DROP CONSTRAINT [DF_ItcsConfiguration_dayLightSaving]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] DROP CONSTRAINT [DF_ItcsConfiguration_collectSystemData]
GO
ALTER TABLE [dbo].[StopPointSet] DROP CONSTRAINT [PK_StopPointSet]
GO
ALTER TABLE [dbo].[ProtocolConfigurationSet] DROP CONSTRAINT [PK_ProtocolConfiguration]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [PK_OperationSet]
GO
ALTER TABLE [dbo].[ProtocolTypeSet] DROP CONSTRAINT [PK_ProtocolType]
GO
DROP INDEX [IX_ItcsConfigurationName] ON [dbo].[ItcsConfigurationSet]
GO
DROP INDEX [IX_OperationSetOperationStatus] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_ProtocolConfiguration] ON [dbo].[ProtocolConfigurationSet]
GO
DROP INDEX [IX_OperationSetName] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_OperationSetUserId] ON [dbo].[OperationSet]
GO
DROP TABLE [dbo].[ItcsStationReferenceSet]
GO
DROP TABLE [dbo].[ItcsProviderSet]
GO
DROP TABLE [dbo].[ItcsFilterSet]
GO
ALTER TABLE [dbo].[UserSet] ADD 
[ShortName] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ALTER COLUMN [Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ALTER COLUMN [Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ALTER COLUMN [DayLightSaving] [bit] NULL
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ADD 
[Properties] [xml] NULL
GO
ALTER TABLE [dbo].[LineSet] DROP COLUMN [ReferenceName]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ALTER COLUMN [UtcOffset] [datetime] NULL
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ALTER COLUMN [CollectSystemData] [bit] NULL
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ALTER COLUMN [OperationDayStartUtc] [datetime] NULL
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ALTER COLUMN [OperationDayDuration] [datetime] NULL
GO
CREATE TABLE [dbo].[ItcsFilters]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[StopPointId] [int] NOT NULL,
	[ItcsConfigurationId] [int] NOT NULL,
	[ItcsDisplayAreaId] [int] NOT NULL,
	[LineName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LineReferenceName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Direction] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DirectionReferenceName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsActive] [bit] NOT NULL,
	[LineId] [int] NULL,
	CONSTRAINT [PK_ItcsFilters] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[UnitStopPointAssociations]
(
	[UnitId] [int] NOT NULL,
	[StopPointId] [int] NOT NULL,
	CONSTRAINT [PK_UnitStopPointAssociations] PRIMARY KEY CLUSTERED
	(
		[UnitId] ASC,
		[StopPointId] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ItcsDisplayAreas]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ProviderId] [int] NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT [PK_ItcsDisplayAreas] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ItcsProviders]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsDeleted] [bit] NOT NULL,
	CONSTRAINT [PK_ItcsProviders] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TempStopPointSet]
(
	[Id] [int] NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsDeleted] [bit] NOT NULL

) ON [PRIMARY]
GO

INSERT INTO [dbo].[TempStopPointSet] ([Id],[Name],[IsDeleted]) SELECT [Id],[Name],0 FROM [dbo].[StopPointSet]
DROP TABLE [dbo].[StopPointSet]
GO
EXEC sp_rename N'[dbo].[TempStopPointSet]',N'StopPointSet', 'OBJECT'
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
	[RevokedBy] [int] NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempOperationSet] ON
INSERT INTO [dbo].[TempOperationSet] ([Id],[UserId],[StartDate],[Name],[StopDate],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[DateCreated],[DateModified],[IsDeleted],[OperationState],[ExecutionOnceStartDateKind],[ExecutionOnceStopDateKind],[ExecutionWeeklyStartDate],[ExecutionWeeklyStopDate],[ExecutionWeeklyBeginTime],[ExecutionWeeklyEndTime],[ExecutionWeeklyStopDateKind],[WeekRepetition],[ActivityStatus],[RevokedOn],[RevokedBy]) SELECT [Id],[UserId],[StartDate],[Name],[StopDate],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[DateCreated],[DateModified],[IsDeleted],[OperationState],[ExecutionOnceStartDateKind],[ExecutionOnceStopDateKind],[ExecutionWeeklyStartDate],[ExecutionWeeklyStopDate],[ExecutionWeeklyBeginTime],[ExecutionWeeklyEndTime],[ExecutionWeeklyStopDateKind],[WeekRepetition],[ActivityStatus],[RevokedOn],[RevokedBy] FROM [dbo].[OperationSet]
SET IDENTITY_INSERT [dbo].[TempOperationSet] OFF
GO

DROP TABLE [dbo].[OperationSet]
GO
EXEC sp_rename N'[dbo].[TempOperationSet]',N'OperationSet', 'OBJECT'
GO


CREATE TABLE [dbo].[TempProtocolConfigurationSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ProtocolTypeId] [int] NOT NULL,
	[realTimeDataOnly] [bit] NULL,
	[httpListenerHost] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpListenerPort] [int] NULL,
	[httpServerHost] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpServerport] [int] NULL,
	[httpWebProxyHost] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpWebProxyPort] [int] NULL,
	[httpClientIdentification] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpServerIdentification] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpResponseTimeOut] [int] NULL,
	[xmlClientRequestSenderId] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[xmlServerRequestSenderId] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[xmlNameSpaceRequest] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[xmlNameSpaceResponse] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[omitXmlDeclaration] [bit] NULL,
	[evaluateDataReadyInStatusResponse] [bit] NULL,
	[statusRequestIntervalInSec] [int] NULL,
	[subscriptionRetryIntervalInSec] [int] NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Properties] [xml] NULL,
	[IsDeleted] [bit] NOT NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempProtocolConfigurationSet] ON
INSERT INTO [dbo].[TempProtocolConfigurationSet] ([Id],[ProtocolTypeId],[realTimeDataOnly],[httpListenerHost],[httpListenerPort],[httpServerHost],[httpServerport],[httpWebProxyHost],[httpWebProxyPort],[httpClientIdentification],[httpServerIdentification],[httpResponseTimeOut],[xmlClientRequestSenderId],[xmlServerRequestSenderId],[xmlNameSpaceRequest],[xmlNameSpaceResponse],[omitXmlDeclaration],[evaluateDataReadyInStatusResponse],[statusRequestIntervalInSec],[subscriptionRetryIntervalInSec],[Name],[IsDeleted]) SELECT [Id],[ProtocolTypeId],[realTimeDataOnly],[httpListenerHost],[httpListenerPort],[httpServerHost],[httpServerport],[httpWebProxyHost],[httpWebProxyPort],[httpClientIdentification],[httpServerIdentification],[httpResponseTimeOut],[xmlClientRequestSenderId],[xmlServerRequestSenderId],[xmlNameSpaceRequest],[xmlNameSpaceResponse],[omitXmlDeclaration],[evaluateDataReadyInStatusResponse],[statusRequestIntervalInSec],[subscriptionRetryIntervalInSec],'',0 FROM [dbo].[ProtocolConfigurationSet]
SET IDENTITY_INSERT [dbo].[TempProtocolConfigurationSet] OFF
GO

DROP TABLE [dbo].[ProtocolConfigurationSet]
GO
EXEC sp_rename N'[dbo].[TempProtocolConfigurationSet]',N'ProtocolConfigurationSet', 'OBJECT'
GO


CREATE TABLE [dbo].[TempProtocolTypeSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsDeleted] [bit] NOT NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempProtocolTypeSet] ON
INSERT INTO [dbo].[TempProtocolTypeSet] ([Id],[Name],[Description],[IsDeleted]) SELECT [Id],[Name],[Description],0 FROM [dbo].[ProtocolTypeSet]
SET IDENTITY_INSERT [dbo].[TempProtocolTypeSet] OFF
GO

DROP TABLE [dbo].[ProtocolTypeSet]
GO
EXEC sp_rename N'[dbo].[TempProtocolTypeSet]',N'ProtocolTypeSet', 'OBJECT'
GO


ALTER VIEW [dbo].[ProtocolTypes]
AS
SELECT [Id]
      ,[Name]
      ,[Description]
      ,[IsDeleted]
  FROM [dbo].[ProtocolTypeSet]
GO
ALTER VIEW [dbo].[Users]
AS
SELECT [Id]
      ,[Username]
      ,[HashedPassword]
      ,[Name]
      ,[LastName]
      ,[Email]
      ,[DateCreated]
      ,[DateModified]
      ,[IsDeleted]
      ,[Culture]
      ,[ShortName]
  FROM [dbo].[UserSet]
GO
ALTER VIEW [dbo].[ItcsConfigurations]
AS
SELECT [Id]
      ,[ProtocolConfigurationId]
      ,[Name]
      ,[Description]
      ,[CollectSystemData]
      ,[OperationDayStartUtc]
      ,[OperationDayDuration]
      ,[UtcOffset]
      ,[DayLightSaving]
      ,[IsDeleted]
      ,[Properties]
  FROM [dbo].[ItcsConfigurationSet]
GO
ALTER VIEW [dbo].[Operations]
AS
SELECT [Id]
      ,[UserId]
      ,[StartDate]
      ,[Name]
      ,[StopDate]
      ,[StartExecutionDayMon]
      ,[StartExecutionDayTue]
      ,[StartExecutionDayWed]
      ,[StartExecutionDayThu]
      ,[StartExecutionDayFri]
      ,[StartExecutionDaySat]
      ,[StartExecutionDaySun]
      ,[Repetition]
      ,[DateCreated]
      ,[DateModified]
      ,[OperationState]
      ,[ExecutionOnceStartDateKind]
      ,[ExecutionOnceStopDateKind]
      ,[ExecutionWeeklyStartDate]
      ,[ExecutionWeeklyStopDate]
      ,[ExecutionWeeklyBeginTime]
      ,[ExecutionWeeklyEndTime]
      ,[ExecutionWeeklyStopDateKind]
      ,[WeekRepetition]
      ,[ActivityStatus]
  FROM [dbo].[OperationSet]
WHERE     (IsDeleted = 0)

;
GO
ALTER VIEW [dbo].[Lines]
AS
SELECT [Id]
      ,[Name]
      ,[ShortName]
      ,[Description]
      , [IsDeleted]
  FROM [dbo].[LineSet]
GO
CREATE NONCLUSTERED INDEX [IX_ItcsConfigurationName] ON [dbo].[ItcsConfigurationSet]
(
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProtocolConfiguration] ON [dbo].[ProtocolConfigurationSet]
(
	[Id] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OperationSetName] ON [dbo].[OperationSet]
(
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OperationSetUserId] ON [dbo].[OperationSet]
(
	[UserId] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OperationSetOperationStatus] ON [dbo].[OperationSet]
(
	[OperationState] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ADD CONSTRAINT [DF_ItcsConfiguration_dayLightSaving] DEFAULT ((0)) FOR [DayLightSaving]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ADD CONSTRAINT [DF_ItcsConfiguration_collectSystemData] DEFAULT ((1)) FOR [CollectSystemData]
GO
ALTER TABLE [dbo].[StopPointSet] ADD CONSTRAINT [PK_StopPointSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ProtocolConfigurationSet] ADD CONSTRAINT [PK_ProtocolConfiguration] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[OperationSet] ADD CONSTRAINT [PK_OperationSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ProtocolTypeSet] ADD CONSTRAINT [PK_ProtocolType] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ItcsFilters] ADD CONSTRAINT [FK_ItcsFilters_ItcsConfigurations] FOREIGN KEY
	(
		[ItcsConfigurationId]
	)
	REFERENCES [dbo].[ItcsConfigurationSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ItcsFilters] ADD CONSTRAINT [FK_ItcsFilters_ItcsDisplayAreas] FOREIGN KEY
	(
		[ItcsDisplayAreaId]
	)
	REFERENCES [dbo].[ItcsDisplayAreas]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ItcsFilters] ADD CONSTRAINT [FK_ItcsFilters_StopPoints] FOREIGN KEY
	(
		[StopPointId]
	)
	REFERENCES [dbo].[StopPointSet]
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
ALTER TABLE [dbo].[ActivitySet] ADD CONSTRAINT [FK_ActivitySet_OperationSet] FOREIGN KEY
	(
		[OperationId]
	)
	REFERENCES [dbo].[OperationSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ADD CONSTRAINT [FK_ItcsConfiguration_ProtocolConfiguration] FOREIGN KEY
	(
		[ProtocolConfigurationId]
	)
	REFERENCES [dbo].[ProtocolConfigurationSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[ItcsDisplayAreas] ADD CONSTRAINT [FK_ItcsDisplayAreas_ItcsProviders] FOREIGN KEY
	(
		[ProviderId]
	)
	REFERENCES [dbo].[ItcsProviders]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ProtocolConfigurationSet] ADD CONSTRAINT [FK_ProtocolConfiguration_ProtocolType] FOREIGN KEY
	(
		[ProtocolTypeId]
	)
	REFERENCES [dbo].[ProtocolTypeSet]
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

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.11.28'
  ,@description = 'Extended Itcs model.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 11
  ,@versionRevision = 28
  ,@dateCreated = '2012-05-09T07:10:00.000'
GO