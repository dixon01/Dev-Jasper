USE [Gorba_CenterOnline]
GO

/****** Object:  StoredProcedure [dbo].[ItcsConfiguration_Insert]    Script Date: 01/24/2012 09:39:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Jerome Coutant
-- Create date: 09-12-2011
-- Description:	
-- =============================================
ALTER PROCEDURE [dbo].[ItcsConfiguration_Insert] 
	@ProtocolConfigurationId int,
	@name varchar(50),
	@description varchar(500) = null,
	@collectSystemData bit = 1,
	@operationDayStartUtc datetime,
	@operationDayDuration datetime,
	@utcOffset datetime,
	@dayLightSaving bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
			BEGIN TRANSACTION ItcsConfiguration_InsertTx -- Start the transaction..
			-- Insert statements for procedure here
			INSERT INTO [Gorba_CenterOnline].[dbo].[ItcsConfigurationSet]
				   ([ProtocolConfigurationId]
				   ,[name]
				   ,[description]
				   ,[collectSystemData]
				   ,[operationDayStartUtc]
				   ,[operationDayDuration]
				   ,[utcOffset]
				   ,[dayLightSaving])
			 VALUES
				   (@ProtocolConfigurationId,
					@name,
					@description,
					@collectSystemData,
					@operationDayStartUtc,
					@operationDayDuration,
					@utcOffset,
					@dayLightSaving)
			
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

