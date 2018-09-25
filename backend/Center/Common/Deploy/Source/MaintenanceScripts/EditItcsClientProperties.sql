DECLARE @id int = 0
DECLARE @properties XML

IF @id = 0
BEGIN
	print 'The id is 0. You should change it to the desired provider id.'
END

SELECT @properties = [P].[Properties]
FROM [Gorba_CenterOnline].[dbo].[ItcsProviderSet] [P]
WHERE [Id] = @id

SET @properties.modify('replace value of (/VdvConfiguration/Vdv/DefaultSubscriptionConfiguration/HandleRealtimeHysteresisLocally/text())[1] with "true"')
SET @properties.modify('replace value of (/VdvConfiguration/Vdv/DefaultSubscriptionConfiguration/MaxTripsNumber/text())[1] with "9"')

UPDATE [Gorba_CenterOnline].[dbo].[ItcsProviderSet]
SET [Properties] = CONVERT(varchar(max), @properties)
WHERE [Id] = @id