 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [FK_AlarmSet_UnitSet]
GO
ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [FK_AlarmSet_UserSet]
GO
ALTER TABLE [dbo].[AssociationUnitOperationSet] DROP CONSTRAINT [FK_AssociationUnitOperationSet_OperationSet]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [FK_ActivitySet_OperationSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [FK_OperationSet_UserSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayWed]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayThu]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayFri]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionWeeklyStopDateKind]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_IsDeleted]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayTue]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_WeekRepetition]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_OperationStatus]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionOnceStartDateKind]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionOnceStopDateKind]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_ExecutionWeeklyStartDate]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDaySat]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDaySun]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_Repetition]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayMon]
GO
ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [DF_AlarmSet_IsDeleted]
GO
ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [PK_AlarmSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [PK_OperationSet]
GO
DROP INDEX [IX_OperationSetOperationStatus] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_OperationSetUserId] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_OperationSetName] ON [dbo].[OperationSet]
GO
CREATE TABLE [dbo].[TempAlarmSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[UnitId] [int] NOT NULL,
	[UserId] [int] NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Severity] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[ConfirmationText] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[DateConfirmed] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_AlarmSet_IsDeleted] DEFAULT ((0)),
	[DateModified] [datetime] NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempAlarmSet] ON
INSERT INTO [dbo].[TempAlarmSet] ([Id],[UnitId],[UserId],[Description],[Severity],[Type],[ConfirmationText],[DateCreated],[EndDate],[DateConfirmed],[IsDeleted],[DateModified],[Name]) SELECT [Id],[UnitId],[UserId],[Description],[Severity],[Type],[ConfirmationText],[DateCreated],[EndDate],[DateConfirmed],[IsDeleted],[DateModified],'' FROM [dbo].[AlarmSet]
SET IDENTITY_INSERT [dbo].[TempAlarmSet] OFF
GO

DROP TABLE [dbo].[AlarmSet]
GO
EXEC sp_rename N'[dbo].[TempAlarmSet]',N'AlarmSet', 'OBJECT'
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
	[IsFavorite] [bit] NOT NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempOperationSet] ON
INSERT INTO [dbo].[TempOperationSet] ([Id],[UserId],[StartDate],[Name],[StopDate],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[DateCreated],[DateModified],[IsDeleted],[OperationState],[ExecutionOnceStartDateKind],[ExecutionOnceStopDateKind],[ExecutionWeeklyStartDate],[ExecutionWeeklyStopDate],[ExecutionWeeklyBeginTime],[ExecutionWeeklyEndTime],[ExecutionWeeklyStopDateKind],[WeekRepetition],[ActivityStatus],[IsFavorite]) SELECT [Id],[UserId],[StartDate],[Name],[StopDate],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[DateCreated],[DateModified],[IsDeleted],[OperationState],[ExecutionOnceStartDateKind],[ExecutionOnceStopDateKind],[ExecutionWeeklyStartDate],[ExecutionWeeklyStopDate],[ExecutionWeeklyBeginTime],[ExecutionWeeklyEndTime],[ExecutionWeeklyStopDateKind],[WeekRepetition],[ActivityStatus],0 FROM [dbo].[OperationSet]
SET IDENTITY_INSERT [dbo].[TempOperationSet] OFF
GO

DROP TABLE [dbo].[OperationSet]
GO
EXEC sp_rename N'[dbo].[TempOperationSet]',N'OperationSet', 'OBJECT'
GO


ALTER VIEW [dbo].[Alarms]
AS
SELECT [Id]
      ,[Name]
      ,[UnitId]
      ,[UserId]
      ,[Description]
      ,[Severity]
      ,[Type]
      ,[ConfirmationText]
      ,[DateCreated]
      ,[EndDate]
      ,[DateConfirmed]
      ,[DateModified]
  FROM [dbo].[AlarmSet]
WHERE     (IsDeleted = 0)
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
      ,[IsFavorite]
  FROM [dbo].[OperationSet]
WHERE     (IsDeleted = 0)

;

;
GO
CREATE NONCLUSTERED INDEX [IX_OperationSetOperationStatus] ON [dbo].[OperationSet]
(
	[OperationState] ASC
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
ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [PK_AlarmSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[OperationSet] ADD CONSTRAINT [PK_OperationSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [FK_AlarmSet_UserSet] FOREIGN KEY
	(
		[UserId]
	)
	REFERENCES [dbo].[UserSet]
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
ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [FK_AlarmSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
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
/****** Object:  StoredProcedure [dbo].[Alarm_Insert]    Script Date: 04/25/2012 16:27:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Thomas Epple (ept@gorba.com)
-- Create date: 2012-01-09
-- Description:	Adds an Alarm
-- =============================================
ALTER PROCEDURE [dbo].[Alarm_Insert]
(
	@unitId int,
	@userId int = NULL,
	@name varchar(100),
	@description varchar(500),
	@endDate datetime,
	@dateCreated datetime = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Alarm_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [AlarmSet]
			([UnitId]
			, [UserId]
			, [Name]
			, [Description]
			, [EndDate]
			, [DateCreated])
			VALUES
			(@unitId
			, @userId
			, @name
			, @description
			, @endDate
			, @dateCreated)
			
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

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.10.25'
  ,@description = 'Updated the alarms.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 10
  ,@versionRevision = 25
  ,@dateCreated = '2012-04-25T10:10:00.000'
GO