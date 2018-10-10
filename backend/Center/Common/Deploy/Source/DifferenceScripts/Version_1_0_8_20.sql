 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[AssociationTenantUserUserRoleSet] DROP CONSTRAINT [FK_AssociationTenantUserUserRoleSet_UserSet]
GO
ALTER TABLE [dbo].[AlarmSet] DROP CONSTRAINT [FK_AlarmSet_UserSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [FK_OperationSet_UserSet]
GO
ALTER TABLE [dbo].[UserSet] DROP CONSTRAINT [DF_UserSet_IsDeleted]
GO
ALTER TABLE [dbo].[UserSet] DROP CONSTRAINT [PK_UserSet]
GO
DROP INDEX [IX_UserSetUserName] ON [dbo].[UserSet]
GO
ALTER TABLE [dbo].[UnitSet] ALTER COLUMN [Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO
ALTER TABLE [dbo].[UnitSet] ADD 
[StationId] [int] NULL
GO
ALTER TABLE [dbo].[StationSet] ADD 
[ReferenceName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[LocationName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[TimeZoneId] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
CREATE TABLE [dbo].[AssociationUnitLineSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UnitId] [int] NOT NULL,
	[LineId] [int] NOT NULL,
	[LaneReferenceName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DirectionReferenceName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT [PK_AssociationUnitLineSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LineSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ShortName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ReferenceName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsDeleted] [bit] NOT NULL,
	CONSTRAINT [PK_LineSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TempUserSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Username] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[HashedPassword] [char] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LastName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Email] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_UserSet_IsDeleted] DEFAULT ((0)),
	[Culture] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempUserSet] ON
INSERT INTO [dbo].[TempUserSet] ([Id],[Username],[DateCreated],[DateModified],[IsDeleted],[Culture],[HashedPassword]) SELECT [Id],[Username],[DateCreated],[DateModified],[IsDeleted],[Culture],'' FROM [dbo].[UserSet]
SET IDENTITY_INSERT [dbo].[TempUserSet] OFF
GO

DROP TABLE [dbo].[UserSet]
GO
EXEC sp_rename N'[dbo].[TempUserSet]',N'UserSet', 'OBJECT'
GO


ALTER VIEW [dbo].[Stations]
AS
SELECT [Id]
      ,[Name]
      ,[Number]
      ,[Latitude]
      ,[Longitude]
      ,[UtcOffset]
      ,[DayLightSaving]
      ,[DateCreated]
      ,[DateModified]
      ,[ReferenceName]
      ,[LocationName]
      ,[Description]
      ,[TimeZoneId]

FROM         dbo.StationSet
WHERE     (IsDeleted = 0)
GO
ALTER VIEW [dbo].[Users]
AS
SELECT [Id]
      ,[Username]
      ,[Name]
      ,[LastName]
      ,[Email]
      ,[DateCreated]
      ,[DateModified]
      ,[Culture]
FROM         dbo.UserSet
WHERE     (IsDeleted = 0)
GO
ALTER VIEW [dbo].[Units]
AS
SELECT [Id]
      ,[TenantId]
      ,[ProductTypeId]
      ,[GroupId]
      ,[LayoutId]
      , [StationId]
      ,[Name]
      ,[ShortName]
      ,[SystemName]
      ,[SerialNumber]
      ,[Description]
      ,[DateCreated]
      ,[DateModified]
      ,[NetworkAddress]
      ,[IsOnline]
      ,[LastSeenOnline]
      ,[LocationName]
      ,[AlarmStatus]
      ,[CommunicationStatus]
      ,[OperationStatus]
      ,[LastRestartRequestDate]
      ,[LastTimeSyncRequestDate]
      ,[LastTimeSyncValue]
      , [TimeZoneInfoId]

FROM         dbo.UnitSet
WHERE     (IsDeleted = 0)
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[User_Insert]
(
	@username varchar(100),
	@hashedPassword char(32),
	@name varchar(100),
	@lastName varchar(100),
	@email varchar(256),
	@culture varchar(5) = NULL,
	@dateCreated datetime = NULL	
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION User_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [UserSet]
			([Username]
			, [HashedPassword]
			, [Name]
			, [LastName]
			, [Email]
			, [Culture]
			, [DateCreated])
			VALUES
			(@username
			, @hashedPassword
			, @name
			, @lastName
			, @email
			, @culture
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
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 14.02.2012
-- Description:	Updates a Unit
-- =============================================
ALTER PROCEDURE [dbo].[Unit_Update]
	(@id int,
	@tenantId int,
	@productTypeId int,
	@groupId int = NULL,
	@layoutId int = NULL
	, @stationId int
	, @name varchar(100),
	@shortName varchar(50) = NULL,
	@systemName varchar(100) = NULL,
	@serialNumber varchar(50) = NULL,
	@description varchar(500) = NULL,
	@networkAddress varchar(256) = NULL,
	@isOnline bit = 0,
	@lastSeenOnline datetime = NULL,
	@dateModified datetime = NULL
	, @locationName varchar(100)
	, @alarmStatus int
	, @communicationStatus int
	, @operationStatus int
	, @lastRestartRequestDate datetime
	, @lastTimeSyncRequestDate datetime
	, @lastTimeSyncValue datetime
	, @timeZoneInfoId varchar(100))
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Unit_EditTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateModified IS NULL
			BEGIN
				SET @dateModified = GETUTCDATE()
			END
			
			UPDATE [UnitSet] 
			SET [TenantId] = @tenantId
			, [ProductTypeId] = @productTypeId
			, [GroupId] = @groupId
			, [LayoutId] = @layoutId
			, [StationId] = @stationId
			, [Name] = @name
			, [ShortName] = @shortName
			, [SystemName] = @systemName
			, [SerialNumber] = @serialNumber
			, [NetworkAddress] = @networkAddress
			, [Description] = @description
			, [IsOnline] = @isOnline
			, [LastSeenOnline] = @lastSeenOnline
			, [DateModified] = @dateModified
			, [LocationName] = @locationName
			, [AlarmStatus] = @alarmStatus
			, [CommunicationStatus] = @communicationStatus
			, [OperationStatus] = @operationStatus
			, [LastRestartRequestDate] = @lastRestartRequestDate
			, [LastTimeSyncRequestDate] = @lastTimeSyncRequestDate
			, [LastTimeSyncValue] = @lastTimeSyncValue
			, [TimeZoneInfoId] = @timeZoneInfoId
			WHERE [Id] = @id
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
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 05.12.11 15:40
-- Description:	Adds a unit to the system
-- =============================================
ALTER PROCEDURE [dbo].[Unit_Insert]
(
	@tenantId int,
	@productTypeId int,
	@groupId int = NULL,
	@layoutId int = NULL
	, @stationId int
	, @name varchar(100),
	@shortName varchar(50) = NULL,
	@systemName varchar(100) = NULL,
	@serialNumber varchar(50) = NULL,
	@networkAddress varchar(256) = NULL,
	@description varchar(500) = NULL,
	@dateCreated datetime = NULL
	, @locationName varchar(100)
	, @alarmStatus int
	, @communicationStatus int
	, @operationStatus int
	, @lastRestartRequestDate datetime
	, @lastTimeSyncRequestDate datetime
	, @lastTimeSyncValue datetime
	, @timeZoneInfoId varchar(100)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Unit_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [UnitSet]
			([TenantId]
			, [ProductTypeId]
			, [GroupId]
			, [LayoutId]
			, [StationId]
			, [Name]
			, [ShortName]
			, [SystemName]
			, [SerialNumber]
			, [NetworkAddress]
			, [Description]
			, [DateCreated]
			, [LocationName]
			, [AlarmStatus]
			, [CommunicationStatus]
			, [OperationStatus]
			, [LastRestartRequestDate]
			, [LastTimeSyncRequestDate]
			, [LastTimeSyncValue]
			, [TimeZoneInfoId])
			VALUES
			(@tenantId
			, @productTypeId
			, @groupId
			, @layoutId
			, @stationId
			, @name
			, @shortName
			, @systemName
			, @serialNumber
			, @networkAddress
			, @description
			, @dateCreated
			, @locationName
			, @alarmStatus
			, @communicationStatus
			, @operationStatus
			, @lastRestartRequestDate
			, @lastTimeSyncRequestDate
			, @lastTimeSyncValue
			, @timeZoneInfoId)
			
			DECLARE @id int = SCOPE_IDENTITY()
			SELECT @id
			
			COMMIT TRAN -- Transaction Success!

	END TRY

	BEGIN CATCH


		IF @@TRANCOUNT > 0
			BEGIN
				ROLLBACK TRAN --RollBack in case of Error
			END
			
			DECLARE @line varchar(5) = ERROR_LINE()
			DECLARE @message varchar(500) = ERROR_MESSAGE()
			DECLARE @severity int = ERROR_SEVERITY()
			print 'Error on line ' + @line + ': ' + @message
			--RAISERROR(@message, @severity, 1)

	END CATCH
END
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 7.12.11 09:20
-- Description:	Adds a station to the system
-- =============================================
ALTER PROCEDURE [dbo].[Station_Insert] 
	(@name varchar(100),
	@number varchar(100),
	@latitude float,
	@longitude float,
	@utcOffset datetime,
	@daylightSaving bit = 0,
	@dateCreated datetime = NULL
	, @referenceName varchar(100)
	, @timeZoneId varchar(100)
	, @locationName varchar(100)
	, @description varchar(500)
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Station_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [StationSet]
			([Name]
			, [Number]
			, [Latitude]
			, [Longitude]
			, [UtcOffset]
			, [DayLightSaving]
			, [DateCreated]
			, [ReferenceName]
			, [TimeZoneId]
			, [LocationName]
			, [Description]
			)
			VALUES
			(@name ,
	@number ,
	@latitude ,
	@longitude ,
	@utcOffset ,
	@daylightSaving ,
	@dateCreated
	, @referenceName
	, @timeZoneId
	, @locationName
	, @description)
			
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
CREATE NONCLUSTERED INDEX [IX_UserSetUserName] ON [dbo].[UserSet]
(
	[Username] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE VIEW [dbo].[AssociationsUnitLine]
AS
SELECT [Id]
      ,[UnitId]
      ,[LineId]
      ,[LaneReferenceName]
      ,[DirectionReferenceName]
  FROM [dbo].[AssociationUnitLineSet]
GO
CREATE VIEW [dbo].[Lines]
AS
SELECT [Id]
      ,[Name]
      ,[ShortName]
      ,[Description]
      ,[ReferenceName]
  FROM [dbo].[LineSet]
  
WHERE     (IsDeleted = 0)
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[MD5] 
(
	-- Add the parameters for the function here
	@input varchar(100)
)
RETURNS char(32)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @md5 char(32) = CONVERT(char(32), HashBytes('MD5', @input), 2)

	-- Return the result of the function
	RETURN @md5

END
GO
ALTER TABLE [dbo].[UserSet] ADD CONSTRAINT [PK_UserSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
ALTER TABLE [dbo].[AssociationTenantUserUserRoleSet] ADD CONSTRAINT [FK_AssociationTenantUserUserRoleSet_UserSet] FOREIGN KEY
	(
		[UserId]
	)
	REFERENCES [dbo].[UserSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[AssociationUnitLineSet] ADD CONSTRAINT [FK_AssociationUnitLineSet_LineSet] FOREIGN KEY
	(
		[LineId]
	)
	REFERENCES [dbo].[LineSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AssociationUnitLineSet] ADD CONSTRAINT [FK_AssociationUnitLineSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_StationSet] FOREIGN KEY
	(
		[StationId]
	)
	REFERENCES [dbo].[StationSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[OperationSet] ADD CONSTRAINT [FK_OperationSet_UserSet] FOREIGN KEY
	(
		[UserId]
	)
	REFERENCES [dbo].[UserSet]
	(
		[Id]
	)
GO

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.8.20'
  ,@description = 'Added stations and lines. Other minor changes to users and units.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 8
  ,@versionRevision = 20
  ,@dateCreated = '2012-03-29T10:10:00.000'
GO