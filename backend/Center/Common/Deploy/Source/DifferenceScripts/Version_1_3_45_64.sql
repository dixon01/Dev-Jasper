/*
 * LEF 04.09.13
 * WARNING!!! always backup the database before executing the script
 * Extends the User table adding a reference to the Owner tenant.
 *
 * The Owner is mandatory. If data (Users) exist, we do the following operations:
 * - Search the Default tenant
 * - If not found, we search the tenant named 'Gorba' (case insensitive)
 * - If not found, we search the first tenant available (lower Id value)
 * - If not found, we create the Gorba tenant
 * - Existing users are assigned to the found tenant
 *
 * WARNING: The column Name is renamed to FirstName
*/

USE [Gorba_CenterOnline]
	
-- Add the OwnerTenantId to UserSet as nullable
ALTER TABLE [Gorba_CenterOnline].[dbo].[UserSet]
ADD [OwnerTenantId] int NULL

-- Add the UId to TenantSet and UnitSet
ALTER TABLE [Gorba_CenterOnline].[dbo].[TenantSet]
ADD [UId] varchar(20) NULL
ALTER TABLE [Gorba_CenterOnline].[dbo].[UnitSet]
ADD [UId] varchar(20) NULL
GO

sp_rename '[Gorba_CenterOnline].[dbo].[UserSet].[Name]', 'FirstName' , 'COLUMN'

GO

BEGIN TRY --Start the Try Block..
	BEGIN TRANSACTION [ClearDatabase_Tx] -- Start the transaction..
	DECLARE @usersCount int
	
	SELECT @usersCount = COUNT(*)
	FROM [Gorba_CenterOnline].[dbo].[UserSet]
	
	IF @usersCount > 0
		BEGIN	 

			DECLARE @ownerTenantId int = NULL

			-- First, let's search the default tenant. If it exists, it will be the owner of all existing users.
			SELECT TOP 1 @ownerTenantId = [Id]
			FROM [Gorba_CenterOnline].[dbo].[TenantSet]
			WHERE [IsDefault] = 1

			IF @ownerTenantId IS NULL
				-- Default tenant not found. Let's try to get the tenant 'Gorba' from the system
				BEGIN
					SELECT TOP 1 @ownerTenantId = [Id]
					FROM [Gorba_CenterOnline].[dbo].[TenantSet]
					WHERE [Name] LIKE 'Gorba'
				END

			IF @ownerTenantId IS NULL
				-- Last attempt: let's get the first Tenant
				BEGIN
					SELECT TOP 1 @ownerTenantId = [Id]
					FROM [Gorba_CenterOnline].[dbo].[TenantSet]
				END

			IF @ownerTenantId IS NULL
				-- No tenant found. Let's add Gorba again
				BEGIN
					INSERT INTO [Gorba_CenterOnline].[dbo].[TenantSet]
					   ([Name]
					   ,[Description]
					   ,[IsDefault]
					   ,[DateCreated]
					   ,[DateModified]
					   ,[IsDeleted])
					VALUES
					   ('Gorba'
					   ,'Default Gorba Tenant'
					   ,1
					   ,GETUTCDATE()
					   ,NULL
					   ,0)
					SET @ownerTenantId = SCOPE_IDENTITY()
				END
				
			UPDATE [Gorba_CenterOnline].[dbo].[UserSet]
			SET [OwnerTenantId] = @ownerTenantId
		END
	-- ENDIF
	
	-- add UId values for tenants and units
	    
	UPDATE [Gorba_CenterOnline].[dbo].[TenantSet]
	SET [UId] = CONVERT(varchar(20), [Id])
	
	UPDATE [Gorba_CenterOnline].[dbo].[UnitSet]
	SET [UId] = CONVERT(varchar(20), [Id])

	--=============================================================
	-- Versioning
	--=============================================================
	DECLARE @RC int

	EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
	   @name = 'Version 1.3.45.64'
	  ,@description = 'Added OwnerTenantId to Users. Renamed column [UserSet].[Name] to [UserSet].[FirstName].'
	  ,@versionMajor = 1
	  ,@versionMinor = 3
	  ,@versionBuild = 45
	  ,@versionRevision = 64
	  ,@dateCreated = '2013-09-04T09:00:00.000'

	COMMIT TRAN -- Transaction Success!

END TRY

	BEGIN CATCH


	IF @@TRANCOUNT > 0


		ROLLBACK TRAN --RollBack in case of Error



-- you can Raise ERROR with RAISEERROR() Statement including the details of the exception

	DECLARE @errorMessage varchar(5000) = ERROR_MESSAGE()


	RAISERROR(@errorMessage, 11, 1)

END CATCH

GO
	
-- Remove the nullability from added columns
ALTER TABLE [Gorba_CenterOnline].[dbo].[UserSet]
ALTER COLUMN [OwnerTenantId] int NOT NULL

ALTER TABLE [Gorba_CenterOnline].[dbo].[TenantSet]
ALTER COLUMN [UId] varchar(20) NOT NULL

ALTER TABLE [Gorba_CenterOnline].[dbo].[UnitSet]
ALTER COLUMN [UId] varchar(20) NOT NULL

-- Add the constraints
ALTER TABLE [Gorba_CenterOnline].[dbo].[UserSet]
ADD CONSTRAINT [FK_UserSet_OwnerTenantId] FOREIGN KEY ([OwnerTenantId])
REFERENCES [Gorba_CenterOnline].[dbo].[TenantSet]([Id])USE [Gorba_CenterOnline]

ALTER TABLE [Gorba_CenterOnline].[dbo].[TenantSet]
ADD CONSTRAINT [TenantSet_UniqueUId] UNIQUE ([UId])

ALTER TABLE [Gorba_CenterOnline].[dbo].[UnitSet]
ADD CONSTRAINT [UnitSet_UniqueUId] UNIQUE ([UId])

GO

ALTER VIEW [dbo].[Tenants]
AS
SELECT     [Id], [UId], [Name], [Description], [IsDefault], [DateCreated], [DateModified]
FROM         [Gorba_CenterOnline].[dbo].[TenantSet]
WHERE     [IsDeleted] = 0

GO

ALTER VIEW [dbo].[Users]
AS
SELECT     [Id], [OwnerTenantId], [Username], [HashedPassword], [FirstName], [LastName], [Email], [DateCreated], [DateModified], [IsDeleted], [Culture], [ShortName], [TimeZoneInfoId]
FROM         [Gorba_CenterOnline].[dbo].[UserSet]
WHERE [IsDeleted] = 0

GO

ALTER VIEW [dbo].[Units]
AS
WITH Map([Type], [Status])
AS
(
	SELECT 0,1
	UNION ALL
	SELECT 1,2
	UNION ALL
	SELECT 2,4
	UNION ALL
	SELECT 3,8
)
, AlarmStatuses([Status], [UnitId])
AS
(
	SELECT  SUM(DISTINCT ISNULL([M].[Status], 1)) AS [Status], [A].[UnitId]
	FROM [AlarmSet] [A]
	LEFT OUTER JOIN [Map] [M] ON [M].[Type] = [A].[Type]
	WHERE [A].[DateConfirmed] IS NULL
	GROUP BY [A].[UnitId]
)
, [Operations_Aggregated] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	GROUP BY [A].[UnitId]
)
, [Operations_Error] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState]=1
	GROUP BY [A].[UnitId]
)
, [Operations_Revoking] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState]=2
	GROUP BY [A].[UnitId]
)
, [Operations_Active] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 3
	GROUP BY [A].[UnitId]
)
, [Operations_Transmitting] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 4
	GROUP BY [A].[UnitId]
)
, [Operations_Transmitted] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 5
	GROUP BY [A].[UnitId]
)
, [Operations_Scheduled] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 6
	GROUP BY [A].[UnitId]
)
, [Operations_Ended] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 7
	GROUP BY [A].[UnitId]
)
, [Operations_Revoked] AS
(
	SELECT COUNT(*) AS [Count], [A].[UnitId]
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[OperationId] = [O].[Id]
	WHERE [O].[OperationState] = 8
	GROUP BY [A].[UnitId]
)
SELECT [U].[Id]
	  , [U].[UId]
      ,[U].[TenantId]
      ,[U].[ProductTypeId]
      ,[U].[LayoutId]
      ,[U].[Name]
      ,[U].[ShortName]
      ,[U].[SystemName]
      ,[U].[SerialNumber]
      ,[U].[Description]
      ,[U].[DateCreated]
      ,[U].[DateModified]
      ,[U].[NetworkAddress]
      ,[U].[IsOnline]
      ,[U].[LastSeenOnline]
      ,[U].[LocationName]
      ,[U].[CommunicationStatus]
      ,[U].[OperationStatus]
      ,[U].[LastRestartRequestDate]
      ,[U].[LastTimeSyncRequestDate]
      ,[U].[LastTimeSyncValue]
      ,[U].[TimeZoneInfoId]
      ,[U].[GatewayAddress]
      ,ISNULL(CAST([A].[Status] AS int), 0) AS [AlarmStatus]
      ,[U].[ErrorOperationsCount]
      ,[U].[RevokingOperationsCount]
      ,[U].[ActiveOperationsCount]
      ,[U].[TransmittingOperationsCount]
      ,[U].[TransmittedOperationsCount]
      ,[U].[ScheduledOperationsCount]
      ,[U].[EndedOperationsCount]
      ,[U].[RevokedOperationsCount]
      ,[U].[TotalOperationsCount]

FROM         [dbo].[UnitSet] [U]
LEFT OUTER JOIN [AlarmStatuses] [A] ON [A].[UnitId] = [U].[Id]

WHERE     [U].[IsDeleted] = 0

GO