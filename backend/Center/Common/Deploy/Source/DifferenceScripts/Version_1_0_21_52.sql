 

USE Gorba_CenterOnline
GO

-- =============================================
-- RENAME THE ItcsFilters TABLE INTO ItcsFilterSet inside a sql transaction
-- =============================================
BEGIN TRY
	BEGIN TRANSACTION
		-- Disable all table constraints
		ALTER TABLE [dbo].[ItcsFilters] NOCHECK CONSTRAINT ALL

		-- Rename table
		EXEC sp_rename 'ItcsFilters', 'ItcsFilterSet'

		-- Enable all table constraints	
		ALTER TABLE [dbo].[ItcsFilterSet] CHECK CONSTRAINT ALL

		COMMIT TRAN -- Transaction Success!
END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN --RollBack in case of Error

	-- you can Raise ERROR with RAISEERROR() Statement including the details of the exception

		DECLARE @errorMessage varchar(5000) = ERROR_MESSAGE()
		RAISERROR(@errorMessage, 11, 1)
END CATCH

-- =============================================
-- Change the IX_UserSetUserName to non-unique to unique
-- =============================================
DROP INDEX [IX_UserSetUserName] ON [dbo].[UserSet]

CREATE UNIQUE NONCLUSTERED INDEX [IX_UserSetUserName] ON [dbo].[UserSet]
(
	[Username] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- =============================================
-- Create new view on [ItcsFiltersSet] called ItcsFilters
-- =============================================
CREATE VIEW [dbo].[ItcsFilters]
AS
SELECT     Id, StopPointId, ItcsDisplayAreaId, Properties, LineReference, DirectionText, DirectionReference, LineText, IsActive
FROM         dbo.ItcsFilterSet
WHERE     (IsActive = 1)
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.21.52'
  ,@description = 'Rename table ItcsFilters to ItcsFilterSet, add view ItcsFilters, make index UserSetUsername unique.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 21
  ,@versionRevision = 52
  ,@dateCreated = '2012-09-25T09:45:00.000'
GO
