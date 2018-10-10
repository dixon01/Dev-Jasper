 

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
	[ActivityStatus] [int] NOT NULL,
	[ExecutionWeeklyStopDateKind] [int] NOT NULL CONSTRAINT [DF_OperationSet_ExecutionWeeklyStopDateKind] DEFAULT ((0)),
	[WeekRepetition] [int] NOT NULL CONSTRAINT [DF_OperationSet_WeekRepetition] DEFAULT ((0))

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempOperationSet] ON
INSERT INTO [dbo].[TempOperationSet] ([Id],[UserId],[StartDate],[Name],[StopDate],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[DateCreated],[DateModified],[IsDeleted],[OperationState],[ExecutionOnceStartDateKind],[ExecutionOnceStopDateKind],[ExecutionWeeklyStartDate],[ExecutionWeeklyStopDate],[ExecutionWeeklyBeginTime],[ExecutionWeeklyEndTime],[ExecutionWeeklyStopDateKind],[WeekRepetition],[ActivityStatus]) SELECT [Id],[UserId],[StartDate],[Name],[StopDate],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[DateCreated],[DateModified],[IsDeleted],[OperationState],[ExecutionOnceStartDateKind],[ExecutionOnceStopDateKind],[ExecutionWeeklyStartDate],[ExecutionWeeklyStopDate],[ExecutionWeeklyBeginTime],[ExecutionWeeklyEndTime],[ExecutionWeeklyStopDateKind],[WeekRepetition],0 FROM [dbo].[OperationSet]
SET IDENTITY_INSERT [dbo].[TempOperationSet] OFF
GO

DROP TABLE [dbo].[OperationSet]
GO
EXEC sp_rename N'[dbo].[TempOperationSet]',N'OperationSet', 'OBJECT'
GO


ALTER VIEW [dbo].[Operations]
AS
SELECT [Id]
      ,[UserId]
      ,[StartDate]
      ,[Name]
      ,[StopDate]
      ,[OperationState]
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
      ,[ExecutionOnceStartDateKind]
      ,[ExecutionOnceStopDateKind]
      ,[ExecutionWeeklyStartDate]
      ,[ExecutionWeeklyStopDate]
      ,[ExecutionWeeklyBeginTime]
      ,[ExecutionWeeklyEndTime]
      ,[ExecutionWeeklyStopDateKind]
      ,[WeekRepetition]
      , [ActivityStatus]
FROM         dbo.OperationSet
WHERE     (IsDeleted = 0)
GO
ALTER VIEW [dbo].[Users]
AS
SELECT [Id]
      ,[Username]
      , [HashedPassword]
      ,[Name]
      ,[LastName]
      ,[Email]
      ,[DateCreated]
      ,[DateModified]
      ,[Culture]
FROM         dbo.UserSet
WHERE     (IsDeleted = 0)
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 22.12.11
-- Description:	Updates an operation
-- =============================================
ALTER PROCEDURE [dbo].[Operation_Update] 
	-- Add the parameters for the stored procedure her
	@id int,
	@startDate datetime,
	@name varchar(100),
	@stopDate datetime,
	@operationState int,
	@startExecutionDayMon bit = 0,
	@startExecutionDayTue bit = 0,
	@startExecutionDayWed bit = 0,
	@startExecutionDayThu bit = 0,
	@startExecutionDayFri bit = 0,
	@startExecutionDaySat bit = 0,
	@startExecutionDaySun bit = 0,
	@repetition int
	, @executionOnceStartDateKind int
	, @executionOnceStopDateKind int
	, @executionWeeklyStartDate datetime = NULL
	, @executionWeeklyStopDate datetime = NULL
	, @executionWeeklyBeginTime datetime = NULL
	, @executionWeeklyEndTime datetime = NULL
	, @executionWeeklyStopDateKind int = 0
	, @weekRepetition int = 0
	, @dateModified datetime = NULL
	, @activityStatus int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Unit_EditTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateModified IS NULL
			BEGIN
				SET @dateModified = GETUTCDATE()
			END
			
			UPDATE [OperationSet] 
			SET [StartDate] = @startDate
			, [Name] = @name
			, [StopDate] = @stopDate
			, [OperationState] = @operationState
			, [StartExecutionDayMon] = @startExecutionDayMon
			, [StartExecutionDayTue] = @startExecutionDayTue
			, [StartExecutionDayWed] = @startExecutionDayWed
			, [StartExecutionDayThu] = @startExecutionDayThu
			, [StartExecutionDayFri] = @startExecutionDayFri
			, [StartExecutionDaySat] = @startExecutionDaySat
			, [StartExecutionDaySun] = @startExecutionDaySun
			, [Repetition] = @repetition
			, [ExecutionOnceStartDateKind] = @executionOnceStartDateKind
			, [ExecutionOnceStopDateKind] = @executionOnceStopDateKind
			, [ExecutionWeeklyStartDate] = @executionWeeklyStartDate
			, [ExecutionWeeklyStopDate] = @executionWeeklyStopDate
			, [ExecutionWeeklyBeginTime] = @executionWeeklyBeginTime
			, [ExecutionWeeklyEndTime] = @executionWeeklyEndTime
			, [ExecutionWeeklyStopDateKind] = @executionWeeklyStopDateKind
			, [WeekRepetition] = @weekRepetition
			, [DateModified] = @dateModified
			, [ActivityStatus] = @activityStatus
			WHERE [Id] = @id
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
-- Author:		<Author,,Name>
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Operation_Insert]
(
	@userId int
	, @name varchar(100)
	, @dateCreated datetime
	, @startDate datetime
	, @stopDate datetime
	, @operationState int
	, @startExecutionDayMon bit = 0
	, @startExecutionDayTue bit = 0
	, @startExecutionDayWed bit = 0
	, @startExecutionDayThu bit = 0
	, @startExecutionDayFri bit = 0
	, @startExecutionDaySat bit = 0
	, @startExecutionDaySun bit = 0
	, @repetition int
	, @executionOnceStartDateKind int
	, @executionOnceStopDateKind int
	, @executionWeeklyStartDate datetime = NULL
	, @executionWeeklyStopDate datetime = NULL
	, @executionWeeklyBeginTime datetime = NULL
	, @executionWeeklyEndTime datetime = NULL
	, @executionWeeklyStopDateKind int = 0
	, @weekRepetition int = 0
	, @activityStatus int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Operation_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [OperationSet]
			([UserId]
			, [Name]
			, [DateCreated]
			, [StartDate]
			, [StopDate]
			, [OperationState]
			, [StartExecutionDayMon]
			, [StartExecutionDayTue]
			, [StartExecutionDayWed]
			, [StartExecutionDayThu]
			, [StartExecutionDayFri]
			, [StartExecutionDaySat]
			, [StartExecutionDaySun]
			, [Repetition]
			, [ExecutionOnceStartDateKind]
			, [ExecutionOnceStopDateKind]
			, [ExecutionWeeklyStartDate]
			, [ExecutionWeeklyStopDate]
			, [ExecutionWeeklyBeginTime]
			, [ExecutionWeeklyEndTime]
			, [ExecutionWeeklyStopDateKind]
			, [WeekRepetition]
			, [ActivityStatus])
			VALUES
			(@userId
			, @name
			, @dateCreated
			, @startDate
			, @stopDate
			, @operationState
			, @startExecutionDayMon
			, @startExecutionDayTue
			, @startExecutionDayWed
			, @startExecutionDayThu
			, @startExecutionDayFri
			, @startExecutionDaySat
			, @startExecutionDaySun
			, @repetition
			, @executionOnceStartDateKind
			, @executionOnceStopDateKind
			, @executionWeeklyStartDate
			, @executionWeeklyStopDate
			, @executionWeeklyBeginTime
			, @executionWeeklyEndTime
			, @executionWeeklyStopDateKind
			, @weekRepetition
			, @activityStatus)
			
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
   @name = 'Version 1.0.8.22'
  ,@description = 'Added Activity status on the operations.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 8
  ,@versionRevision = 22
  ,@dateCreated = '2012-04-02T10:10:00.000'
GO