 

USE Gorba_CenterOnline
GO

ALTER TABLE [dbo].[ItcsConfiguration] DROP CONSTRAINT [FK_ItcsConfiguration_ProtocolConfiguration]
GO
ALTER TABLE [dbo].[ProtocolConfiguration] DROP CONSTRAINT [FK_ProtocolConfiguration_ProtocolType]
GO
DROP TABLE [dbo].[ItcsConfiguration]
GO
DROP TABLE [dbo].[ProtocolType]
GO
DROP TABLE [dbo].[ProtocolConfiguration]
GO
CREATE TABLE [dbo].[ItcsConfigurationSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ItcsConfigurationId] [int] NULL,
	[name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[description] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[collectSystemData] [bit] NOT NULL CONSTRAINT [DF_ItcsConfiguration_collectSystemData] DEFAULT ((1)),
	[operationDayStartUtc] [datetime] NOT NULL,
	[operationDayDuration] [datetime] NOT NULL,
	[utcOffset] [datetime] NOT NULL,
	[dayLightSaving] [bit] NOT NULL CONSTRAINT [DF_ItcsConfiguration_dayLightSaving] DEFAULT ((0)),
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ItcsConfiguration_IsDeleted] DEFAULT ((0)),
	CONSTRAINT [PK_ItcsConfiguration] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ItcsConfigurationName] ON [dbo].[ItcsConfigurationSet]
(
	[name] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ProtocolConfigurationSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[ProtocolTypeId] [int] NOT NULL,
	[realTimeDataOnly] [bit] NULL,
	[httpListenerHost] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpListenerPort] [int] NULL,
	[httpServerHost] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpServerport] [int] NULL,
	[httpWebProxyHost] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpWebProxyPort] [int] NULL,
	[httpClientIdentification] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpServerIdentification] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[httpResponseTimeOut] [int] NULL,
	[xmlClientRequestSenderId] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[xmlServerRequestSenderId] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[xmlNameSpaceRequest] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[xmlNameSpaceResponse] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[omitXmlDeclaration] [bit] NULL,
	[evaluateDataReadyInStatusResponse] [bit] NULL,
	[statusRequestIntervalInSec] [int] NULL,
	[subscriptionRetryIntervalInSec] [int] NULL,
	CONSTRAINT [PK_ProtocolConfiguration] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProtocolConfiguration] ON [dbo].[ProtocolConfigurationSet]
(
	[Id] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ProtocolTypeSet]
(
	[Id] [int] IDENTITY (1,1) NOT NULL,
	[name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[description] [varchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT [PK_ProtocolType] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER VIEW [dbo].[ItcsConfigurations]
AS
SELECT     Id, ItcsConfigurationId, name, description, collectSystemData, operationDayStartUtc, operationDayDuration, utcOffset, dayLightSaving
FROM         dbo.ItcsConfigurationSet
WHERE     (IsDeleted = 0)
GO
ALTER VIEW [dbo].[ProtocolTypes]
AS
SELECT     Id, name, description
FROM         dbo.ProtocolTypeSet
GO
-- =============================================
-- Author:		Jerome Coutant
-- Create date: 09-12-2011
-- Description:	
-- =============================================
ALTER PROCEDURE [dbo].[ItcsConfiguration_Insert] 
	@ItcsConfigurationId int,
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
				   ([ItcsConfigurationId]
				   ,[name]
				   ,[description]
				   ,[collectSystemData]
				   ,[operationDayStartUtc]
				   ,[operationDayDuration]
				   ,[utcOffset]
				   ,[dayLightSaving])
			 VALUES
				   (@ItcsConfigurationId,
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
-- Author:		Francesco Leonetti (francesco.leonetti@gorba.com)
-- Create date: 22.12.11
-- Description:	Updates an operation
-- =============================================
ALTER PROCEDURE [dbo].[Operation_Update] 
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
-- =============================================
-- Author:		Jerome Coutant
-- Create date: 09-12-2011
-- Description:	Insert new ProtocolConfiguration row with only needed vdv fields. 
--				Note : The different fieds are set or not according to the underlying used protocol.
-- =============================================
ALTER PROCEDURE [dbo].[VdvProtocolConfiguration_Insert] 
	-- Add the parameters for the stored procedure here
	@ProtocolTypeId int,
	@realTimeDataOnly bit = 0,
	@httpListenerHost varchar(50),
	@httpListenerPort int,
	@httpServerHost varchar(50),
	@httpServerport int,
	@httpWebProxyHost varchar(50) = null,
	@httpWebProxyPort int = 0,
	@httpClientIdentification varchar(50),
	@httpServerIdentification varchar(50),
	@httpResponseTimeOut int = 10,
	@xmlClientRequestSenderId varchar(50) = "client",
	@xmlServerRequestSenderId varchar(50) = "server",
	@xmlNameSpaceRequest varchar(50) = null,
	@xmlNameSpaceResponse varchar(50) = null,
	@omitXmlDeclaration bit = 1,
	@evaluateDataReadyInStatusResponse bit = 1,
	@statusRequestIntervalInSec int = 60,
	@subscriptionRetryIntervalInSec int = 60
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION VdvProtocolConfig_InsertTx -- Start the transaction.				
					
    -- Insert statements for procedure here
		INSERT INTO [Gorba_CenterOnline].[dbo].[ProtocolConfigurationSet]
			   ( [ProtocolTypeId]
			   , [realTimeDataOnly]
			   , [httpListenerHost]
			   , [httpListenerPort]
			   , [httpServerHost]
			   , [httpServerport]
			   , [httpWebProxyHost]
			   , [httpWebProxyPort]
			   , [httpClientIdentification]
			   , [httpServerIdentification]
			   , [httpResponseTimeOut]
			   , [xmlClientRequestSenderId]
			   , [xmlServerRequestSenderId]
			   , [xmlNameSpaceRequest]
			   , [xmlNameSpaceResponse]
			   , [omitXmlDeclaration]
			   , [evaluateDataReadyInStatusResponse]
			   , [statusRequestIntervalInSec]
			   , [subscriptionRetryIntervalInSec])
		 VALUES
			   ( @ProtocolTypeId,
				 @realTimeDataOnly,
				 @httpListenerHost,
				 @httpListenerPort,
				 @httpServerHost,
				 @httpServerport,
				 @httpWebProxyHost,
				 @httpWebProxyPort,
				 @httpClientIdentification,
				 @httpServerIdentification,
				 @httpResponseTimeOut,
				 @xmlClientRequestSenderId,
				 @xmlServerRequestSenderId,
				 @xmlNameSpaceRequest,
				 @xmlNameSpaceResponse,
				 @omitXmlDeclaration,
				 @evaluateDataReadyInStatusResponse,
				 @statusRequestIntervalInSec,
				 @subscriptionRetryIntervalInSec)
			
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
ALTER PROCEDURE [dbo].[ProtocolType_Insert] 
	@name varchar(50),
	@description varchar(500) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY --Start the Try Block..
		BEGIN TRANSACTION ProtocolType_InsertTx -- Start the transaction..
		-- Insert statements for procedure here		
			INSERT INTO [ProtocolTypeSet]
			([Name]
			, [Description])
			VALUES
			(@name
			, @description)
			
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
CREATE VIEW [dbo].[ProtocolConfigurations]
AS
SELECT     Id, ProtocolTypeId, realTimeDataOnly, httpListenerHost, httpListenerPort, httpServerHost, httpServerport, httpWebProxyHost, httpWebProxyPort, 
                      httpClientIdentification, httpServerIdentification, httpResponseTimeOut, xmlClientRequestSenderId, xmlServerRequestSenderId, xmlNameSpaceRequest, 
                      xmlNameSpaceResponse, omitXmlDeclaration, evaluateDataReadyInStatusResponse, statusRequestIntervalInSec, subscriptionRetryIntervalInSec
FROM         dbo.ProtocolConfigurationSet
GO
ALTER TABLE [dbo].[SeveritySet] ADD CONSTRAINT [DF_SeveritySet_IsDeleted] DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[AlarmCategorySet] ADD CONSTRAINT [DF_AlarmCategorySet_IsDeleted] DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[AlarmSet] ADD CONSTRAINT [DF_AlarmSet_IsDeleted] DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[AlarmTypeSet] ADD CONSTRAINT [DF_AlarmTypeSet_IsDeleted] DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[AlarmStatusTypeSet] ADD CONSTRAINT [DF_AlarmStatusTypeSet_IsDeleted] DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ItcsConfigurationSet] ADD CONSTRAINT [FK_ItcsConfiguration_ProtocolConfiguration] FOREIGN KEY
	(
		[ItcsConfigurationId]
	)
	REFERENCES [dbo].[ProtocolConfigurationSet]
	(
		[Id]
	)
GO

ALTER TABLE [dbo].[ProtocolConfigurationSet] ADD CONSTRAINT [FK_ProtocolConfiguration_ProtocolType] FOREIGN KEY
	(
		[ProtocolTypeId]
	)
	REFERENCES [dbo].[ProtocolTypeSet]
	(
		[Id]
	)
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.2.9'
  ,@description = 'Changes to meet naming conventions'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 2
  ,@versionRevision = 9
  ,@dateCreated = '2012-01-10T18:27:00.000'
GO