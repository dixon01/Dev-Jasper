USE [Gorba_CenterControllers]
GO

/****** Object:  StoredProcedure [dbo].[SetState]    Script Date: 01/14/2013 12:44:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SetState]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SetState]
GO

/****** Object:  View [dbo].[ActivityTaskLifecycles]    Script Date: 01/14/2013 12:44:57 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ActivityTaskLifecycles]'))
DROP VIEW [dbo].[ActivityTaskLifecycles]
GO

/****** Object:  View [dbo].[ActivityControllerStates]    Script Date: 01/14/2013 12:45:41 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ActivityControllerStates]'))
DROP VIEW [dbo].[ActivityControllerStates]
GO

/****** Object:  View [dbo].[ActivityControllers]    Script Date: 01/14/2013 12:46:19 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ActivityControllers]'))
DROP VIEW [dbo].[ActivityControllers]
GO

/****** Object:  StoredProcedure [dbo].[ClearInstances]    Script Date: 01/14/2013 12:47:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 17.01.12
-- Description:	Clears instances from the database
-- =============================================
ALTER PROCEDURE [dbo].[ClearInstances]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @id bigint
	DECLARE @RC int
	DECLARE @text varchar(1000)
	
	DECLARE instanceCursor CURSOR READ_ONLY
	FOR
	SELECT [SurrogateInstanceId]
	FROM [System.Activities.DurableInstancing].[InstancesTable]


	OPEN instanceCursor

	FETCH NEXT FROM instanceCursor
	INTO @id

	WHILE @@FETCH_STATUS = 0
	BEGIN
	

		EXECUTE @RC = [Gorba_CenterControllers].[System.Activities.DurableInstancing].[DeleteInstance] 
			@surrogateInstanceId = @id

		SET @text = 'Deleted instance ' + CONVERT(varchar(100), @id)
		
		print @text

		FETCH NEXT FROM instanceCursor
		INTO @id

	END

	CLOSE instanceCursor
	DEALLOCATE instanceCursor
	
END

GO

USE [Gorba_CenterControllers]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ActivityInstanceControllerSet_ActivityTaskLifecycleSet]') AND parent_object_id = OBJECT_ID(N'[dbo].[ActivityInstanceControllerSet]'))
ALTER TABLE [dbo].[ActivityInstanceControllerSet] DROP CONSTRAINT [FK_ActivityInstanceControllerSet_ActivityTaskLifecycleSet]
GO

/****** Object:  Table [dbo].[ActivityInstanceControllerSet]    Script Date: 01/14/2013 12:49:09 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActivityInstanceControllerSet]') AND type in (N'U'))
DROP TABLE [dbo].[ActivityInstanceControllerSet]
GO

/****** Object:  Table [dbo].[ActivityTaskLifecycleSet]    Script Date: 01/14/2013 12:50:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActivityTaskLifecycleSet]') AND type in (N'U'))
DROP TABLE [dbo].[ActivityTaskLifecycleSet]
GO

/****** Object:  Table [dbo].[ActivityControllerStateSet]    Script Date: 01/14/2013 12:51:01 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActivityControllerStateSet]') AND type in (N'U'))
DROP TABLE [dbo].[ActivityControllerStateSet]
GO

/****** Object:  Table [dbo].[DatabaseVersionSet]    Script Date: 01/28/2013 16:04:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[DatabaseVersionSet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](500) NULL,
	[VersionMajor] [int] NOT NULL,
	[VersionMinor] [int] NOT NULL,
	[VersionBuild] [int] NOT NULL,
	[VersionRevision] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_DatabaseVersionSet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  StoredProcedure [dbo].[DatabaseVersion_Insert]    Script Date: 01/28/2013 16:05:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 02.12.11 10:47
-- Description:	Inserts a new version of the database in the DatabaseVersionSet table.
--				Everything is done within a transaction.
--				If the @dateCreated parameter is NULL, then the actual UTC time is used.
--				If the operations succeeds, the identifier assigned to the created row is selected
-- =============================================
CREATE PROCEDURE [dbo].[DatabaseVersion_Insert]
(
	@name varchar(100),
	@description varchar(500) = NULL,
	@versionMajor int,
	@versionMinor int,
	@versionBuild int,
	@versionRevision int,
	@dateCreated datetime2 = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION DatabaseVersion_InsertTx -- Start the transaction..
			IF @dateCreated IS NULL
				BEGIN
					SET @dateCreated = GETUTCDATE()
				END
			INSERT INTO [DatabaseVersionSet]
			([Name], [Description], [VersionMajor], [VersionMinor], [VersionBuild], [VersionRevision], [DateCreated])
			VALUES
			(@name, @description, @versionMajor, @versionMinor, @versionBuild, @versionRevision, @dateCreated)
			
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




--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterControllers].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.1.29.1'
  ,@description = 'Refactoring for the code quality. Removed unused stored procedures. Added versioning'
  ,@versionMajor = 1
  ,@versionMinor = 1
  ,@versionBuild = 29
  ,@versionRevision = 1
  ,@dateCreated = '2013-01-28 14:30:00'
GO