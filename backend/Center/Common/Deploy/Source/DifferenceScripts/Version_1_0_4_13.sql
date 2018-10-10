 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [FK_OperationSet_UnitSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP COLUMN [UnitId]
GO
ALTER VIEW [dbo].[Operations]
AS
SELECT     Id, UserId, Name, DateCreated, StartDate, StopDate, OperationStatus, UntilAbort, StartExecutionDayMon, StartExecutionDayTue, StartExecutionDayWed, 
                      StartExecutionDayThu, StartExecutionDayFri, StartExecutionDaySat, StartExecutionDaySun, Repetition, DateModified
FROM         dbo.OperationSet
WHERE     (IsDeleted = 0)
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
	, @operationStatus int
	, @untilAbort bit
	, @startExecutionDayMon bit
	, @startExecutionDayTue bit
	, @startExecutionDayWed bit
	, @startExecutionDayThu bit
	, @startExecutionDayFri bit
	, @startExecutionDaySat bit
	, @startExecutionDaySun bit
	, @repetition int
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
			, [OperationStatus]
			, [UntilAbort]
			, [StartExecutionDayMon]
			, [StartExecutionDayTue]
			, [StartExecutionDayWed]
			, [StartExecutionDayThu]
			, [StartExecutionDayFri]
			, [StartExecutionDaySat]
			, [StartExecutionDaySun]
			, [Repetition])
			VALUES
			(@userId
			, @name
			, @dateCreated
			, @startDate
			, @stopDate
			, @operationStatus
			, @untilAbort
			, @startExecutionDayMon
			, @startExecutionDayTue
			, @startExecutionDayWed
			, @startExecutionDayThu
			, @startExecutionDayFri
			, @startExecutionDaySat
			, @startExecutionDaySun
			, @repetition)
			
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

USE [Gorba_CenterOnline]
GO

/****** Object:  StoredProcedure [dbo].[Operation_ListByUnit]    Script Date: 02/04/2012 10:58:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
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
--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.4.13'
  ,@description = 'Removed UnitId from Operations.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 4
  ,@versionRevision = 13
  ,@dateCreated = '2012-01-26T16:00:00.000'
GO

