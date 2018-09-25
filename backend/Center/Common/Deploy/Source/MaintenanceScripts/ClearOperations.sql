DECLARE @RC int

-- TODO: Set parameter values here.

EXECUTE @RC = [Gorba_CenterControllersMetabase].[dbo].[ClearControllers] 
GO


DECLARE @RC int

-- TODO: Set parameter values here.

EXECUTE @RC = [Gorba_CenterControllers].[dbo].[ClearInstances] 
GO


DECLARE @RC int

-- TODO: Set parameter values here.

EXECUTE @RC = [Gorba_CenterOnline].[dbo].[ClearOperations] 
GO

