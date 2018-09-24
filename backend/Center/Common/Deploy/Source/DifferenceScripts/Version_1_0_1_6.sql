 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[OperationSet] ALTER COLUMN [StopDate] [datetime] NULL
GO
ALTER TABLE [dbo].[UnitSet] ADD 
[NetworkAddress] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
ALTER TABLE [dbo].[AssociationUnitOperationSet] ADD 
[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_AssociationUnitOperationSet_IsDeleted] DEFAULT ((0))
GO
CREATE TABLE [dbo].[AssociationUnitStationSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UnitId] [int] NOT NULL,
	[StationId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_AssociationUnitStationSet_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_AssociationUnitStationSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER VIEW [dbo].[Units]
AS
SELECT     Id, TenantId, ProductTypeId, GroupId, LayoutId, Name, ShortName, SystemName, SerialNumber, [NetworkAddress], [Description], DateCreated, DateModified
FROM         dbo.UnitSet
WHERE     (IsDeleted = 0)
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 13.12.11
-- Description:	Deleted the association between a unit and an operation (if exists).
-- =============================================
ALTER PROCEDURE [dbo].[AssociationUnitOperation_Delete]
(
	@id int = NULL,
	@unitId int = NULL,
	@operationId int = NULL
)
AS
BEGIN
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AssociationUnitOperation_Del_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			UPDATE [AssociationUnitOperationSet]
			SET [IsDeleted]=1
			WHERE
			(@id IS NOT NULL AND [Id]=@id)
			OR
			([UnitId]=@unitId AND [OperationId]=@operationId)
			
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
	@layoutId int = NULL,
	@name varchar(100),
	@shortName varchar(50) = NULL,
	@systemName varchar(100) = NULL,
	@serialNumber varchar(50) = NULL,
	@networkAddress varchar(256) = NULL,
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
			, [NetworkAddress]
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
			, @networkAddress
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
CREATE VIEW [dbo].[AssociationsUnitOperation]
AS
SELECT     Id, UnitId, OperationId, DateCreated
FROM         dbo.AssociationUnitOperationSet
WHERE     (IsDeleted = 0)
GO
CREATE VIEW [dbo].[AssociationsUnitStation]
AS
SELECT     Id, UnitId, StationId, DateCreated
FROM         dbo.AssociationUnitStationSet
WHERE     (IsDeleted = 0)
GO
ALTER TABLE [dbo].[AssociationUnitStationSet] ADD CONSTRAINT [FK_AssociationUnitStationSet_StationSet] FOREIGN KEY
	(
		[StationId]
	)
	REFERENCES [dbo].[StationSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AssociationUnitStationSet] ADD CONSTRAINT [FK_AssociationUnitStationSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[AssociationUnitOperationSet] ADD CONSTRAINT [FK_AssociationUnitOperationSet_OperationSet] FOREIGN KEY
	(
		[OperationId]
	)
	REFERENCES [dbo].[OperationSet]
	(
		[Id]
	)
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 22.12.11
-- Description:	Select stations associated with a specified unit
-- =============================================
CREATE PROCEDURE [dbo].[Unit_SelectAssociatedStations]
(
	@unitId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [S].*
	FROM [Stations] [S]
	INNER JOIN [AssociationsUnitStation] [A] ON [A].[UnitId]=@unitId AND [A].[StationId]=[S].[Id]
	
END
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 22.12.11
-- Description:	Select Operations associated with a specified unit
-- =============================================
CREATE PROCEDURE [dbo].[Unit_SelectAssociatedOperations]
(
	@unitId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [O].*
	FROM [Operations] [O]
	INNER JOIN [AssociationsUnitOperation] [A] ON [A].[UnitId]=@unitId AND [A].[OperationId]=[O].[Id]
	
END
GO
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 22.12.11
-- Description:	Updates an operation
-- =============================================
CREATE PROCEDURE [dbo].[Operation_Update] 
	-- Add the parameters for the stored procedure her
	@id int,
	@startDate datetime,
	@name varchar(100),
	@stopDate datetime,
	@operationStatus int,
	@untilAbort bit,
	@startExecutionDayMon bit,
	@startExecutionDayTue bit,
	@startExecutionDayWed bit,
	@startExecutionDayThu bit,
	@startExecutionDayFri bit,
	@startExecutionDaySat bit,
	@startExecutionDaySun bit,
	@repetition int,
	@dateModified datetime = NULL
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
			
			UPDATE [OperationSet]
			SET [StartDate] = @startDate
			, [Name] = @name
			, [StopDate] = @stopDate
			, [OperationStatus] = @operationStatus
			, [UntilAbort] = @untilAbort
			, [StartExecutionDayMon] = @startExecutionDayMon
			, [StartExecutionDayTue] = @startExecutionDayTue
			, [StartExecutionDayWed] = @startExecutionDayWed
			, [StartExecutionDayThu] = @startExecutionDayThu
			, [StartExecutionDayFri] = @startExecutionDayFri
			, [StartExecutionDaySat] = @startExecutionDaySat
			, [StartExecutionDaySun] = @startExecutionDaySun
			, [Repetition] = @repetition
			, [DateModified] = @dateModified
			
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
-- Create date: 13.12.11
-- Description:	Adds the association between a unit and an operation if does not exist.
-- =============================================
CREATE PROCEDURE [dbo].[AssociationUnitStation_Insert]
(
	@unitId int,
	@stationId int,
	@dateCreated datetime = NULL
)
AS
BEGIN

	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END
		
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
					, [StationId]
					, [DateCreated])
					VALUES
					(@unitId
					, @stationId
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
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 13.12.11
-- Description:	Deleted the association between a unit and an operation (if exists).
-- =============================================
CREATE PROCEDURE [dbo].[AssociationUnitStation_Delete]
(
	@id int = NULL,
	@unitId int = NULL,
	@stationId int = NULL
)
AS
BEGIN
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AssociationUnitStation_Del_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			UPDATE [AssociationUnitStationSet]
			SET [IsDeleted]=1
			WHERE
			(@id IS NOT NULL AND [Id]=@id)
			OR
			([UnitId]=@unitId AND [StationId]=@stationId)
			
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
-- Create date: 22.12.11
-- Description:	Deletes an operation and all its related activities
-- =============================================
CREATE PROCEDURE [dbo].[Operation_Delete]
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
	
	UPDATE [ActivitySet]
	SET [IsDeleted]=1
	WHERE [OperationId]=@id

	UPDATE [OperationSet]
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

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.1.6'
  ,@description = 'Added association between units and stations. Added the network address to the units. Other improvements to tables and SPs'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 1
  ,@versionRevision = 6
  ,@dateCreated = '2011-12-22T08:00:00.000'
GO