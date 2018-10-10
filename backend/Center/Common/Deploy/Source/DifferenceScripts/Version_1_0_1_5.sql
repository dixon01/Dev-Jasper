 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [FK_ActivitySet_OperationSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [FK_OperationSet_WorkflowInstanceSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [FK_OperationSet_UserSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_UntilAbort]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayMon]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_OperationStatus]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayThu]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_Repetition]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayFri]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDaySat]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDaySun]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayTue]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayWed]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [DF_ActivitySet_TypeId]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [PK_ActivitySet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [PK_OperationSet]
GO
DROP INDEX [IX_OperationSetWorkflowInstanceId] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_OperationSetName] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_OperationSetUserId] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_OperationSetOperationStatus] ON [dbo].[OperationSet]
GO
CREATE TABLE [dbo].[AssociationUnitOperationSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UnitId] [int] NOT NULL,
	[OperationId] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	CONSTRAINT [PK_AssociationUnitOperationSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_AssociationUnitOperationUnitIdOperationId] ON [dbo].[AssociationUnitOperationSet]
(
	[UnitId] ASC,
	[OperationId] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[TempActivitySet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[OperationId] [int] NOT NULL,
	[TypeId] [int] NOT NULL CONSTRAINT [DF_ActivitySet_TypeId] DEFAULT ((0)),
	[Attribute] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ActivitySet_IsDeleted] DEFAULT ((0))

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempActivitySet] ON
INSERT INTO [dbo].[TempActivitySet] ([Id],[OperationId],[TypeId],[Attribute],[DateCreated],[IsDeleted]) SELECT [Id],[OperationId],[TypeId],[Attribute],getdate(),((0)) FROM [dbo].[ActivitySet]
SET IDENTITY_INSERT [dbo].[TempActivitySet] OFF
GO

DROP TABLE [dbo].[ActivitySet]
GO
EXEC sp_rename N'[dbo].[TempActivitySet]',N'ActivitySet', 'OBJECT'
GO


CREATE TABLE [dbo].[TempOperationSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[StopDate] [datetime] NOT NULL,
	[OperationStatus] [int] NOT NULL CONSTRAINT [DF_OperationSet_OperationStatus] DEFAULT ((0)),
	[UntilAbort] [bit] NOT NULL CONSTRAINT [DF_OperationSet_UntilAbort] DEFAULT ((0)),
	[StartExecutionDayMon] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDayMon] DEFAULT ((0)),
	[StartExecutionDayTue] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDayTue] DEFAULT ((0)),
	[StartExecutionDayWed] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDayWed] DEFAULT ((0)),
	[StartExecutionDayThu] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDayThu] DEFAULT ((0)),
	[StartExecutionDayFri] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDayFri] DEFAULT ((0)),
	[StartExecutionDaySat] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDaySat] DEFAULT ((0)),
	[StartExecutionDaySun] [bit] NOT NULL CONSTRAINT [DF_OperationSet_StartExecutionDaySun] DEFAULT ((0)),
	[Repetition] [int] NOT NULL CONSTRAINT [DF_OperationSet_Repetition] DEFAULT ((0)),
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_OperationSet_IsDeleted] DEFAULT ((0))

) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[TempOperationSet] ON
INSERT INTO [dbo].[TempOperationSet] ([Id],[UserId],[Name],[DateCreated],[StartDate],[StopDate],[OperationStatus],[UntilAbort],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[IsDeleted]) SELECT [Id],[UserId],[Name],[DateCreated],[StartDate],[StopDate],[OperationStatus],[UntilAbort],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],((0)) FROM [dbo].[OperationSet]
SET IDENTITY_INSERT [dbo].[TempOperationSet] OFF
GO

DROP TABLE [dbo].[OperationSet]
GO
EXEC sp_rename N'[dbo].[TempOperationSet]',N'OperationSet', 'OBJECT'
GO


ALTER VIEW [dbo].[Operations]
AS
SELECT [Id]
      ,[UserId]
      ,[Name]
      ,[DateCreated]
      ,[StartDate]
      ,[StopDate]
      ,[OperationStatus]
      ,[UntilAbort]
      ,[StartExecutionDayMon]
      ,[StartExecutionDayTue]
      ,[StartExecutionDayWed]
      ,[StartExecutionDayThu]
      ,[StartExecutionDayFri]
      ,[StartExecutionDaySat]
      ,[StartExecutionDaySun]
      ,[Repetition]
      , [DateModified]
FROM         dbo.OperationSet
WHERE [IsDeleted]=0
GO
ALTER VIEW [dbo].[Activities]
AS
SELECT [Id]
      ,[OperationId]
      ,[TypeId]
      ,[Attribute]
      , [DateCreated]
      , [DateModified]

FROM         dbo.ActivitySet
WHERE [IsDeleted]=0
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Activity_Insert]
(
	@operationId int
	, @typeId int
	, @attribute varchar(1000)
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
		BEGIN TRANSACTION Activity_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [ActivitySet]
			([OperationId]
			, [TypeId]
			, [Attribute]
			, [DateCreated])
			VALUES
			(@operationId
			, @typeId
			, @attribute
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
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Operation_Insert]
(
	@userId int
	, @name varchar(100)
	, @dateCreated datetime
	, @startDate datetime
	, @stopDate datetime
	, @operationStatus int
	, @untilAbort bit
	, @startExecutionDayMon bit
	, @startExecutionDayTue bit
	, @startExecutionDayWed bit
	, @startExecutionDayThu bit
	, @startExecutionDayFri bit
	, @startExecutionDaySat bit
	, @startExecutionDaySun bit
	, @repetition int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION Operation_InsertTx -- Start the transaction..
		-- Insert statements for procedure here
		
			
			INSERT INTO [OperationSet]
			([UserId]
			, [Name]
			, [DateCreated]
			, [StartDate]
			, [StopDate]
			, [OperationStatus]
			, [UntilAbort]
			, [StartExecutionDayMon]
			, [StartExecutionDayTue]
			, [StartExecutionDayWed]
			, [StartExecutionDayThu]
			, [StartExecutionDayFri]
			, [StartExecutionDaySat]
			, [StartExecutionDaySun]
			, [Repetition])
			VALUES
			(@userId
			, @name
			, @dateCreated
			, @startDate
			, @stopDate
			, @operationStatus
			, @untilAbort
			, @startExecutionDayMon
			, @startExecutionDayTue
			, @startExecutionDayWed
			, @startExecutionDayThu
			, @startExecutionDayFri
			, @startExecutionDaySat
			, @startExecutionDaySun
			, @repetition)
			
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
CREATE NONCLUSTERED INDEX [IX_OperationSetUserId] ON [dbo].[OperationSet]
(
	[UserId] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OperationSetOperationStatus] ON [dbo].[OperationSet]
(
	[OperationStatus] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OperationSetName] ON [dbo].[OperationSet]
(
	[Name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ActivitySet] ADD CONSTRAINT [PK_ActivitySet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[OperationSet] ADD CONSTRAINT [PK_OperationSet] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ActivitySet] ADD CONSTRAINT [FK_ActivitySet_OperationSet] FOREIGN KEY
	(
		[OperationId]
	)
	REFERENCES [dbo].[OperationSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[AssociationUnitOperationSet] ADD CONSTRAINT [FK_AssociationUnitOperationSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
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
-- =============================================
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 13.12.11
-- Description:	Deleted the association between a unit and an operation (if exists).
-- =============================================
CREATE PROCEDURE [dbo].[AssociationUnitOperation_Delete]
(
	@unitId int,
	@operationId int
)
AS
BEGIN
		
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION AssociationUnitOperation_Del_Tx -- Start the transaction..
		-- Insert statements for procedure here
		
			DELETE
			FROM [AssociationUnitOperationSet]
			WHERE [UnitId]=@unitId AND [OperationId]=@operationId
			
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
CREATE PROCEDURE [dbo].[AssociationUnitOperation_Insert]
(
	@unitId int,
	@operationId int,
	@dateCreated datetime = NULL
)
AS
BEGIN

	IF @dateCreated IS NULL
		BEGIN
			SET @dateCreated = GETUTCDATE()
		END
		
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
					, [OperationId]
					, [DateCreated])
					VALUES
					(@unitId
					, @operationId
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
-- Description:	Selects the activities bound to an operation
-- =============================================
CREATE PROCEDURE [dbo].[Activity_SelectByOperation]
(
	@operationId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT [A].*
    FROM [Activities] [A]
    WHERE [A].[OperationId]=@operationId
END
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.1.5'
  ,@description = 'Changes and fixes for Operations and Activities. Removed the connection to workflows.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 1
  ,@versionRevision = 5
  ,@dateCreated = '2011-12-14T11:00:00.000'
GO