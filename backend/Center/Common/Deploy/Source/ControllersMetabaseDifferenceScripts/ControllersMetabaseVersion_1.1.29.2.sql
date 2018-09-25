USE [Gorba_CenterControllersMetabase]
GO

/****** Object:  StoredProcedure [dbo].[AddOrUpdateActivityInstanceController]    Script Date: 01/14/2013 12:54:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddOrUpdateActivityInstanceController]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AddOrUpdateActivityInstanceController]
GO

/****** Object:  StoredProcedure [dbo].[AddOrUpdateActivityTaskLifecycle]    Script Date: 01/14/2013 12:54:46 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddOrUpdateActivityTaskLifecycle]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AddOrUpdateActivityTaskLifecycle]
GO

/****** Object:  StoredProcedure [dbo].[FindCurrentActivityTaskIdentifier]    Script Date: 01/14/2013 12:55:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FindCurrentActivityTaskIdentifier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FindCurrentActivityTaskIdentifier]
GO

/****** Object:  StoredProcedure [dbo].[GetActivityInstanceController]    Script Date: 01/14/2013 12:55:46 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetActivityInstanceController]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetActivityInstanceController]
GO


--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterControllersMetabase].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.1.29.2'
  ,@description = 'Refactoring for the code quality. Removed unused stored procedures.'
  ,@versionMajor = 1
  ,@versionMinor = 1
  ,@versionBuild = 29
  ,@versionRevision = 2
  ,@dateCreated = '2013-01-28 14:30:00'
GO