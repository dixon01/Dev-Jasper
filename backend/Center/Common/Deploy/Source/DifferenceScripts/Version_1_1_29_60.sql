USE [Gorba_CenterOnline]
GO

/****** Object:  StoredProcedure [dbo].[InfoLineTextActivity_Delete]    Script Date: 01/14/2013 13:00:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InfoLineTextActivity_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InfoLineTextActivity_Delete]
GO

/****** Object:  StoredProcedure [dbo].[Activity_Delete]    Script Date: 01/14/2013 13:01:08 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Activity_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Activity_Delete]
GO

/****** Object:  StoredProcedure [dbo].[Activity_SelectByOperation]    Script Date: 01/14/2013 13:03:07 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Activity_SelectByOperation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Activity_SelectByOperation]
GO

/****** Object:  StoredProcedure [dbo].[Alarm_Insert]    Script Date: 01/14/2013 13:03:31 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Alarm_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Alarm_Insert]
GO

/****** Object:  StoredProcedure [dbo].[AlarmCategory_Insert]    Script Date: 01/14/2013 13:04:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlarmCategory_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AlarmCategory_Insert]
GO

/****** Object:  StoredProcedure [dbo].[AlarmStatusType_Insert]    Script Date: 01/14/2013 13:04:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlarmStatusType_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AlarmStatusType_Insert]
GO

/****** Object:  StoredProcedure [dbo].[AlarmType_Insert]    Script Date: 01/14/2013 13:05:01 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlarmType_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AlarmType_Insert]
GO

/****** Object:  StoredProcedure [dbo].[AssociationPermissionDataScopeUserRole_Insert]    Script Date: 01/14/2013 13:07:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AssociationPermissionDataScopeUserRole_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AssociationPermissionDataScopeUserRole_Insert]
GO

/****** Object:  StoredProcedure [dbo].[AssociationTenantUserUserRole_Insert]    Script Date: 01/14/2013 13:07:57 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AssociationTenantUserUserRole_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AssociationTenantUserUserRole_Insert]
GO

/****** Object:  StoredProcedure [dbo].[AssociationUnitOperation_Delete]    Script Date: 01/14/2013 13:08:08 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AssociationUnitOperation_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AssociationUnitOperation_Delete]
GO

/****** Object:  StoredProcedure [dbo].[AssociationUnitOperation_Insert]    Script Date: 01/14/2013 13:08:29 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AssociationUnitOperation_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AssociationUnitOperation_Insert]
GO

/****** Object:  StoredProcedure [dbo].[DataScope_Insert]    Script Date: 01/14/2013 13:10:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataScope_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DataScope_Insert]
GO

/****** Object:  StoredProcedure [dbo].[Operation_FindByName]    Script Date: 01/14/2013 13:10:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Operation_FindByName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Operation_FindByName]
GO

/****** Object:  StoredProcedure [dbo].[Operation_Insert]    Script Date: 01/14/2013 13:11:01 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Operation_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Operation_Insert]
GO

/****** Object:  StoredProcedure [dbo].[Operation_ListByUnit]    Script Date: 01/14/2013 13:11:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Operation_ListByUnit]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Operation_ListByUnit]
GO

/****** Object:  StoredProcedure [dbo].[Operation_Select]    Script Date: 01/14/2013 13:11:57 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Operation_Select]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Operation_Select]
GO

/****** Object:  StoredProcedure [dbo].[Operation_Update]    Script Date: 01/14/2013 13:12:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Operation_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Operation_Update]
GO

/****** Object:  StoredProcedure [dbo].[Permission_Insert]    Script Date: 01/14/2013 13:13:43 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permission_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Permission_Insert]
GO

/****** Object:  StoredProcedure [dbo].[ProductType_Insert]    Script Date: 01/14/2013 13:14:40 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductType_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ProductType_Insert]
GO

/****** Object:  StoredProcedure [dbo].[ProtocolType_Insert]    Script Date: 01/14/2013 13:15:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProtocolType_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ProtocolType_Insert]
GO

/****** Object:  StoredProcedure [dbo].[Severity_Insert]    Script Date: 01/14/2013 13:15:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Severity_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Severity_Insert]
GO

/****** Object:  StoredProcedure [dbo].[Tenant_Insert]    Script Date: 01/14/2013 13:15:36 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tenant_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Tenant_Insert]
GO

/****** Object:  StoredProcedure [dbo].[Unit_SelectAssociatedOperations]    Script Date: 01/14/2013 13:15:59 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Unit_SelectAssociatedOperations]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Unit_SelectAssociatedOperations]
GO

/****** Object:  StoredProcedure [dbo].[Unit_SelectByTenant]    Script Date: 01/14/2013 13:16:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Unit_SelectByTenant]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Unit_SelectByTenant]
GO

/****** Object:  StoredProcedure [dbo].[Unit_SelectByTenantOfUser]    Script Date: 01/14/2013 13:16:41 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Unit_SelectByTenantOfUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Unit_SelectByTenantOfUser]
GO

/****** Object:  StoredProcedure [dbo].[UnitGroup_Insert]    Script Date: 01/14/2013 13:17:01 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UnitGroup_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UnitGroup_Insert]
GO

/****** Object:  StoredProcedure [dbo].[UnitGroupType_Insert]    Script Date: 01/14/2013 13:17:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UnitGroupType_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UnitGroupType_Insert]
GO

/****** Object:  StoredProcedure [dbo].[UnitType_Insert]    Script Date: 01/14/2013 13:17:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UnitType_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UnitType_Insert]
GO

/****** Object:  StoredProcedure [dbo].[User_Insert]    Script Date: 01/14/2013 13:18:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[User_Insert]
GO

/****** Object:  StoredProcedure [dbo].[UserRole_Insert]    Script Date: 01/14/2013 13:18:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRole_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserRole_Insert]
GO

USE [Gorba_CenterOnline]
GO

/****** Object:  View [dbo].[AlarmTypes]    Script Date: 01/14/2013 13:21:24 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[AlarmTypes]'))
DROP VIEW [dbo].[AlarmTypes]
GO

/****** Object:  View [dbo].[AlarmStatusTypes]    Script Date: 01/14/2013 13:21:48 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[AlarmStatusTypes]'))
DROP VIEW [dbo].[AlarmStatusTypes]
GO
/****** Object:  View [dbo].[AlarmCategories]    Script Date: 01/14/2013 13:22:07 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[AlarmCategories]'))
DROP VIEW [dbo].[AlarmCategories]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_AlarmStatusTypeSet_IsDeleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AlarmStatusTypeSet] DROP CONSTRAINT [DF_AlarmStatusTypeSet_IsDeleted]
END

GO

/****** Object:  Table [dbo].[AlarmStatusTypeSet]    Script Date: 01/14/2013 13:23:15 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlarmStatusTypeSet]') AND type in (N'U'))
DROP TABLE [dbo].[AlarmStatusTypeSet]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AlarmTypeSet_AlarmCategorySet]') AND parent_object_id = OBJECT_ID(N'[dbo].[AlarmTypeSet]'))
ALTER TABLE [dbo].[AlarmTypeSet] DROP CONSTRAINT [FK_AlarmTypeSet_AlarmCategorySet]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AlarmTypeSet_SeveritySet]') AND parent_object_id = OBJECT_ID(N'[dbo].[AlarmTypeSet]'))
ALTER TABLE [dbo].[AlarmTypeSet] DROP CONSTRAINT [FK_AlarmTypeSet_SeveritySet]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_AlarmTypeSet_IsDeleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AlarmTypeSet] DROP CONSTRAINT [DF_AlarmTypeSet_IsDeleted]
END

GO

/****** Object:  Table [dbo].[AlarmTypeSet]    Script Date: 01/14/2013 13:23:43 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlarmTypeSet]') AND type in (N'U'))
DROP TABLE [dbo].[AlarmTypeSet]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_AlarmCategorySet_IsDeleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AlarmCategorySet] DROP CONSTRAINT [DF_AlarmCategorySet_IsDeleted]
END

GO

/****** Object:  Table [dbo].[AlarmCategorySet]    Script Date: 01/14/2013 13:24:11 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlarmCategorySet]') AND type in (N'U'))
DROP TABLE [dbo].[AlarmCategorySet]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DirectionReferences_Directions]') AND parent_object_id = OBJECT_ID(N'[dbo].[DirectionReferences]'))
ALTER TABLE [dbo].[DirectionReferences] DROP CONSTRAINT [FK_DirectionReferences_Directions]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DirectionReferences_ItcsProviders]') AND parent_object_id = OBJECT_ID(N'[dbo].[DirectionReferences]'))
ALTER TABLE [dbo].[DirectionReferences] DROP CONSTRAINT [FK_DirectionReferences_ItcsProviders]
GO

/****** Object:  Table [dbo].[DirectionReferences]    Script Date: 01/14/2013 13:24:52 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DirectionReferences]') AND type in (N'U'))
DROP TABLE [dbo].[DirectionReferences]
GO

/****** Object:  Table [dbo].[Directions]    Script Date: 01/14/2013 13:25:08 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Directions]') AND type in (N'U'))
DROP TABLE [dbo].[Directions]
GO

/****** Object:  UserDefinedFunction [dbo].[MD5]    Script Date: 01/14/2013 13:26:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MD5]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[MD5]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItcsDisplayAreas]') AND type in (N'U'))
EXEC sp_rename 'ItcsDisplayAreas', 'ItcsDisplayAreaSet'
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItcsProviders]') AND type in (N'U'))
EXEC sp_rename 'ItcsProviders', 'ItcsProviderSet'
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SwitchDisplayStateActivitySet]') AND type in (N'U'))
DROP TABLE [SwitchDisplayStateActivitySet]
GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ItcsDisplayAreas]'))
DROP VIEW [dbo].[ItcsDisplayAreas]
GO

/****** Object:  View [dbo].[ItcsDisplayAreas]    Script Date: 01/14/2013 14:18:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Create new view on [ItcsDisplayAreaSet] called ItcsDisplayAreas
-- =============================================
CREATE VIEW [dbo].[ItcsDisplayAreas]
AS
SELECT [Id]
      ,[ProviderId]
      ,[Name]
      ,[Description]
      ,[Properties]
  FROM [Gorba_CenterOnline].[dbo].[ItcsDisplayAreaSet]


GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ItcsProviders]'))
DROP VIEW [dbo].[ItcsProviders]
GO

/****** Object:  View [dbo].[ItcsProviders]    Script Date: 01/14/2013 14:18:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Create new view on [ItcsProviderSet] called ItcsProviders
-- =============================================
CREATE VIEW [dbo].[ItcsProviders]
AS
SELECT [Id]
      ,[Name]
      ,[Description]
      ,[IsDeleted]
      ,[Properties]
      ,[ProtocolTypeId]
      ,[ReferenceId]
  FROM [Gorba_CenterOnline].[dbo].[ItcsProviderSet]


GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UnitStopPointAssociations]') AND type in (N'U'))
EXEC sp_rename 'UnitStopPointAssociations', 'AssociationUnitStopPointSet'
GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[AssociationsUnitStopPoint]'))
DROP VIEW [dbo].[AssociationsUnitStopPoint]
GO

/****** Object:  View [dbo].[AssociationsUnitStopPointSet]    Script Date: 01/14/2013 14:18:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Create new view on [AssociationUnitStopPointSet] called ItcsProviders
-- =============================================
CREATE VIEW [dbo].[AssociationsUnitStopPoint]
AS
SELECT [UnitId]
      ,[StopPointId]
  FROM [Gorba_CenterOnline].[dbo].[AssociationUnitStopPointSet]

GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[AssociationUnitUnitGroup]'))
EXEC sp_rename 'AssociationUnitUnitGroup', 'AssociationsUnitUnitGroup'
GO



--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.1.29.60'
  ,@description = 'Refactoring for the code quality. Removed unused stored procedures and tables. Fixed naming according to conventions already used/defined.'
  ,@versionMajor = 1
  ,@versionMinor = 1
  ,@versionBuild = 29
  ,@versionRevision = 60
  ,@dateCreated = '2013-01-28T14:30:00.000'
GO