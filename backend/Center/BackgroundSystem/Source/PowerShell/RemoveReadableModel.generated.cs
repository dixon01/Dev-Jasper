namespace Gorba.Center.BackgroundSystem.PowerShell
{
    using System;
    using System.Management.Automation;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Client.ChangeTracking;

    using Gorba.Center.Common.Client.ChangeTracking.AccessControl;
    using Gorba.Center.Common.Client.ChangeTracking.Membership;
    using Gorba.Center.Common.Client.ChangeTracking.Units;
    using Gorba.Center.Common.Client.ChangeTracking.Resources;
    using Gorba.Center.Common.Client.ChangeTracking.Update;
    using Gorba.Center.Common.Client.ChangeTracking.Documents;
    using Gorba.Center.Common.Client.ChangeTracking.Software;
    using Gorba.Center.Common.Client.ChangeTracking.Configurations;
    using Gorba.Center.Common.Client.ChangeTracking.Meta;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;

    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;

    using NLog;

	[Cmdlet(VerbsCommon.Remove, "UserRoleReadableModel")]
	public class RemoveUserRoleReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UserRoleChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public UserRoleReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "AuthorizationReadableModel")]
	public class RemoveAuthorizationReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public AuthorizationChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public AuthorizationReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "TenantReadableModel")]
	public class RemoveTenantReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public TenantChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public TenantReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "UserReadableModel")]
	public class RemoveUserReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UserChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public UserReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "AssociationTenantUserUserRoleReadableModel")]
	public class RemoveAssociationTenantUserUserRoleReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public AssociationTenantUserUserRoleChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public AssociationTenantUserUserRoleReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "ProductTypeReadableModel")]
	public class RemoveProductTypeReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public ProductTypeChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public ProductTypeReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "UnitReadableModel")]
	public class RemoveUnitReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UnitChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public UnitReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "ResourceReadableModel")]
	public class RemoveResourceReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public ResourceChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public ResourceReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "UpdateGroupReadableModel")]
	public class RemoveUpdateGroupReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UpdateGroupChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public UpdateGroupReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "UpdatePartReadableModel")]
	public class RemoveUpdatePartReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UpdatePartChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public UpdatePartReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "UpdateCommandReadableModel")]
	public class RemoveUpdateCommandReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UpdateCommandChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public UpdateCommandReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "UpdateFeedbackReadableModel")]
	public class RemoveUpdateFeedbackReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UpdateFeedbackChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public UpdateFeedbackReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "DocumentReadableModel")]
	public class RemoveDocumentReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public DocumentChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public DocumentReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "DocumentVersionReadableModel")]
	public class RemoveDocumentVersionReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public DocumentVersionChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public DocumentVersionReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "PackageReadableModel")]
	public class RemovePackageReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public PackageChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public PackageReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "PackageVersionReadableModel")]
	public class RemovePackageVersionReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public PackageVersionChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public PackageVersionReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "UnitConfigurationReadableModel")]
	public class RemoveUnitConfigurationReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UnitConfigurationChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public UnitConfigurationReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "MediaConfigurationReadableModel")]
	public class RemoveMediaConfigurationReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public MediaConfigurationChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public MediaConfigurationReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "UserDefinedPropertyReadableModel")]
	public class RemoveUserDefinedPropertyReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UserDefinedPropertyChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public UserDefinedPropertyReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Remove, "SystemConfigReadableModel")]
	public class RemoveSystemConfigReadableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public SystemConfigChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter]
		public SystemConfigReadableModel InputObject { get; set; }

		[Parameter]
		public SwitchParameter LoadReferenceProperties { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.DeleteAsync(this.InputObject);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "RemoveUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	
}
