 

USE Gorba_CenterOnline
GO

-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 14.02.2012
-- Description:	Selects all Units belonging to the tenants of the current user
-- =============================================
ALTER PROCEDURE [dbo].[Unit_SelectByTenantOfUser]
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
		SELECT U.* from UnitSet U 
		JOIN AssociationTenantUserUserRoleSet TU ON U.TenantId = TU.TenantId 
		WHERE TU.UserId = @userId
	END
END
GO
-- =============================================
-- Author:		Thomas Epple (thomas.epple@gorba.com)
-- Create date: 14.02.2012
-- Description:	Selects Operations filterd by userId and/or tenantId
-- =============================================
ALTER PROCEDURE [dbo].[Operation_SelectByUser] 
	(@userId int = NULL,
	@tenantId int = NULL
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
		IF @userId IS NULL
		BEGIN
			SELECT DISTINCT O.* from OperationSet O 
			JOIN AssociationUnitOperationSet UO ON O.Id = UO.OperationId 
			JOIN UnitSet U on UO.UnitId = U.Id
			WHERE U.TenantId = @tenantId		
		END
		ELSE
		BEGIN
			IF @tenantId IS NULL
			BEGIN
				SELECT *
				FROM [Operations] [O]
				WHERE O.UserId = @userID
			END
			ELSE
			BEGIN
				SELECT DISTINCT O.* from OperationSet O 
				JOIN AssociationUnitOperationSet UO ON O.Id = UO.OperationId 
				JOIN UnitSet U on UO.UnitId = U.Id
				WHERE U.TenantId = @tenantId AND O.UserId = @userId
			END
		END	
	END	END
GO


DECLARE @RC int
DECLARE @name varchar(100) = 'Database version 1.0.6.18'
DECLARE @description varchar(500) = 'Fixed SPs Unit_SelectByTenantOfUser and Operation_SelectByUser'
DECLARE @versionMajor int = 1
DECLARE @versionMinor int = 0
DECLARE @versionBuild int = 6
DECLARE @versionRevision int = 18
DECLARE @dateCreated datetime = '2012-03-05T07:25:00.000'

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name
  ,@description
  ,@versionMajor
  ,@versionMinor
  ,@versionBuild
  ,@versionRevision
  ,@dateCreated
GO

 