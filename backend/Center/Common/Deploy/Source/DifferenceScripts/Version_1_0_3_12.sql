 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[InfoLineTextActivitySet] DROP CONSTRAINT [FK_InfoLineTextActivitySet_ActivitySet]
GO
ALTER TABLE [dbo].[InfoLineTextActivitySet] DROP CONSTRAINT [FK_InfoLineTextActivitySet_UnitSet]
GO
ALTER TABLE [dbo].[AssociationUnitOperationSet] DROP CONSTRAINT [FK_AssociationUnitOperationSet_OperationSet]
GO
ALTER TABLE [dbo].[ActivitySet] DROP CONSTRAINT [FK_ActivitySet_OperationSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [FK_OperationSet_UserSet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_UntilAbort]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayMon]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_OperationStatus]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_Repetition]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_IsDeleted]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayThu]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayFri]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDaySat]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayTue]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDayWed]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [DF_OperationSet_StartExecutionDaySun]
GO
ALTER TABLE [dbo].[InfoLineTextActivitySet] DROP CONSTRAINT [PK_InfoLineTextActivitySet]
GO
ALTER TABLE [dbo].[OperationSet] DROP CONSTRAINT [PK_OperationSet]
GO
DROP INDEX [IX_OperationSetOperationStatus] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_OperationSetName] ON [dbo].[OperationSet]
GO
DROP INDEX [IX_OperationSetUserId] ON [dbo].[OperationSet]
GO
ALTER TABLE [dbo].[ActivitySet] DROP COLUMN [Attribute]
GO
CREATE TABLE [dbo].[TempInfoLineTextActivitySet]
(
	[Id] [int] NOT NULL,
	[LineNumber] [int] NOT NULL,
	[DisplayText] [varchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DestinationId] [int] NOT NULL,
	[ExpirationDate] [datetime] NOT NULL,
	[InfoRowId] [smallint] NOT NULL,
	[Blink] [bit] NOT NULL,
	[DisplayedScreenSide] [smallint] NOT NULL,
	[Alignment] [smallint] NOT NULL,
	[Font] [smallint] NULL

) ON [PRIMARY]
GO

INSERT INTO [dbo].[TempInfoLineTextActivitySet] ([Id],[LineNumber],[DestinationId],[DisplayText],[InfoRowId],[Blink],[DisplayedScreenSide],[Alignment],[Font],[ExpirationDate]) SELECT [Id],[LineNumber],[DestinationId],[DisplayText],[InfoRowId],[Blink],[DisplayedScreenSide],[Alignment],[Font],getdate() FROM [dbo].[InfoLineTextActivitySet]
DROP TABLE [dbo].[InfoLineTextActivitySet]
GO
EXEC sp_rename N'[dbo].[TempInfoLineTextActivitySet]',N'InfoLineTextActivitySet', 'OBJECT'
GO


CREATE TABLE [dbo].[TempOperationSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[UnitId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[StopDate] [datetime] NULL,
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
INSERT INTO [dbo].[TempOperationSet] ([Id],[UserId],[StartDate],[Name],[StopDate],[OperationStatus],[UntilAbort],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[DateCreated],[DateModified],[IsDeleted],[UnitId]) SELECT [Id],[UserId],[StartDate],[Name],[StopDate],[OperationStatus],[UntilAbort],[StartExecutionDayMon],[StartExecutionDayTue],[StartExecutionDayWed],[StartExecutionDayThu],[StartExecutionDayFri],[StartExecutionDaySat],[StartExecutionDaySun],[Repetition],[DateCreated],[DateModified],[IsDeleted],0 FROM [dbo].[OperationSet]
SET IDENTITY_INSERT [dbo].[TempOperationSet] OFF
GO

DROP TABLE [dbo].[OperationSet]
GO
EXEC sp_rename N'[dbo].[TempOperationSet]',N'OperationSet', 'OBJECT'
GO


ALTER VIEW [dbo].[Activities]
AS
SELECT     Id, OperationId, DateCreated, DateModified
FROM         dbo.ActivitySet
WHERE     (IsDeleted = 0)
GO
ALTER VIEW [dbo].[Operations]
AS
SELECT [Id]
      ,[UserId]
      , [UnitId]
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
ALTER VIEW [dbo].[InfoLineTextActivities]
AS
SELECT     Base.Id, Base.OperationId, Base.DateCreated, Base.DateModified, Derived.LineNumber, Derived.DestinationId, Derived.DisplayText, 
                      Derived.ExpirationDate, Derived.InfoRowId, Derived.Blink, Derived.DisplayedScreenSide, Derived.Alignment, Derived.Font
FROM         dbo.InfoLineTextActivitySet AS Derived INNER JOIN
                      dbo.ActivitySet AS Base ON Derived.Id = Base.Id
WHERE     (Base.IsDeleted = 0)
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 2011-12-07
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Activity_Insert]
(
	@operationId int
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
			( [OperationId]
			, [DateCreated]
			)
			VALUES
			( @operationId
			, @dateCreated
			)
			
			DECLARE @id int = SCOPE_IDENTITY()
			SELECT @id			
			COMMIT TRAN -- Transaction Success!

			RETURN @id
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
	, @unitId int
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
			, [UnitId]
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
			, @unitId
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
-- =============================================
-- Author:		Jerome Coutant
-- Create date: 09-12-2011
-- Description:	
-- =============================================
ALTER PROCEDURE [dbo].[ItcsConfiguration_Insert] 
	@ProtocolConfigurationId int,
	@name varchar(50),
	@description varchar(500) = null,
	@collectSystemData bit = 1,
	@operationDayStartUtc datetime,
	@operationDayDuration datetime,
	@utcOffset datetime,
	@dayLightSaving bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
			BEGIN TRANSACTION ItcsConfiguration_InsertTx -- Start the transaction..
			-- Insert statements for procedure here
			INSERT INTO [Gorba_CenterOnline].[dbo].[ItcsConfigurationSet]
				   ([ProtocolConfigurationId]
				   ,[name]
				   ,[description]
				   ,[collectSystemData]
				   ,[operationDayStartUtc]
				   ,[operationDayDuration]
				   ,[utcOffset]
				   ,[dayLightSaving])
			 VALUES
				   (@ProtocolConfigurationId,
					@name,
					@description,
					@collectSystemData,
					@operationDayStartUtc,
					@operationDayDuration,
					@utcOffset,
					@dayLightSaving)
			
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
-- Create date: 7.12.11 08:50
-- Description:	Adds an info line text to the system
-- =============================================
ALTER PROCEDURE [dbo].[InfoLineTextActivity_Insert] 
	( @operationId int
	, @dateCreated datetime = NULL
	, @lineNumber int
	, @destinationId int
	, @displayText varchar(100)
	, @expirationDate datetime
	, @infoRowId smallint
	, @blink bit
	, @displayedScreenSide smallint
	, @alignment smallint
	, @font smallint
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @id int;
	
	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION InfoLineTextActivity_InsertTx -- Start the transaction..
		-- Call base stored procedure Activity_Insert to insert first the fields of
		-- base entity
		EXEC @id = dbo.Activity_Insert @operationId = @operationId, @dateCreated = @dateCreated;
		
		-- insert the fields of the derived entity		
			INSERT INTO [InfoLineTextActivitySet]
			( [Id]
			, [LineNumber]
			, [DestinationId]
			, [DisplayText]
			, [ExpirationDate]
			, [InfoRowId]
			, [Blink]
			, [DisplayedScreenSide]
			, [Alignment]
			, [Font]
			)
			VALUES
			( @id
			, @lineNumber
			, @destinationId
			, @displayText 
			, @expirationDate
			, @infoRowId
			, @blink
			, @displayedScreenSide
			, @alignment
			, @font
			)
			
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
ALTER TABLE [dbo].[InfoLineTextActivitySet] ADD CONSTRAINT [PK_InfoLineTextActivitySet] PRIMARY KEY CLUSTERED
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
ALTER TABLE [dbo].[AssociationUnitOperationSet] ADD CONSTRAINT [FK_AssociationUnitOperationSet_OperationSet] FOREIGN KEY
	(
		[OperationId]
	)
	REFERENCES [dbo].[OperationSet]
	(
		[Id]
	)
GO
ALTER TABLE [dbo].[InfoLineTextActivitySet] ADD CONSTRAINT [FK_InfoLineTextActivitySet_ActivitySet] FOREIGN KEY
	(
		[Id]
	)
	REFERENCES [dbo].[ActivitySet]
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
ALTER TABLE [dbo].[OperationSet] ADD CONSTRAINT [FK_OperationSet_UnitSet] FOREIGN KEY
	(
		[UnitId]
	)
	REFERENCES [dbo].[UnitSet]
	(
		[Id]
	)
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: 04.0.12
-- Description:	Finds an operation by name. The search is case insensitive and finds the given string even if it is in the middle of the name.
-- =============================================
CREATE PROCEDURE [dbo].[Operation_ListByUnit]
(
	@unitId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT *
	FROM [Operations] [O]
	WHERE [O].[UnitId] = @unitId
END
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.3.12'
  ,@description = 'Fixed some minor issues. Moved UnitId from Activity to Operation.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 3
  ,@versionRevision = 12
  ,@dateCreated = '2012-01-20T11:40:00.000'
GO

