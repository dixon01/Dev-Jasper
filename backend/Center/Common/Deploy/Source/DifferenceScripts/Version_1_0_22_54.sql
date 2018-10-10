USE Gorba_CenterOnline
GO

DROP PROCEDURE [dbo].[UnitGroup_SelectAssociatedUnits]
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.22.54'
  ,@description = 'Remove the UnitGroup_SelectAssociatedUnits stored procedure because we don t need it and contained a fixed value.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 22
  ,@versionRevision = 54
  ,@dateCreated = '2012-10-15T00:00:00.000'
GO