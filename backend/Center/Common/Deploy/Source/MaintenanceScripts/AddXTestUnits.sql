USE [Gorba_CenterOnline]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 18.09.2012
-- Description:	Inserts [x] units to a predefined tenant starting with a predefined NetworkAddress.
-- =============================================
BEGIN TRY
	BEGIN TRANSACTION
		DECLARE @tenantId int = 2 -- The related tenant (2 = Gorba(Oberbüren))
		DECLARE @numberOfUnits int = 1000 -- The number of units to add.
		DECLARE @networkStart varchar(256) = 'A:20' -- The start prefix for the network address.
		DECLARE @productTypeId int = 5 -- The product type (5 = iqube)
		DECLARE @layoutId int = null -- The optional layout id.					
		DECLARE @description varchar(500) = null -- The optional description
		
		DECLARE @dateCreated datetime = GETUTCDATE()
		DECLARE @unitCounter int = 0
		DECLARE @unitName varchar(100) = null
		DECLARE @networkAddress varchar(256) = null
		DECLARE @addressHighNibble int = 1
		DECLARE @addressLowNibble int = 1
		WHILE @unitCounter < @numberOfUnits
		BEGIN
			
			-- Increase the network high nibble when all 255 units of one range are added.
			IF @addressLowNibble % 256 = 0
			BEGIN
				SET @addressHighNibble = @addressHighNibble + 1
				SET @addressLowNibble = 1
			END
			
			SET @unitCounter = @unitCounter + 1
			SET @unitName = 'TestUnit ' + RIGHT('0000' + convert(varchar, @unitCounter),4) 
			SET @networkAddress = @networkStart + '.' + convert(varchar(3), @addressHighNibble) + '.' + convert(varchar(3), @addressLowNibble)
			SET @dateCreated = GETUTCDATE()
			PRINT 'Inserting ' + @unitName + ' with address ' + @networkAddress
			INSERT INTO [UnitSet]
				([TenantId]
      ,[ProductTypeId]
      ,[GroupId]
      ,[LayoutId]
      ,[Name]
      ,[ShortName]
      ,[SystemName]
      ,[SerialNumber]
      ,[Description]
      ,[DateCreated]
      ,[DateModified]
      ,[IsDeleted]
      ,[NetworkAddress]
      ,[IsOnline]
      ,[LastSeenOnline]
      ,[LocationName]
      ,[CommunicationStatus]
      ,[LastRestartRequestDate]
      ,[LastTimeSyncRequestDate]
      ,[LastTimeSyncValue]
      ,[TimeZoneInfoId]
      ,[ErrorOperationsCount]
      ,[StationId]
      ,[RevokingOperationsCount]
      ,[GatewayAddress]
      ,[ActiveOperationsCount]
      ,[TransmittingOperationsCount]
      ,[TransmittedOperationsCount]
      ,[ScheduledOperationsCount]
      ,[EndedOperationsCount]
      ,[RevokedOperationsCount]
      ,[TotalOperationsCount]
      ,[OperationStatus])
				VALUES
				(@tenantId
				, @productTypeId
				, null
				, @layoutId
				, @unitName
				, null
				, null
				, null
				, @description
				, @dateCreated
				, null
				, 0
				, @networkAddress
				, 0
				, null
				, null
				, 2
				, null
				, null
				, null
				, null
				, 0
				, null
				, 0
				, null
				, 0
				, 0
				, 0
				, 0
				, 0
				, 0
				, 0
				, 0
				)
			
			--DECLARE @id int = SCOPE_IDENTITY()
			--SELECT @id
			SET @addressLowNibble = @addressLowNibble + 1
		END
			
	COMMIT TRAN -- Transaction Success!

	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN --RollBack in case of Error

	-- you can Raise ERROR with RAISEERROR() Statement including the details of the exception

		DECLARE @errorMessage varchar(5000) = ERROR_MESSAGE()
		RAISERROR(@errorMessage, 11, 1)
	END CATCH
	