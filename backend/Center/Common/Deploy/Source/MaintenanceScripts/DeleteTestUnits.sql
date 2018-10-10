USE [Gorba_CenterOnline]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 18.09.2012
-- Description:	Removes all units from database with a predefined network prefix.
-- =============================================
BEGIN TRY
	BEGIN TRANSACTION
		DECLARE @networkStart varchar(256) = 'A:20%' -- The start prefix for the network address.
		
		DELETE FROM [UnitSet]
		WHERE [NetworkAddress] LIKE @networkStart	
	COMMIT TRAN -- Transaction Success!
		
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN --RollBack in case of Error

	-- you can Raise ERROR with RAISEERROR() Statement including the details of the exception

		DECLARE @errorMessage varchar(5000) = ERROR_MESSAGE()
		RAISERROR(@errorMessage, 11, 1)
	END CATCH
	