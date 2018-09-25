namespace Gorba.Center.Admin.Core.DataViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    public class DataViewModelFactory
    {
        public DataViewModelFactory(ICommandRegistry commandRegistry)
        {
            this.CommandRegistry = commandRegistry;
        }

        public ICommandRegistry CommandRegistry { get; private set; }

        public AccessControl.UserRoleReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl.UserRoleReadableModel readableModel, IUserRoleUdpContext context = null)
        {
            return new AccessControl.UserRoleReadOnlyDataViewModel(readableModel, context, this);
        }
        
        public AccessControl.UserRoleDataViewModel Create(
            AccessControl.UserRoleReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new AccessControl.UserRoleDataViewModel(readOnlyDataViewModel, this);
        }

        public AccessControl.UserRoleDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl.UserRoleWritableModel writableModel, IUserRoleUdpContext context = null)
        {
            return new AccessControl.UserRoleDataViewModel(writableModel, context, this);
        }

        public AccessControl.AuthorizationReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl.AuthorizationReadableModel readableModel)
        {
            return new AccessControl.AuthorizationReadOnlyDataViewModel(readableModel, this);
        }
        
        public AccessControl.AuthorizationDataViewModel Create(
            AccessControl.AuthorizationReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new AccessControl.AuthorizationDataViewModel(readOnlyDataViewModel, this);
        }

        public AccessControl.AuthorizationDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl.AuthorizationWritableModel writableModel)
        {
            return new AccessControl.AuthorizationDataViewModel(writableModel, this);
        }

        public Membership.TenantReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Membership.TenantReadableModel readableModel, ITenantUdpContext context = null)
        {
            return new Membership.TenantReadOnlyDataViewModel(readableModel, context, this);
        }
        
        public Membership.TenantDataViewModel Create(
            Membership.TenantReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Membership.TenantDataViewModel(readOnlyDataViewModel, this);
        }

        public Membership.TenantDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Membership.TenantWritableModel writableModel, ITenantUdpContext context = null)
        {
            return new Membership.TenantDataViewModel(writableModel, context, this);
        }

        public Membership.UserReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Membership.UserReadableModel readableModel, IUserUdpContext context = null)
        {
            return new Membership.UserReadOnlyDataViewModel(readableModel, context, this);
        }
        
        public Membership.UserDataViewModel Create(
            Membership.UserReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Membership.UserDataViewModel(readOnlyDataViewModel, this);
        }

        public Membership.UserDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Membership.UserWritableModel writableModel, IUserUdpContext context = null)
        {
            return new Membership.UserDataViewModel(writableModel, context, this);
        }

        public Membership.AssociationTenantUserUserRoleReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Membership.AssociationTenantUserUserRoleReadableModel readableModel)
        {
            return new Membership.AssociationTenantUserUserRoleReadOnlyDataViewModel(readableModel, this);
        }
        
        public Membership.AssociationTenantUserUserRoleDataViewModel Create(
            Membership.AssociationTenantUserUserRoleReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Membership.AssociationTenantUserUserRoleDataViewModel(readOnlyDataViewModel, this);
        }

        public Membership.AssociationTenantUserUserRoleDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Membership.AssociationTenantUserUserRoleWritableModel writableModel)
        {
            return new Membership.AssociationTenantUserUserRoleDataViewModel(writableModel, this);
        }

        public Units.ProductTypeReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Units.ProductTypeReadableModel readableModel)
        {
            return new Units.ProductTypeReadOnlyDataViewModel(readableModel, this);
        }
        
        public Units.ProductTypeDataViewModel Create(
            Units.ProductTypeReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Units.ProductTypeDataViewModel(readOnlyDataViewModel, this);
        }

        public Units.ProductTypeDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Units.ProductTypeWritableModel writableModel)
        {
            return new Units.ProductTypeDataViewModel(writableModel, this);
        }

        public Units.UnitReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Units.UnitReadableModel readableModel, IUnitUdpContext context = null)
        {
            return new Units.UnitReadOnlyDataViewModel(readableModel, context, this);
        }
        
        public Units.UnitDataViewModel Create(
            Units.UnitReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Units.UnitDataViewModel(readOnlyDataViewModel, this);
        }

        public Units.UnitDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Units.UnitWritableModel writableModel, IUnitUdpContext context = null)
        {
            return new Units.UnitDataViewModel(writableModel, context, this);
        }

        public Resources.ResourceReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Resources.ResourceReadableModel readableModel)
        {
            return new Resources.ResourceReadOnlyDataViewModel(readableModel, this);
        }
        
        public Resources.ResourceDataViewModel Create(
            Resources.ResourceReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Resources.ResourceDataViewModel(readOnlyDataViewModel, this);
        }

        public Resources.ResourceDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Resources.ResourceWritableModel writableModel)
        {
            return new Resources.ResourceDataViewModel(writableModel, this);
        }

        public Update.UpdateGroupReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Update.UpdateGroupReadableModel readableModel, IUpdateGroupUdpContext context = null)
        {
            return new Update.UpdateGroupReadOnlyDataViewModel(readableModel, context, this);
        }
        
        public Update.UpdateGroupDataViewModel Create(
            Update.UpdateGroupReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Update.UpdateGroupDataViewModel(readOnlyDataViewModel, this);
        }

        public Update.UpdateGroupDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Update.UpdateGroupWritableModel writableModel, IUpdateGroupUdpContext context = null)
        {
            return new Update.UpdateGroupDataViewModel(writableModel, context, this);
        }

        public Update.UpdatePartReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Update.UpdatePartReadableModel readableModel)
        {
            return new Update.UpdatePartReadOnlyDataViewModel(readableModel, this);
        }
        
        public Update.UpdatePartDataViewModel Create(
            Update.UpdatePartReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Update.UpdatePartDataViewModel(readOnlyDataViewModel, this);
        }

        public Update.UpdatePartDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Update.UpdatePartWritableModel writableModel)
        {
            return new Update.UpdatePartDataViewModel(writableModel, this);
        }

        public Update.UpdateCommandReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Update.UpdateCommandReadableModel readableModel)
        {
            return new Update.UpdateCommandReadOnlyDataViewModel(readableModel, this);
        }
        
        public Update.UpdateCommandDataViewModel Create(
            Update.UpdateCommandReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Update.UpdateCommandDataViewModel(readOnlyDataViewModel, this);
        }

        public Update.UpdateCommandDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Update.UpdateCommandWritableModel writableModel)
        {
            return new Update.UpdateCommandDataViewModel(writableModel, this);
        }

        public Update.UpdateFeedbackReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Update.UpdateFeedbackReadableModel readableModel)
        {
            return new Update.UpdateFeedbackReadOnlyDataViewModel(readableModel, this);
        }
        
        public Update.UpdateFeedbackDataViewModel Create(
            Update.UpdateFeedbackReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Update.UpdateFeedbackDataViewModel(readOnlyDataViewModel, this);
        }

        public Update.UpdateFeedbackDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Update.UpdateFeedbackWritableModel writableModel)
        {
            return new Update.UpdateFeedbackDataViewModel(writableModel, this);
        }

        public Documents.DocumentReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Documents.DocumentReadableModel readableModel)
        {
            return new Documents.DocumentReadOnlyDataViewModel(readableModel, this);
        }
        
        public Documents.DocumentDataViewModel Create(
            Documents.DocumentReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Documents.DocumentDataViewModel(readOnlyDataViewModel, this);
        }

        public Documents.DocumentDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Documents.DocumentWritableModel writableModel)
        {
            return new Documents.DocumentDataViewModel(writableModel, this);
        }

        public Documents.DocumentVersionReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Documents.DocumentVersionReadableModel readableModel)
        {
            return new Documents.DocumentVersionReadOnlyDataViewModel(readableModel, this);
        }
        
        public Documents.DocumentVersionDataViewModel Create(
            Documents.DocumentVersionReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Documents.DocumentVersionDataViewModel(readOnlyDataViewModel, this);
        }

        public Documents.DocumentVersionDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Documents.DocumentVersionWritableModel writableModel)
        {
            return new Documents.DocumentVersionDataViewModel(writableModel, this);
        }

        public Software.PackageReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Software.PackageReadableModel readableModel)
        {
            return new Software.PackageReadOnlyDataViewModel(readableModel, this);
        }
        
        public Software.PackageDataViewModel Create(
            Software.PackageReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Software.PackageDataViewModel(readOnlyDataViewModel, this);
        }

        public Software.PackageDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Software.PackageWritableModel writableModel)
        {
            return new Software.PackageDataViewModel(writableModel, this);
        }

        public Software.PackageVersionReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Software.PackageVersionReadableModel readableModel)
        {
            return new Software.PackageVersionReadOnlyDataViewModel(readableModel, this);
        }
        
        public Software.PackageVersionDataViewModel Create(
            Software.PackageVersionReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Software.PackageVersionDataViewModel(readOnlyDataViewModel, this);
        }

        public Software.PackageVersionDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Software.PackageVersionWritableModel writableModel)
        {
            return new Software.PackageVersionDataViewModel(writableModel, this);
        }

        public Configurations.UnitConfigurationReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations.UnitConfigurationReadableModel readableModel)
        {
            return new Configurations.UnitConfigurationReadOnlyDataViewModel(readableModel, this);
        }
        
        public Configurations.UnitConfigurationDataViewModel Create(
            Configurations.UnitConfigurationReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Configurations.UnitConfigurationDataViewModel(readOnlyDataViewModel, this);
        }

        public Configurations.UnitConfigurationDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations.UnitConfigurationWritableModel writableModel)
        {
            return new Configurations.UnitConfigurationDataViewModel(writableModel, this);
        }

        public Configurations.MediaConfigurationReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations.MediaConfigurationReadableModel readableModel)
        {
            return new Configurations.MediaConfigurationReadOnlyDataViewModel(readableModel, this);
        }
        
        public Configurations.MediaConfigurationDataViewModel Create(
            Configurations.MediaConfigurationReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Configurations.MediaConfigurationDataViewModel(readOnlyDataViewModel, this);
        }

        public Configurations.MediaConfigurationDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations.MediaConfigurationWritableModel writableModel)
        {
            return new Configurations.MediaConfigurationDataViewModel(writableModel, this);
        }

        public Meta.UserDefinedPropertyReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Meta.UserDefinedPropertyReadableModel readableModel)
        {
            return new Meta.UserDefinedPropertyReadOnlyDataViewModel(readableModel, this);
        }
        
        public Meta.UserDefinedPropertyDataViewModel Create(
            Meta.UserDefinedPropertyReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Meta.UserDefinedPropertyDataViewModel(readOnlyDataViewModel, this);
        }

        public Meta.UserDefinedPropertyDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Meta.UserDefinedPropertyWritableModel writableModel)
        {
            return new Meta.UserDefinedPropertyDataViewModel(writableModel, this);
        }

        public Meta.SystemConfigReadOnlyDataViewModel CreateReadOnly(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Meta.SystemConfigReadableModel readableModel)
        {
            return new Meta.SystemConfigReadOnlyDataViewModel(readableModel, this);
        }
        
        public Meta.SystemConfigDataViewModel Create(
            Meta.SystemConfigReadOnlyDataViewModel readOnlyDataViewModel)
        {
            return new Meta.SystemConfigDataViewModel(readOnlyDataViewModel, this);
        }

        public Meta.SystemConfigDataViewModel Create(
            Gorba.Center.Common.ServiceModel.ChangeTracking.Meta.SystemConfigWritableModel writableModel)
        {
            return new Meta.SystemConfigDataViewModel(writableModel, this);
        }
    }

    public interface IUserRoleUdpContext
    {
        IEnumerable<string> GetAdditionalUserRoleProperties();
    }

    public interface ITenantUdpContext
    {
        IEnumerable<string> GetAdditionalTenantProperties();
    }

    public interface IUserUdpContext
    {
        IEnumerable<string> GetAdditionalUserProperties();
    }

    public interface IUnitUdpContext
    {
        IEnumerable<string> GetAdditionalUnitProperties();
    }

    public interface IUpdateGroupUdpContext
    {
        IEnumerable<string> GetAdditionalUpdateGroupProperties();
    }
}