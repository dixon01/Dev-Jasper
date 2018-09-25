USE [Gorba_CenterOnline]
GO

ALTER TABLE [Gorba_CenterOnline].[dbo].[AssociationTenantUserUserRoleSet]
ALTER COLUMN [TenantId] INT NULL

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.3.41.63'
  ,@description = 'Tenant is now nullable in the association with users and user roles.'
  ,@versionMajor = 1
  ,@versionMinor = 3
  ,@versionBuild = 41
  ,@versionRevision = 63
  ,@dateCreated = '2013-07-16T09:00:00.000'
GO