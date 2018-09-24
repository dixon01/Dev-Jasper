namespace Gorba.Center.Admin.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    public partial class AdminShellController
    {
        private void LoadControllers()
        {
            this.stageControllers.Add(
                new Entities.AccessControl.UserRoleStageController(dataController.UserRole));
            this.stageControllers.Add(
                new Entities.AccessControl.AuthorizationStageController(dataController.Authorization));
            this.stageControllers.Add(
                new Entities.Membership.TenantStageController(dataController.Tenant));
            this.stageControllers.Add(
                new Entities.Membership.UserStageController(dataController.User));
            this.stageControllers.Add(
                new Entities.Membership.AssociationTenantUserUserRoleStageController(dataController.AssociationTenantUserUserRole));
            this.stageControllers.Add(
                new Entities.Units.ProductTypeStageController(dataController.ProductType));
            this.stageControllers.Add(
                new Entities.Units.UnitStageController(dataController.Unit));
            this.stageControllers.Add(
                new Entities.Resources.ResourceStageController(dataController.Resource));
            this.stageControllers.Add(
                new Entities.Update.UpdateGroupStageController(dataController.UpdateGroup));
            this.stageControllers.Add(
                new Entities.Update.UpdatePartStageController(dataController.UpdatePart));
            this.stageControllers.Add(
                new Entities.Update.UpdateCommandStageController(dataController.UpdateCommand));
            this.stageControllers.Add(
                new Entities.Update.UpdateFeedbackStageController(dataController.UpdateFeedback));
            this.stageControllers.Add(
                new Entities.Documents.DocumentStageController(dataController.Document));
            this.stageControllers.Add(
                new Entities.Documents.DocumentVersionStageController(dataController.DocumentVersion));
            this.stageControllers.Add(
                new Entities.Software.PackageStageController(dataController.Package));
            this.stageControllers.Add(
                new Entities.Software.PackageVersionStageController(dataController.PackageVersion));
            this.stageControllers.Add(
                new Entities.Configurations.UnitConfigurationStageController(dataController.UnitConfiguration));
            this.stageControllers.Add(
                new Entities.Configurations.MediaConfigurationStageController(dataController.MediaConfiguration));
            this.stageControllers.Add(
                new Entities.Meta.UserDefinedPropertyStageController(dataController.UserDefinedProperty));
            this.stageControllers.Add(
                new Entities.Meta.SystemConfigStageController(dataController.SystemConfig));
        }
    }
}