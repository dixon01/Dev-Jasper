 

USE Gorba_CenterOnline
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
			FROM [AssociationsTenantUserUserRole] [A]
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
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 14.02.2012
-- Description:	Selects all Units belonging to the tenant of the current user
-- =============================================
CREATE PROCEDURE [dbo].[Unit_SelectByTenantOfUser]
	@userId int = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF @userId IS NULL
	BEGIN
		SELECT *
		FROM [Units]
	END
	ELSE
	BEGIN
		DECLARE @tenantId int
		SELECT @tenantId = [A].[TenantId]
		FROM [AssociationsTenantUserUserRole] [A]
		WHERE [A].[UserId] = @userId
		-- Insert statements for procedure here
		SELECT [U].*
		FROM [Units] [U]
		WHERE [U].[TenantId] = @tenantId
	END
END
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 14.02.2012
-- Description:	Selects all Units belonging to a tenantId
-- =============================================
CREATE PROCEDURE [dbo].[Unit_SelectByTenant]
	@tenantId int = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    IF @tenantId IS NULL
    BEGIN
		SELECT *
		FROM [Units]
	END
	ELSE
	BEGIN
		SELECT [U].*
		FROM [Units] [U]
		WHERE [U].[TenantId] = @tenantId
	END
END
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 14.02.2012
-- Description:	Selects Operations by the userId. If the flag tenantWide is set
--				it selects all operations of users related to the tenant of the userId.
-- =============================================
CREATE PROCEDURE [dbo].[Operation_SelectByUser] 
	(@userId int = NULL,
	@tenantWide bit = 0
	)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF @userId IS NULL
	BEGIN
		SELECT *
		FROM [Operations]
	END
	ELSE
	BEGIN
		IF @tenantWide = 0
		BEGIN
			SELECT [O].*
			FROM [Operations] [O]
			WHERE [O].UserId = @userId
		END
		ELSE
		BEGIN
			DECLARE @tenantId int
			SELECT @tenantId = [A].TenantId 
			FROM [AssociationsTenantUserUserRole] [A]
			WHERE [A].UserId = @userId
			SELECT [O].*
			FROM [Operations] [O]
			WHERE [O].UserId IN 
			(SELECT [A].UserId FROM [AssociationsTenantUserUserRole] [A]
			 WHERE [A].TenantId = @tenantId)
		END	
	END	
END
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.5.15'
  ,@description = 'Added SPs Operation_SelectByUser, Unit_SelectByTenant and Unit_SelectByTenantOfUser for filtering'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 5
  ,@versionRevision = 15
  ,@dateCreated = '2012-02-14T14:16:00.000'
GO