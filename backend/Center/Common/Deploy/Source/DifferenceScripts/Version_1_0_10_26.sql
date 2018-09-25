 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[AssociationTenantUserUserRoleSet] DROP COLUMN [DateCreated],[DateModified]
GO
ALTER TABLE [dbo].[AssociationUnitOperationSet] DROP COLUMN [DateCreated]
GO
ALTER TABLE [dbo].[AssociationUnitStationSet] DROP COLUMN [DateCreated]
GO
ALTER TABLE [dbo].[AssociationPermissionDataScopeUserRoleSet] DROP COLUMN [DateCreated],[DateModified]
GO
CREATE TABLE [dbo].[ItcsFilterSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[StationId] [int] NOT NULL,
	[ItcsConfigurationId] [int] NOT NULL,
	[ItcsStationReferenceId] [int] NOT NULL,
	[LineName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LineReferenceName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Direction] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT [PK_ItcsFilterSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ItcsStationReferenceSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ProviderId] [int] NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT [PK_ItcsStationReferenceSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ItcsProviderSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT [PK_ItcsProviderSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER VIEW [dbo].[AssociationsUnitOperation]
AS
SELECT     Id, UnitId, OperationId
FROM         dbo.AssociationUnitOperationSet
WHERE     (IsDeleted = 0)



;

;
GO
ALTER VIEW [dbo].[AssociationsTenantUserUserRole]
AS
SELECT     Id, TenantId, UserId, UserRoleId
FROM         dbo.AssociationTenantUserUserRoleSet
WHERE     (IsDeleted = 0)



;

;
GO
ALTER VIEW [dbo].[AssociationsPermissionDataScopeUserRole]
AS
SELECT     Id, PermissionId, DataScopeId, UserRoleId, Name
FROM         dbo.AssociationPermissionDataScopeUserRoleSet
WHERE     (IsDeleted = 0)



;

;
GO
ALTER VIEW [dbo].[AssociationsUnitStation]
AS
SELECT     Id, UnitId, StationId
FROM         dbo.AssociationUnitStationSet
WHERE     (IsDeleted = 0)



;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 04.01.12
-- Description:	Adds the association.
-- =============================================
ALTER PROCEDURE [dbo].[AssociationTenantUserUserRole_Insert]
(
	@tenantId int,
	@userId int,
	@userRoleId int
)
AS
BEGIN
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION TenantUserURole_Ins_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			DECLARE @id int = NULL
			SELECT @id = [A].[Id]
			FROM [AssociationsTenantUserUserRole] [A]
			WHERE [A].[TenantId]=@tenantId AND [A].[UserId]=@userId AND [A]
			.[UserRoleId]=@userRoleId
			
			IF @id IS NULL
				BEGIN
			
					INSERT INTO [AssociationTenantUserUserRoleSet]
					([TenantId]
					, [UserId]
					, [UserRoleId])
					VALUES
					(@tenantId
					, @userId
					, @userRoleId)
					
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

;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 13.12.11
-- Description:	Adds the association between a unit and an operation if does not exist.
-- =============================================
ALTER PROCEDURE [dbo].[AssociationUnitOperation_Insert]
(
	@unitId int,
	@operationId int
)
AS
BEGIN
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AssociationUnitOperation_Ins_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			DECLARE @id int = NULL
			SELECT @id = [A].[Id]
			FROM [AssociationUnitOperationSet] [A]
			WHERE [A].[UnitId]=@unitId AND [A].[OperationId]=@operationId
			
			IF @id IS NULL
				BEGIN
			
					INSERT INTO [AssociationUnitOperationSet]
					([UnitId]
					, [OperationId])
					VALUES
					(@unitId
					, @operationId)
					
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



;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 13.12.11
-- Description:	Adds the association between a unit and an operation if does not exist.
-- =============================================
ALTER PROCEDURE [dbo].[AssociationUnitStation_Insert]
(
	@unitId int,
	@stationId int
)
AS
BEGIN
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AssociationUnitStation_Ins_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			DECLARE @id int = NULL
			SELECT @id = [A].[Id]
			FROM [AssociationUnitStationSet] [A]
			WHERE [A].[UnitId]=@unitId AND [A].[StationId]=@stationId
			
			IF @id IS NULL
				BEGIN
			
					INSERT INTO [AssociationUnitStationSet]
					([UnitId]
					, [StationId])
					VALUES
					(@unitId
					, @stationId)
					
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



;

;
GO
-- =============================================
-- Author:		Thomas Epple (ept@gorba.com)
-- Create date: 2012-01-09
-- Description:	Adds an Alarm
-- =============================================
ALTER PROCEDURE [dbo].[Alarm_Insert]
(
	@unitId int,
	@userId int = NULL,
	@name varchar(100),
	@description varchar(500),
	@endDate datetime,
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
			([UnitId]
			, [UserId]
			, [Name]
			, [Description]
			, [EndDate]
			, [DateCreated])
			VALUES
			(@unitId
			, @userId
			, @name
			, @description
			, @endDate
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



;

;
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 04.01.12
-- Description:	Adds the association.
-- =============================================
ALTER PROCEDURE [dbo].[AssociationPermissionDataScopeUserRole_Insert]
(
	@permissionId int,
	@dataScopeId int,
	@userRoleId int,
	@name varchar(100) = NULL
)
AS
BEGIN
		
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
					, [Name])
					VALUES
					(@permissionId
					, @dataScopeId
					, @userRoleId
					, @name)
					
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



;

;
GO
ALTER TABLE [dbo].[ItcsFilterSet] ADD CONSTRAINT [FK_ItcsFilterSet_ItcsConfigurationSet] FOREIGN KEY
	(
		[ItcsConfigurationId]
	)
	REFERENCES [dbo].[ItcsConfigurationSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ItcsFilterSet] ADD CONSTRAINT [FK_ItcsFilterSet_ItcsStationReferenceSet] FOREIGN KEY
	(
		[ItcsStationReferenceId]
	)
	REFERENCES [dbo].[ItcsStationReferenceSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ItcsFilterSet] ADD CONSTRAINT [FK_ItcsFilterSet_StationSet] FOREIGN KEY
	(
		[StationId]
	)
	REFERENCES [dbo].[StationSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ItcsStationReferenceSet] ADD CONSTRAINT [FK_ItcsStationReferenceSet_ItcsProviderSet] FOREIGN KEY
	(
		[ProviderId]
	)
	REFERENCES [dbo].[ItcsProviderSet]
	(
		[Id]
	)
GO

-- =======================================================================
-- Versioning
-- =======================================================================

DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.10.26'
  ,@description = 'Added Itcs specific tables.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 10
  ,@versionRevision = 26
  ,@dateCreated = '2012-05-01T10:10:00.000'
GO