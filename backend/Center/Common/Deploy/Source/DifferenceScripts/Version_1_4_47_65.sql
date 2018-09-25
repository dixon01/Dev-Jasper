/*
 * EPT 10.10.13
 * WARNING!!! always backup the database before executing the script
 * Extends the Operation table adding a not null column named Priority.
 * WARNING!!! For existing operations the priority 2 (icenter.online) will be set.
*/

USE [Gorba_CenterOnline]
	
-- Add the Priority to OperationSet as nullable
ALTER TABLE [Gorba_CenterOnline].[dbo].[OperationSet]
ADD [Priority] tinyint NULL
GO

BEGIN TRY --Start the Try Block..
	BEGIN TRANSACTION [ClearDatabase_Tx] -- Start the transaction..
	DECLARE @operationsCount int
	
	SELECT @operationsCount = COUNT(*)
	FROM [Gorba_CenterOnline].[dbo].[OperationSet]
	
	IF @operationsCount > 0
		BEGIN	 

			DECLARE @priority tinyint = 2

			UPDATE [Gorba_CenterOnline].[dbo].[OperationSet]
			SET [Priority] = @priority
		END
	-- ENDIF
	
	
	--=============================================================
	-- Versioning
	--=============================================================
	DECLARE @RC int

	EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
	   @name = 'Version 1.4.47.65'
	  ,@description = 'Added column Priority to table OperationSet and updated the view.'
	  ,@versionMajor = 1
	  ,@versionMinor = 4
	  ,@versionBuild = 47
	  ,@versionRevision = 65
	  ,@dateCreated = '2013-10-10T09:00:00.000'

	COMMIT TRAN -- Transaction Success!

END TRY

	BEGIN CATCH


	IF @@TRANCOUNT > 0


		ROLLBACK TRAN --RollBack in case of Error



-- you can Raise ERROR with RAISEERROR() Statement including the details of the exception

	DECLARE @errorMessage varchar(5000) = ERROR_MESSAGE()


	RAISERROR(@errorMessage, 11, 1)

END CATCH

GO

-- Remove the nullability from added columns
ALTER TABLE [Gorba_CenterOnline].[dbo].[OperationSet]
ALTER COLUMN [Priority] tinyint NOT NULL
GO
	
ALTER VIEW [dbo].[Operations]
AS
SELECT     [Id], [UserId], [StartDate], [Name], [StopDate], [StartExecutionDayMon], [StartExecutionDayTue], [StartExecutionDayWed], [StartExecutionDayThu], [StartExecutionDayFri], 
                      [StartExecutionDaySat], [StartExecutionDaySun], [Repetition], [DateCreated], [DateModified], 0 AS OperationState, [ExecutionOnceStartDateKind], [ExecutionOnceStopDateKind], 
                      [ExecutionWeeklyStartDate], [ExecutionWeeklyStopDate], [ExecutionWeeklyBeginTime], [ExecutionWeeklyEndTime], [ExecutionWeeklyStopDateKind], [WeekRepetition], 
                      [ActivityStatus], [RevokedOn], [IsRevoked], [RevokedBy], [FavoriteFlag], [Priority], TotalActivitiesCount AS ActivitiesCount, [TotalActivitiesCount], [ErrorActivitiesCount], 
                      [RevokingActivitiesCount], [ActiveActivitiesCount], [TransmittingActivitiesCount], [TransmittedActivitiesCount], [ScheduledActivitiesCount], [EndedActivitiesCount], 
                      [RevokedActivitiesCount]
FROM         [Gorba_CenterOnline].[dbo].[OperationSet]
WHERE     (IsDeleted = 0)
GO