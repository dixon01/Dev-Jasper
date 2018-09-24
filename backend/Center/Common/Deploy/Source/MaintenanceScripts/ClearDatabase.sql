USE [Gorba_CenterOnline]
GO

BEGIN TRY --Start the Try Block..


	BEGIN TRANSACTION [ClearDatabase_Tx] -- Start the transaction..
	
	DELETE FROM [AssociationPermissionDataScopeUserRoleSet]
	DELETE FROM [AssociationTenantUserUserRoleSet]
	DELETE FROM [AssociationUnitOperationSet]
	DELETE FROM [AssociationUnitStationSet]
	DELETE FROM [AssociationUnitLineSet]
	
	DELETE FROM [AlarmSet]
	DELETE FROM [AlarmCategorySet]
	DELETE FROM [AlarmStatusTypeSet]
	DELETE FROM [AlarmTypeSet]
	DELETE FROM [AssociationUnitOperationSet]
	
	DELETE FROM [SeveritySet]
	DELETE FROM [TimeTableEntrySet]
	DELETE FROM [ProtocolConfigurationSet]
	DELETE FROM [ProtocolTypeSet]
	DELETE FROM [ScheduledSet]
	DELETE FROM [RealtimeSet]
	DELETE FROM [ItcsConfigurationSet]
	
	DELETE FROM [LayoutSet]
	DELETE FROM [UnitSet]
	DELETE FROM [ProductTypeSet]
	DELETE FROM [UnitTypeSet]
	DELETE FROM [UnitGroupSet]
	DELETE FROM [UnitGroupTypeSet]
	DELETE FROM [StationSet]
	
	DELETE FROM [InfoLineTextActivitySet]
	DELETE FROM [ActivitySet]
	DELETE FROM [OperationSet]
	
	DELETE FROM [DataScopeSet]
	DELETE FROM [PermissionSet]
	DELETE FROM [UserRoleSet]
	
	DELETE FROM [UserSet]
	DELETE FROM [TenantSet]

	COMMIT TRAN -- Transaction Success!

END TRY

	BEGIN CATCH


	IF @@TRANCOUNT > 0


		ROLLBACK TRAN --RollBack in case of Error



-- you can Raise ERROR with RAISEERROR() Statement including the details of the exception

	DECLARE @errorMessage varchar(5000) = ERROR_MESSAGE()


	RAISERROR(@errorMessage, 11, 1)

END CATCH