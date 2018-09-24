 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_OperationStatus]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_UntilAbort]
GO
DROP INDEX [IX_OperationSetOperationStatus] ON [dbo].[OperationSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP COLUMN [OperationStatus],[UntilAbort]
GO
ALTER TABLE [dbo].[OperationSet] ADD 
[OperationState] [int] NOT NULL CONSTRAINT [DF_OperationSet_OperationStatus] DEFAULT ((0)),
[ExecutionOnceStartDateKind] [int] NOT NULL CONSTRAINT [DF_OperationSet_ExecutionOnceStartDateKind] DEFAULT ((0)),
[ExecutionOnceStopDateKind] [int] NOT NULL CONSTRAINT [DF_OperationSet_ExecutionOnceStopDateKind] DEFAULT ((0)),
[ExecutionWeeklyStartDate] [datetime] NULL CONSTRAINT [DF_OperationSet_ExecutionWeeklyStartDate] DEFAULT ((0)),
[ExecutionWeeklyStopDate] [datetime] NULL,
[ExecutionWeeklyBeginTime] [datetime] NULL,
[ExecutionWeeklyEndTime] [datetime] NULL,
[ExecutionWeeklyStopDateKind] [int] NOT NULL CONSTRAINT [DF_OperationSet_ExecutionWeeklyStopDateKind] DEFAULT ((0)),
[WeekRepetition] [int] NOT NULL CONSTRAINT [DF_OperationSet_WeekRepetition] DEFAULT ((0))
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
FROM         dbo.OperationSet
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
-- Create date: 04.0.12
-- Description:	Finds an operation by name. The search is case insensitive and finds the given string even if it is in the middle of the name.
-- =============================================
ALTER PROCEDURE [dbo].[Operation_ListByUnit]
(
	@unitId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT *
	FROM [Operations] [O]
	WHERE EXISTS(
	SELECT [A].[OperationId]
	FROM [AssociationsUnitOperation] [A] 
	WHERE [A].[UnitId]=@unitId AND [A].[OperationId]=[O].[Id]
	)
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
			, [WeekRepetition])
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
			, @weekRepetition)
			
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
--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.4.14'
  ,@description = 'Changes in the OperationSet and related views and SPs.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 4
  ,@versionRevision = 14
  ,@dateCreated = '2012-02-04T16:00:00.000'
GO