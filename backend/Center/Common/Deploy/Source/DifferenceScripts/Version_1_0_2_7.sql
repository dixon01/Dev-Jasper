 

USE Gorba_CenterOnline
GO

CREATE TABLE [dbo].[AssociationPermissionDataScopeUserRoleSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[PermissionId] [int] NOT NULL,
	[DataScopeId] [int] NOT NULL,
	[UserRoleId] [int] NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_PermissionDataScopeUserRoleSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_AssociationPermissionDataScopeUserRoleSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AssociationTenantUserUserRoleSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[UserRoleId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_AssociationTenantUserUserRoleSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_AssociationTenantUserUserRoleSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DataScopeSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NULL CONSTRAINT [DF_DataScopeSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_DataScopeSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[PermissionSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NULL CONSTRAINT [DF_PermissionSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_PermissionSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[UserRoleSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_UserRoleSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_UserRoleSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 22.12.11
-- Description:	Deletes an operation and all its related activities
-- =============================================
ALTER PROCEDURE [dbo].[Operation_Delete]
(
	@id int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Operation_DeleteTx -- Start the transaction..
	
	-- If needed, it can be replaced with the usage of a cursor and the Activity_Delete operation.
	UPDATE [ActivitySet]
	SET [IsDeleted]=1
	WHERE [OperationId]=@id

	UPDATE [OperationSet]
	SET [Name] = [dbo].GetDeletedName([Id], [Name]), [IsDeleted]=1
	WHERE [Id]=@id
			
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
CREATE VIEW [dbo].[Permissions]
AS
SELECT     Id, Name, Description, DateCreated, DateModified
FROM         dbo.PermissionSet
GO
CREATE VIEW [dbo].[DataScopes]
AS
SELECT     Id, Name, Description, DateCreated, DateModified
FROM         dbo.DataScopeSet
GO
CREATE VIEW [dbo].[UserRoles]
AS
SELECT     Id, Name, Description, DateCreated, DateModified
FROM         dbo.UserRoleSet
WHERE     (IsDeleted = 0)
GO
-- =============================================
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 04.01.12
-- Description:	Gets the name for a deleted entity
-- =============================================
CREATE FUNCTION [dbo].[GetDeletedName] 
(
	@id int,
	@entityName varchar(100)
)
RETURNS varchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @len int = LEN(@entityName)
	DECLARE @size int = 8
	DECLARE @idString varchar(8000) = CONVERT(varchar(8000), @id)
	
	DECLARE @normalizedIdString char(8) = REPLICATE('0',@size-LEN(RTRIM(@idString))) + @idString

	DECLARE @finalLen int = 100 - @size - 1 -- I remove the '~' char too
	
	IF @finalLen > @len
		BEGIN
			SET @finalLen = @len
		END
	
	DECLARE @final varchar(100) = '~' + SUBSTRING(@entityName, 1, @finalLen) + @normalizedIdString

	-- Return the result of the function
	RETURN @final

END
GO
CREATE VIEW [dbo].[AssociationsTenantUserUserRole]
AS
SELECT     Id, TenantId, UserId, UserRoleId, DateCreated, DateModified
FROM         dbo.AssociationTenantUserUserRoleSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[AssociationsPermissionDataScopeUserRole]
AS
SELECT     Id, PermissionId, DataScopeId, UserRoleId, Name, DateCreated, DateModified
FROM         dbo.AssociationPermissionDataScopeUserRoleSet
WHERE     (IsDeleted = 0)
GO
ALTER TABLE [dbo].[AssociationTenantUserUserRoleSet] ADD CONSTRAINT [FK_AssociationTenantUserUserRoleSet_TenantSet] FOREIGN KEY
	(
		[TenantId]
	)
	REFERENCES [dbo].[TenantSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AssociationTenantUserUserRoleSet] ADD CONSTRAINT [FK_AssociationTenantUserUserRoleSet_UserRoleSet] FOREIGN KEY
	(
		[UserRoleId]
	)
	REFERENCES [dbo].[UserRoleSet]
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

ALTER TABLE [dbo].[AssociationPermissionDataScopeUserRoleSet] ADD CONSTRAINT [FK_AssociationPermissionDataScopeUserRoleSet_DataScopeSet] FOREIGN KEY
	(
		[DataScopeId]
	)
	REFERENCES [dbo].[DataScopeSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AssociationPermissionDataScopeUserRoleSet] ADD CONSTRAINT [FK_AssociationPermissionDataScopeUserRoleSet_PermissionSet] FOREIGN KEY
	(
		[PermissionId]
	)
	REFERENCES [dbo].[PermissionSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AssociationPermissionDataScopeUserRoleSet] ADD CONSTRAINT [FK_AssociationPermissionDataScopeUserRoleSet_UserRoleSet] FOREIGN KEY
	(
		[UserRoleId]
	)
	REFERENCES [dbo].[UserRoleSet]
	(
		[Id]
	)
GO

-- =============================================
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 05.01.12
-- Description:	Deletes an activity
-- =============================================
CREATE PROCEDURE [dbo].[Activity_Delete]
(
	@id int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Activity_DeleteTx -- Start the transaction..
	
	UPDATE [ActivitySet]
	SET [IsDeleted]=1
	WHERE [Id]=@id
			
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
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 2012-01-04
-- Description:	Adds a DataScope
-- =============================================
CREATE PROCEDURE [dbo].[DataScope_Insert]
(
	@name varchar(100)
	, @description varchar(500)
	, @dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION DataScope_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [DataScopeSet]
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
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 04.01.12
-- Description:	Adds the association.
-- =============================================
CREATE PROCEDURE [dbo].[AssociationPermissionDataScopeUserRole_Insert]
(
	@permissionId int,
	@dataScopeId int,
	@userRoleId int,
	@name varchar(100) = NULL,
	@dateCreated datetime = NULL
)
AS
BEGIN

	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION PermissionDScopeURole_Ins_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			DECLARE @id int = NULL
			SELECT @id = [A].[Id]
			FROM [AssociationPermissionDataScopeUserRoles] [A]
			WHERE [A].[PermissionId]=@permissionId AND [A].[DataScopeId]=@dataScopeId AND [A]
			.[UserRoleId]=@userRoleId
			
			IF @id IS NULL
				BEGIN
			
					INSERT INTO [AssociationPermissionDataScopeUserRoleSet]
					([PermissionId]
					, [DataScopeId]
					, [UserRoleId]
					, [Name]
					, [DateCreated])
					VALUES
					(@permissionId
					, @dataScopeId
					, @userRoleId
					, @name
					, @dateCreated)
					
					SET @id = SCOPE_IDENTITY()
				
				END
			
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
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 2012-01-04
-- Description:	Adds a UserRole
-- =============================================
CREATE PROCEDURE [dbo].[UserRole_Insert]
(
	@name varchar(100)
	, @description varchar(500) = NULL
	, @dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION UserRole_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [UserRoleSet]
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
-- Author:		Francesco Leonetti (lef@gorba.com)
-- Create date: 2012-01-04
-- Description:	Adds a permission
-- =============================================
CREATE PROCEDURE [dbo].[Permission_Insert]
(
	@name varchar(100)
	, @description varchar(500) = NULL
	, @dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Permission_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [PermissionSet]
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
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 04.01.12
-- Description:	Adds the association.
-- =============================================
CREATE PROCEDURE [dbo].[AssociationTenantUserUserRole_Insert]
(
	@tenantId int,
	@userId int,
	@userRoleId int,
	@dateCreated datetime = NULL
)
AS
BEGIN

	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION TenantUserURole_Ins_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			DECLARE @id int = NULL
			SELECT @id = [A].[Id]
			FROM [AssociationTenantUserUserRoles] [A]
			WHERE [A].[TenantId]=@tenantId AND [A].[UserId]=@userId AND [A]
			.[UserRoleId]=@userRoleId
			
			IF @id IS NULL
				BEGIN
			
					INSERT INTO [AssociationTenantUserUserRoleSet]
					([TenantId]
					, [UserId]
					, [UserRoleId]
					, [DateCreated])
					VALUES
					(@tenantId
					, @userId
					, @userRoleId
					, @dateCreated)
					
					SET @id = SCOPE_IDENTITY()
				
				END
			
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
-- Author:		<Author,,Name>
-- Create date: 04.0.12
-- Description:	Finds an operation by name. The search is case insensitive and finds the given string even if it is in the middle of the name.
-- =============================================
CREATE PROCEDURE [dbo].[Operation_FindByName]
(
	@name varchar(100)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT *
	FROM [Operations] [O]
	WHERE [O].[Name] = @name
END
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.2.7'
  ,@description = 'Added access levels'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 2
  ,@versionRevision = 7
  ,@dateCreated = '2012-01-05T12:00:00.000'
GO

