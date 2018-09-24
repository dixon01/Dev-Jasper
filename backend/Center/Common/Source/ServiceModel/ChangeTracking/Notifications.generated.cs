namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Log;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;

    namespace AccessControl
    {
        public partial class UserRoleDeltaNotification : DeltaNotification<UserRoleDeltaMessage>
        {
            public int EntityId { get; set; }
        }

        public partial class AuthorizationDeltaNotification : DeltaNotification<AuthorizationDeltaMessage>
        {
            public int EntityId { get; set; }
        }
    }

    namespace Membership
    {
        public partial class TenantDeltaNotification : DeltaNotification<TenantDeltaMessage>
        {
            public int EntityId { get; set; }
        }

        public partial class UserDeltaNotification : DeltaNotification<UserDeltaMessage>
        {
            public int EntityId { get; set; }
        }

        public partial class AssociationTenantUserUserRoleDeltaNotification : DeltaNotification<AssociationTenantUserUserRoleDeltaMessage>
        {
            public int EntityId { get; set; }
        }
    }

    namespace Units
    {
        public partial class ProductTypeDeltaNotification : DeltaNotification<ProductTypeDeltaMessage>
        {
            public int EntityId { get; set; }
        }

        public partial class UnitDeltaNotification : DeltaNotification<UnitDeltaMessage>
        {
            public int EntityId { get; set; }
        }
    }

    namespace Resources
    {
        public partial class ResourceDeltaNotification : DeltaNotification<ResourceDeltaMessage>
        {
            public int EntityId { get; set; }
        }

        public partial class ContentResourceDeltaNotification : DeltaNotification<ContentResourceDeltaMessage>
        {
            public int EntityId { get; set; }
        }
    }

    namespace Update
    {
        public partial class UpdateGroupDeltaNotification : DeltaNotification<UpdateGroupDeltaMessage>
        {
            public int EntityId { get; set; }
        }

        public partial class UpdatePartDeltaNotification : DeltaNotification<UpdatePartDeltaMessage>
        {
            public int EntityId { get; set; }
        }

        public partial class UpdateCommandDeltaNotification : DeltaNotification<UpdateCommandDeltaMessage>
        {
            public int EntityId { get; set; }
        }

        public partial class UpdateFeedbackDeltaNotification : DeltaNotification<UpdateFeedbackDeltaMessage>
        {
            public int EntityId { get; set; }
        }
    }

    namespace Documents
    {
        public partial class DocumentDeltaNotification : DeltaNotification<DocumentDeltaMessage>
        {
            public int EntityId { get; set; }
        }

        public partial class DocumentVersionDeltaNotification : DeltaNotification<DocumentVersionDeltaMessage>
        {
            public int EntityId { get; set; }
        }
    }

    namespace Software
    {
        public partial class PackageDeltaNotification : DeltaNotification<PackageDeltaMessage>
        {
            public int EntityId { get; set; }
        }

        public partial class PackageVersionDeltaNotification : DeltaNotification<PackageVersionDeltaMessage>
        {
            public int EntityId { get; set; }
        }
    }

    namespace Configurations
    {
        public partial class UnitConfigurationDeltaNotification : DeltaNotification<UnitConfigurationDeltaMessage>
        {
            public int EntityId { get; set; }
        }

        public partial class MediaConfigurationDeltaNotification : DeltaNotification<MediaConfigurationDeltaMessage>
        {
            public int EntityId { get; set; }
        }
    }

    namespace Log
    {    }

    namespace Meta
    {
        public partial class UserDefinedPropertyDeltaNotification : DeltaNotification<UserDefinedPropertyDeltaMessage>
        {
            public int EntityId { get; set; }
        }

        public partial class SystemConfigDeltaNotification : DeltaNotification<SystemConfigDeltaMessage>
        {
            public int EntityId { get; set; }
        }
    }
}
