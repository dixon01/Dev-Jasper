 

USE Gorba_CenterOnline
GO

-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: Friday, March 30th
-- Description:	Sets the request restart date for the specified unit.
-- =============================================
CREATE PROCEDURE [dbo].[Unit_Restart]
(
	@unitId int,
	@requestDate datetime
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    BEGIN TRY
		BEGIN TRANSACTION UnitRestart_Tx
		
			UPDATE [UnitSet]
			SET [LastRestartRequestDate] = @requestDate
			WHERE [Id] = @unitId
		
		COMMIT TRANSACTION
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
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: Friday, March 30th
-- Description:	Sets the request time sync date for the specified unit.
-- =============================================
CREATE PROCEDURE [dbo].[Unit_TimeSync]
(
	@unitId int,
	@requestDate datetime,
	@value datetime
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    BEGIN TRY
		BEGIN TRANSACTION UnitTimeSync_Tx
		
			UPDATE [UnitSet]
			SET [LastTimeSyncRequestDate] = @requestDate
			, [LastTimeSyncValue] = @value
			WHERE [Id] = @unitId
		
		COMMIT TRANSACTION
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

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.8.21'
  ,@description = 'Added Unit_Restart and Unit_TimeSync SPs.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 8
  ,@versionRevision = 21
  ,@dateCreated = '2012-03-30T10:10:00.000'
GO