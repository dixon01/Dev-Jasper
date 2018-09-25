USE [Gorba_CenterControllersMetabase]
GO

/****** Object:  Table [dbo].[DatabaseVersionSet]    Script Date: 11/28/2012 07:24:18 ******/
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

/****** Object:  View [dbo].[DatabaseVersions]    Script Date: 11/28/2012 07:25:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[DatabaseVersions]
AS
SELECT     Id, Name, VersionMajor, VersionMinor, VersionBuild, VersionRevision, Description, DateCreated
FROM         dbo.DatabaseVersionSet

GO

USE [Gorba_CenterControllersMetabase]
GO

/****** Object:  StoredProcedure [dbo].[DatabaseVersion_Insert]    Script Date: 11/28/2012 07:32:44 ******/
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


ALTER TABLE [dbo].[ActivityInstanceControllerSet] ADD [ActivityInstanceId] int NOT NULL
GO
ALTER TABLE [dbo].[ActivityTaskLifecycleSet] ADD [ActivityInstanceId] int NOT NULL
GO

DROP INDEX [IX_ActivityInstanceController_UnitId_ActivityId_IsDeleted] ON [dbo].[ActivityInstanceControllerSet]
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterControllersMetabase].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 0.99.26.1'
  ,@description = 'Added database version management. Added ActivityInstanceId columns to ActivityInstanceControllerSet and ActivityTaskLifecycleSet.'
  ,@versionMajor = 0
  ,@versionMinor = 99
  ,@versionBuild = 26
  ,@versionRevision = 1
  ,@dateCreated = '2012-11-28 05:30:00'
GO
