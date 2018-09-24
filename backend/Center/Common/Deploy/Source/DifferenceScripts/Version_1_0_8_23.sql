 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[AssociationUnitOperationSet] DROP CONSTRAINT [FK_AssociationUnitOperationSet_OperationSet]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [FK_ActivitySet_OperationSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [FK_OperationSet_UserSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_IsDeleted]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDaySat]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDaySun]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_Repetition]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_OperationStatus]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionOnceStopDateKind]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionWeeklyStartDate]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionOnceStartDateKind]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_WeekRepetition]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionWeeklyStopDateKind]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayFri]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayTue]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayMon]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayThu]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayWed]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [PK_OperationSet]
GO
DROP INDEX [IX_OperationSetName] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_OperationSetOperationStatus] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_OperationSetUserId] ON [dbo].[OperationSet]
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
	[ActivityStatus] [int] NOT NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempOperationSet] ON
INSERT INTO [dbo].[TempOperationSet] ([Id],[UserId],[StartDate],[Name],[StopDate],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[DateCreated],[DateModified],[IsDeleted],[OperationState],[ExecutionOnceStartDateKind],[ExecutionOnceStopDateKind],[ExecutionWeeklyStartDate],[ExecutionWeeklyStopDate],[ExecutionWeeklyBeginTime],[ExecutionWeeklyEndTime],[ActivityStatus],[ExecutionWeeklyStopDateKind],[WeekRepetition]) SELECT [Id],[UserId],[StartDate],[Name],[StopDate],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[DateCreated],[DateModified],[IsDeleted],[OperationState],[ExecutionOnceStartDateKind],[ExecutionOnceStopDateKind],[ExecutionWeeklyStartDate],[ExecutionWeeklyStopDate],[ExecutionWeeklyBeginTime],[ExecutionWeeklyEndTime],[ActivityStatus],[ExecutionWeeklyStopDateKind],[WeekRepetition] FROM [dbo].[OperationSet]
SET IDENTITY_INSERT [dbo].[TempOperationSet] OFF
GO

DROP TABLE [dbo].[OperationSet]
GO
EXEC sp_rename N'[dbo].[TempOperationSet]',N'OperationSet', 'OBJECT'
GO


-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: February 16th, 2012
-- Description:	Retrieves the submitted activity for an operation on a specified unit.
-- ACHTUNG! In this version, all activities are returned, and not only active ones.
-- The management of the state of an activity should be implemented.
-- =============================================
ALTER PROCEDURE [dbo].[Operation_GetActivitySubmissions] 
	-- Add the parameters for the stored procedure here
	@operationId int = NULL,
	@unitId int = NULL,
	@onlyActive bit = 0,
	@userId int = NULL,
	@tenantId int = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

DECLARE @operations TABLE
		([Id] int
      ,[UserId] int
      ,[StartDate] datetime
      ,[Name] varchar(100)
      ,[StopDate] datetime NULL
      ,[OperationState] int
      ,[StartExecutionDayMon] bit
      ,[StartExecutionDayTue] bit
      ,[StartExecutionDayWed] bit
      ,[StartExecutionDayThu] bit
      ,[StartExecutionDayFri] bit
      ,[StartExecutionDaySat] bit
      ,[StartExecutionDaySun] bit
      ,[Repetition] int
      ,[DateCreated] datetime
      ,[DateModified] datetime NULL
      ,[ExecutionOnceStartDateKind] int
      ,[ExecutionOnceStopDateKind] int
      ,[ExecutionWeeklyStartDate] datetime
      ,[ExecutionWeeklyStopDate] datetime
      ,[ExecutionWeeklyBeginTime] datetime
      ,[ExecutionWeeklyEndTime] datetime
      ,[ExecutionWeeklyStopDateKind] int
      ,[WeekRepetition] int
      , [ActivityStatus] int)


    -- Insert statements for procedure here
    INSERT INTO @operations
	EXEC [Operation_SelectByUser] @userId = @userId, @tenantId = @tenantId
	
	DECLARE @unit_operation TABLE([UnitId] int, [OperationId] int)
	
	INSERT INTO @unit_operation
	SELECT [UO].[UnitId], [UO].[OperationId]
	FROM [AssociationsUnitOperation] [UO]
	WHERE [UO].[OperationId] IN
	(
		SELECT [O].[Id]
		FROM @operations [O]
		WHERE (@operationId IS NULL OR [O].[Id]=@operationId)
	)
	AND
	(@unitId IS NULL OR [UO].[UnitId]=@unitId)
	
	SELECT [A].[Id] [ActivityId], [UO].[OperationId], [UO].[UnitId]
	FROM @unit_operation [UO]
	INNER JOIN [Activities] [A] ON [A].[OperationId]=[UO].[OperationId]
END
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
ALTER TABLE [dbo].[OperationSet] ADD CONSTRAINT [PK_OperationSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
   @name = 'Version 1.0.8.23'
  ,@description = 'Fixed bug in the Operation_GetActivitySubmissions SP.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 8
  ,@versionRevision = 23
  ,@dateCreated = '2012-04-04T10:10:00.000'
GO