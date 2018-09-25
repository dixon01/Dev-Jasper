 

USE Gorba_CenterOnline
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ClearItcsData]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    DECLARE @now datetime2(7)
    
    DELETE FROM [ReferenceTextSet]
    WHERE [ValidUntil] IS NOT NULL AND [ValidUntil] < @now
    
    DELETE FROM [VdvMessageSet]
    WHERE [ValidUntil] IS NOT NULL AND [ValidUntil] < @now
END
GO

--=============================================================
-- Versioning
--=============================================================
DECLARE @RC int

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[DatabaseVersion_Insert] 
   @name = 'Version 1.0.16.46'
  ,@description = 'Added the ClearItcsData SP.'
  ,@versionMajor = 1
  ,@versionMinor = 0
  ,@versionBuild = 16
  ,@versionRevision = 46
  ,@dateCreated = '2012-07-23T08:47:00.000'
GO