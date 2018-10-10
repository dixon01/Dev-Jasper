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

	[Cmdlet(VerbsCommon.Add, "UserRoleWritableModel")]
	public class AddUserRoleWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UserRoleChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public UserRoleWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "AuthorizationWritableModel")]
	public class AddAuthorizationWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public AuthorizationChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public AuthorizationWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "TenantWritableModel")]
	public class AddTenantWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public TenantChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public TenantWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "UserWritableModel")]
	public class AddUserWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UserChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public UserWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "AssociationTenantUserUserRoleWritableModel")]
	public class AddAssociationTenantUserUserRoleWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public AssociationTenantUserUserRoleChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public AssociationTenantUserUserRoleWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "ProductTypeWritableModel")]
	public class AddProductTypeWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public ProductTypeChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public ProductTypeWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "UnitWritableModel")]
	public class AddUnitWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UnitChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public UnitWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "ResourceWritableModel")]
	public class AddResourceWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public ResourceChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public ResourceWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "UpdateGroupWritableModel")]
	public class AddUpdateGroupWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UpdateGroupChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public UpdateGroupWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "UpdatePartWritableModel")]
	public class AddUpdatePartWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UpdatePartChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public UpdatePartWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "UpdateCommandWritableModel")]
	public class AddUpdateCommandWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UpdateCommandChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public UpdateCommandWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "UpdateFeedbackWritableModel")]
	public class AddUpdateFeedbackWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UpdateFeedbackChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public UpdateFeedbackWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "DocumentWritableModel")]
	public class AddDocumentWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public DocumentChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public DocumentWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "DocumentVersionWritableModel")]
	public class AddDocumentVersionWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public DocumentVersionChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public DocumentVersionWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "PackageWritableModel")]
	public class AddPackageWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public PackageChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public PackageWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "PackageVersionWritableModel")]
	public class AddPackageVersionWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public PackageVersionChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public PackageVersionWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "UnitConfigurationWritableModel")]
	public class AddUnitConfigurationWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UnitConfigurationChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public UnitConfigurationWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "MediaConfigurationWritableModel")]
	public class AddMediaConfigurationWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public MediaConfigurationChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public MediaConfigurationWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "UserDefinedPropertyWritableModel")]
	public class AddUserDefinedPropertyWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public UserDefinedPropertyChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public UserDefinedPropertyWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	[Cmdlet(VerbsCommon.Add, "SystemConfigWritableModel")]
	public class AddSystemConfigWritableModel : AsyncCmdlet
	{
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Parameter(Mandatory = true)]
        public SystemConfigChangeTrackingManager ChangeTrackingManager { get; set; }

		[Parameter(Mandatory = true)]
		public SystemConfigWritableModel WritableModel { get; set; }

        protected override async Task ProcessRecordAsync()
        {
            try
            {
				await this.ChangeTrackingManager.AddAsync(this.WritableModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while bootstrapping change tracking");
                this.WriteError(
                    new ErrorRecord(
                        exception,
                        "GetUserReadableModel",
                        ErrorCategory.ReadError,
                        this.ChangeTrackingManager));
                throw new Exception(exception.StackTrace);
            }
        }
	}
	
}
