 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[UnitSet] ADD 
[IsOnline] [bit] NOT NULL CONSTRAINT [DF_UnitSet_IsOnline] DEFAULT ((0)),
[LastSeenOnline] [datetime] NULL
GO
ALTER TABLE [dbo].[UserSet] ADD 
[Culture] [varchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER VIEW [dbo].[Units]
AS
SELECT     Id, TenantId, ProductTypeId, GroupId, LayoutId, Name, ShortName, SystemName, SerialNumber, NetworkAddress, Description, IsOnline, LastSeenOnline, DateCreated, 
                      DateModified
FROM         dbo.UnitSet
WHERE     (IsDeleted = 0)
GO
ALTER VIEW [dbo].[Users]
AS
SELECT     Id, Username, Culture, DateCreated, DateModified
FROM         dbo.UserSet
WHERE     (IsDeleted = 0)
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 14.02.2012
-- Description:	Selects Operations by the userId. If the flag ignoreTenant is not set
--				it selects all operations of users related to the tenantId.
-- =============================================
ALTER PROCEDURE [dbo].[Operation_SelectByUser] 
	(@userId int = NULL,
	@tenantId int = NULL,
	@ignoreTenant bit = 0
	)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF @userId IS NULL AND @tenantId IS NULL
	BEGIN
		SELECT *
		FROM [Operations]
	END
	ELSE
	BEGIN
		IF @ignoreTenant = 0
		BEGIN
			SELECT [O].*
			FROM [Operations] [O]
			WHERE [O].UserId IN 
			(SELECT [A].UserId FROM [AssociationsTenantUserUserRole] [A]
			 WHERE [A].TenantId = @tenantId)
		END
		ELSE
		BEGIN
			SELECT [O].*
			FROM [Operations] [O]
			WHERE [O].UserId = @userId
		END	
	END	
END
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[User_Insert]
(
	@username varchar(100),
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
			, [Culture]
			, [DateCreated])
			VALUES
			(@username
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
CREATE PROCEDURE [dbo].[Unit_Update]
	(@id int,
	@tenantId int,
	@productTypeId int,
	@groupId int = NULL,
	@layoutId int = NULL,
	@name varchar(100),
	@shortName varchar(50) = NULL,
	@systemName varchar(100) = NULL,
	@serialNumber varchar(50) = NULL,
	@description varchar(500) = NULL,
	@networkAddress varchar(256) = NULL,
	@isOnline bit = 0,
	@lastSeenOnline datetime = NULL,
	@dateModified datetime = NULL)
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
			, [Name] = @name
			, [ShortName] = @shortName
			, [SystemName] = @systemName
			, [SerialNumber] = @serialNumber
			, [NetworkAddress] = @networkAddress
			, [Description] = @description
			, [IsOnline] = @isOnline
			, [LastSeenOnline] = @lastSeenOnline
			, [DateModified] = @dateModified
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

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.5.16'
  ,@description = 'Added properties IsOnline, LastSeenOnline, Culture and modified related SP. Add SP Unit_Update.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 5
  ,@versionRevision = 16
  ,@dateCreated = '2012-02-15T14:41:00.000'
GO
