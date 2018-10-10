 

USE Gorba_CenterOnline
GO

CREATE TABLE [dbo].[AlarmSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[AlarmStatusTypeId] [int] NOT NULL,
	[AlarmTypeId] [int] NOT NULL,
	[UnitId] [int] NOT NULL,
	[UserId] [int] NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ConfirmationText] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateAlarmEnd] [datetime] NULL,
	[DateConfirmed] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	CONSTRAINT [PK_AlarmSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AlarmTypeSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[AlarmCategoryId] [int] NOT NULL,
	[SeverityId] [int] NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	CONSTRAINT [PK_AlarmTypeSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AlarmStatusTypeSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	CONSTRAINT [PK_AlarmStatusTypeSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AlarmCategorySet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	CONSTRAINT [PK_AlarmCategorySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SeveritySet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	CONSTRAINT [PK_SeveritySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE VIEW [dbo].[AlarmStatusTypes]
AS
SELECT     Id, Name, Description, DateCreated, DateModified
FROM         dbo.AlarmStatusTypeSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[Alarms]
AS
SELECT     Id, AlarmStatusTypeId, AlarmTypeId, UnitId, UserId, Description, ConfirmationText, DateCreated, DateAlarmEnd, DateConfirmed
FROM         dbo.AlarmSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[Severities]
AS
SELECT     Id, Name, Description, DateCreated, DateModified
FROM         dbo.SeveritySet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[AlarmCategories]
AS
SELECT     Id, Name, Description, DateCreated, DateModified
FROM         dbo.AlarmCategorySet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[AlarmTypes]
AS
SELECT     Id, AlarmCategoryId, SeverityId, Name, Description, DateCreated, DateModified
FROM         dbo.AlarmTypeSet
WHERE     (IsDeleted = 0)
GO
ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [FK_AlarmSet_AlarmStatusTypeSet] FOREIGN KEY
	(
		[AlarmStatusTypeId]
	)
	REFERENCES [dbo].[AlarmStatusTypeSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [FK_AlarmSet_AlarmTypeSet] FOREIGN KEY
	(
		[AlarmTypeId]
	)
	REFERENCES [dbo].[AlarmTypeSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [FK_AlarmSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [FK_AlarmSet_UserSet] FOREIGN KEY
	(
		[UserId]
	)
	REFERENCES [dbo].[UserSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AlarmTypeSet] ADD CONSTRAINT [FK_AlarmTypeSet_AlarmCategorySet] FOREIGN KEY
	(
		[AlarmCategoryId]
	)
	REFERENCES [dbo].[AlarmCategorySet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AlarmTypeSet] ADD CONSTRAINT [FK_AlarmTypeSet_SeveritySet] FOREIGN KEY
	(
		[SeverityId]
	)
	REFERENCES [dbo].[SeveritySet]
	(
		[Id]
	)
GO

-- =============================================
-- Author:		Thomas Epple (ept@gorba.com)
-- Create date: 2012-01-09
-- Description:	Adds an Alarm
-- =============================================
CREATE PROCEDURE [dbo].[Alarm_Insert]
(
	@alarmStatusTypeId int,
	@alarmTypeId int,
	@unitId int,
	@userId int = NULL,
	@description varchar(500),
	@confirmationText varchar(500),
	@dateAlarmEnd datetime,
	@dateCreated datetime = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Alarm_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [AlarmSet]
			([AlarmStatusTypeId]
			, [AlarmTypeId]
			, [UnitId]
			, [UserId]
			, [Description]
			, [ConfirmationText]
			, [DateAlarmEnd]
			, [DateCreated])
			VALUES
			(@alarmStatusTypeId
			, @alarmTypeId
			, @unitId
			, @userId
			, @description
			, @confirmationText
			, @dateAlarmEnd
			, @dateCreated)
			
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

-- =============================================
-- Author:		Thomas Epple (ept@gorba.com)
-- Create date: 12-01-09
-- Description:	Adds an AlarmStatusType
-- =============================================
CREATE PROCEDURE [dbo].[AlarmStatusType_Insert]
(
	@name varchar(100),
	@description varchar(500),
	@dateCreated datetime = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AlarmStatusType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [AlarmStatusTypeSet]
			([Name]
			, [Description]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @dateCreated)
			
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
-- =============================================
-- Author:		Thomas Epple (ept@gorba.com)
-- Create date: 12-01-09
-- Description:	Adds an AlarmType
-- =============================================
CREATE PROCEDURE [dbo].[AlarmType_Insert]
(
	@alarmCategoryId int,
	@severityId int,
	@description varchar(500),
	@name varchar(100),
	@dateCreated datetime = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AlarmType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [AlarmTypeSet]
			([AlarmCategoryId]
			, [SeverityId]
			, [Description]
			, [Name]
			, [DateCreated])
			VALUES
			(@alarmCategoryId
			, @severityId
			, @description
			, @name
			, @dateCreated)
			
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

-- =============================================
-- Author:		Thomas Epple (ept@gorba.com)
-- Create date: 12-01-09
-- Description:	Adds an AlarmCategory
-- =============================================
CREATE PROCEDURE [dbo].[AlarmCategory_Insert]
(
	@name varchar(100),
	@description varchar(500),
	@dateCreated datetime = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AlarmCategory_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [AlarmCategorySet]
			([Name]
			, [Description]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @dateCreated)
			
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

-- =============================================
-- Author:		Thomas Epple (ept@gorba.com)
-- Create date: 12-01-09
-- Description:	Adds a Severity
-- =============================================
CREATE PROCEDURE [dbo].[Severity_Insert]
(
	@name varchar(100),
	@description varchar(500),
	@dateCreated datetime = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Severity_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [SeveritySet]
			([Name]
			, [Description]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @dateCreated)
			
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

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.2.8'
  ,@description = 'Added tables for alarms.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 2
  ,@versionRevision = 8
  ,@dateCreated = '2012-01-10T10:10:00.000'
GO