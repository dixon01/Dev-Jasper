 

USE Gorba_CenterOnline
GO

CREATE TABLE [dbo].[UnitSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[ProductTypeId] [int] NOT NULL,
	[GroupId] [int] NULL,
	[LayoutId] [int] NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ShortName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SystemName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SerialNumber] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_UnitSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_UnitSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UnitSetUniqueName] ON [dbo].[UnitSet]
(
	[TenantId] ASC,
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ProductTypeSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UnitTypeId] [int] NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Revision] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsDefault] [bit] NOT NULL CONSTRAINT [DF_ProductTypeSet_IsDefault] DEFAULT ((0)),
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ProductTypeSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_ProductTypeSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ProductTypeSetUniqueName] ON [dbo].[ProductTypeSet]
(
	[UnitTypeId] ASC,
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[UnitGroupSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[GroupTypeId] [int] NOT NULL,
	[ParentId] [int] NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[SystemName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_UnitGroupSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_UnitGroupSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UnitGroupSetUniqueName] ON [dbo].[UnitGroupSet]
(
	[TenantId] ASC,
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[LayoutSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Definition] [xml] NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_LayoutSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_LayoutSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_LayoutSetUniqueName] ON [dbo].[LayoutSet]
(
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[UnitTypeSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsDefault] [bit] NOT NULL CONSTRAINT [DF_UnitTypeSet_IsDefault] DEFAULT ((0)),
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_UnitTypeSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_UnitTypeSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UnitTypeSetUniqueName] ON [dbo].[UnitTypeSet]
(
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[UnitGroupTypeSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsDefault] [bit] NOT NULL CONSTRAINT [DF_UnitGroupTypeSet_IsDefault] DEFAULT ((0)),
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_UnitGroupTypeSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_UnitGroupTypeSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UnitGroupTypeSetUniqueName] ON [dbo].[UnitGroupTypeSet]
(
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[TenantSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsDefault] [bit] NOT NULL CONSTRAINT [DF_TenantSet_IsDefault] DEFAULT ((0)),
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_TenantSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_TenantSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TenantSetIsDeleted] ON [dbo].[TenantSet]
(
	[IsDefault] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_TenantSetUniqueName] ON [dbo].[TenantSet]
(
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 02.12.11 10:47
-- Description:	Inserts a new version of the database in the DatabaseVersionSet table.
--				Everything is done within a transaction.
--				If the @dateCreated parameter is NULL, then the actual UTC time is used.
--				If the operations succeeds, the identifier assigned to the created row is selected
-- =============================================
ALTER PROCEDURE [dbo].[DatabaseVersion_Insert]
(
	@name varchar(100),
	@description varchar(500) = NULL,
	@versionMajor int,
	@versionMinor int,
	@versionBuild int,
	@versionRevision int,
	@dateCreated datetime = NULL
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
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 02.12.11
-- Description:	This SP selects the database version entries ordering them by version.
-- =============================================
ALTER PROCEDURE [dbo].[DatabaseVersion_Select]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [DV].*
	FROM [DatabaseVersions] [DV]
	ORDER BY [VersionMajor], [VersionMinor], [VersionBuild], [VersionRevision]
END
GO
CREATE VIEW [dbo].[Units]
AS
SELECT     Id, TenantId, ProductTypeId, GroupId, LayoutId, Name, ShortName, SystemName, SerialNumber, Description, DateCreated, DateModified
FROM         dbo.UnitSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[UnitGroupTypes]
AS
SELECT     Id, Name, Description, IsDefault, DateCreated, DateModified
FROM         dbo.UnitGroupTypeSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[Tenants]
AS
SELECT     Id, Name, Description, IsDefault, DateCreated, DateModified
FROM         dbo.TenantSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[ProductTypes]
AS
SELECT     Id, UnitTypeId, Name, Revision, Description, IsDefault, DateCreated, DateModified
FROM         dbo.ProductTypeSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[UnitGroups]
AS
SELECT     Id, TenantId, GroupTypeId, ParentId, Name, SystemName, Description, DateCreated, DateModified
FROM         dbo.UnitGroupSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[Layouts]
AS
SELECT     Id, Name, Definition, [Description], DateCreated, DateModified
FROM         dbo.LayoutSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[UnitTypes]
AS
SELECT     Id, Name, Description, IsDefault, DateCreated, DateModified
FROM         dbo.UnitTypeSet
WHERE     (IsDeleted = 0)
GO
ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_LayoutSet] FOREIGN KEY
	(
		[LayoutId]
	)
	REFERENCES [dbo].[LayoutSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_ProductTypeSet] FOREIGN KEY
	(
		[ProductTypeId]
	)
	REFERENCES [dbo].[ProductTypeSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_ProductTypeSet1] FOREIGN KEY
	(
		[ProductTypeId]
	)
	REFERENCES [dbo].[ProductTypeSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_TenanttSet] FOREIGN KEY
	(
		[TenantId]
	)
	REFERENCES [dbo].[TenantSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[UnitSet] ADD CONSTRAINT [FK_UnitSet_UnitGroupSet] FOREIGN KEY
	(
		[GroupId]
	)
	REFERENCES [dbo].[UnitGroupSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[UnitGroupSet] ADD CONSTRAINT [FK_UnitGroupSet_ParentUnitGroupSet] FOREIGN KEY
	(
		[ParentId]
	)
	REFERENCES [dbo].[UnitGroupSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[UnitGroupSet] ADD CONSTRAINT [FK_UnitGroupSet_TenantSet] FOREIGN KEY
	(
		[TenantId]
	)
	REFERENCES [dbo].[TenantSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[UnitGroupSet] ADD CONSTRAINT [FK_UnitGroupSet_UnitGroupTypeSet] FOREIGN KEY
	(
		[GroupTypeId]
	)
	REFERENCES [dbo].[UnitGroupTypeSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ProductTypeSet] ADD CONSTRAINT [FK_ProductTypeSet_UnitTypeSet] FOREIGN KEY
	(
		[UnitTypeId]
	)
	REFERENCES [dbo].[UnitTypeSet]
	(
		[Id]
	)
GO

-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 05.12.11 15:40
-- Description:	Adds a layout to the system
-- =============================================
CREATE PROCEDURE [dbo].[Layout_Insert]
(
	@name varchar(100),
	@definition xml,
	@description varchar(500) = NULL,
	@dateCreated datetime = NULL,
	@dateModified datetime = NULL,
	@isDeleted bit = 0	
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Layout_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [LayoutSet]
			([Name]
			, [Definition]
			, [Description]
			, [DateCreated])
			VALUES
			(@name
			, @definition
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
-- Create date: 05.12.11 15:40
-- Description:	Adds a unit group to the system
-- =============================================
CREATE PROCEDURE [dbo].[UnitGroup_Insert]
(
	@tenantId int,
	@groupTypeId int,
	@parentId int = NULL,
	@name varchar(100),
	@systemName varchar(100) = NULL,
	@description varchar(500) = NULL,
	@dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION UnitGroup_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [UnitGroupSet]
			([TenantId]
			, [GroupTypeId]
			, [ParentId]
			, [Name]
			, [SystemName]
			, [Description]
			, [DateCreated])
			VALUES
			(@tenantId
			, @groupTypeId
			, @parentId
			, @name
			, @systemName
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
-- Create date: 05.12.11 15:40
-- Description:	Adds a product type to the system
-- =============================================
CREATE PROCEDURE [dbo].[ProductType_Insert]
(
	@unitTypeId int,
	@name varchar(100),
	@revision varchar(100),
	@description varchar(500) = NULL,
	@isDefault bit = 0,
	@dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION ProductType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [ProductTypeSet]
			([UnitTypeId]
			, [Name]
			, [Revision]
			, [Description]
			, [IsDefault]
			, [DateCreated])
			VALUES
			(@unitTypeId
			, @name
			, @revision
			, @description
			, @isDefault
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
-- Create date: 05.12.11 15:40
-- Description:	Adds a unit to the system
-- =============================================
CREATE PROCEDURE [dbo].[Unit_Insert]
(
	@tenantId int,
	@productTypeId int,
	@groupId int = NULL,
	@layoutId int = NULL,
	@name varchar(100),
	@shortName varchar(50),
	@systemName varchar(100),
	@serialNumber varchar(50),
	@description varchar(500) = NULL,
	@dateCreated datetime = NULL
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
			, [Name]
			, [ShortName]
			, [SystemName]
			, [SerialNumber]
			, [Description]
			, [DateCreated])
			VALUES
			(@tenantId
			, @productTypeId
			, @groupId
			, @layoutId
			, @name
			, @shortName
			, @systemName
			, @serialNumber
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
-- Create date: 05.12.11 15:40
-- Description:	Adds a unit type to the system
-- =============================================
CREATE PROCEDURE [dbo].[UnitType_Insert]
(
	@name varchar(100),
	@description varchar(500) = NULL,
	@isDefault bit = 0,
	@dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION UnitType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [UnitTypeSet]
			([Name]
			, [Description]
			, [IsDefault]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @isDefault
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
-- Create date: 05.12.11 15:40
-- Description:	Adds a tenant to the system
-- =============================================
CREATE PROCEDURE [dbo].[Tenant_Insert]
(
	@name varchar(100),
	@description varchar(500) = NULL,
	@isDefault bit = 0,
	@dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Tenant_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [TenantSet]
			([Name]
			, [Description]
			, [IsDefault]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @isDefault
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
-- Create date: 05.12.11 15:40
-- Description:	Adds a unit group type to the system
-- =============================================
CREATE PROCEDURE [dbo].[UnitGroupType_Insert]
(
	@name varchar(100),
	@description varchar(500) = NULL,
	@isDefault bit = 0,
	@dateCreated datetime = NULL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION ProductType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		IF @dateCreated IS NULL
			BEGIN
				SET @dateCreated = GETUTCDATE()
			END
			
			INSERT INTO [ProductTypeSet]
			([Name]
			, [Description]
			, [IsDefault]
			, [DateCreated])
			VALUES
			(@name
			, @description
			, @isDefault
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
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Layout_Delete]
(
	@id int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Layout_DeleteTx -- Start the transaction..

	UPDATE [LayoutSet]
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


-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.1.1'
  ,@description = 'Added basic tables.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 1
  ,@versionRevision = 1
  ,@dateCreated = '2011-12-06T08:15:00.000'
GO
