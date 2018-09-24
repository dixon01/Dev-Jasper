// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Configuration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Gorba.Center.BackgroundSystem.Data.Model.Units;

namespace Gorba.Center.BackgroundSystem.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Diagnostics;
    using System.Linq;

    using Gorba.Center.BackgroundSystem.Data;
    using Gorba.Center.BackgroundSystem.Data.Model.AccessControl;
    using Gorba.Center.BackgroundSystem.Data.Model.Membership;
    using Gorba.Center.BackgroundSystem.Data.Model.Meta;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Common.Utility.Core;

    using NLog;

    using Extensions = Gorba.Center.BackgroundSystem.Data.Model.Extensions;

    /// <summary>
    /// Defines the configuration for migrations of the <see cref="CenterDataContext"/>.
    /// </summary>
    internal sealed class Configuration : DbMigrationsConfiguration<CenterDataContext>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = false;
        }

        /// <summary>
        /// Runs after upgrading to the latest migration to allow seed data to be updated.
        /// </summary>
        /// <param name="context">Context to be used for updating seed data. </param>
        protected override void Seed(CenterDataContext context)
        {
            CheckCreateAdminUser(context);

#if __UseLuminatorTftDisplay
            // Luminator alternative to initial powershell setup script
            CheckCreateTenantAdminUser(context);
            CheckCreateUnitAdminUser(context);
            CheckCreateProductTypes(context);
#endif

            CheckCreateSystemConfiguration(context);
        }

        private static void CheckCreateSystemConfiguration(CenterDataContext context)
        {
            if (context.SystemConfigs.Any())
            {
                return;
            }

            var settings = new XmlData(new BackgroundSystemSettings());
            var systemConfig = new SystemConfig
                                   {
                                       SystemId = Guid.NewGuid(),
                                       Settings = Extensions.ToDatabase(settings)
                                   };
            Logger.Info("Creating system configuration in database with GUID {0}", systemConfig.SystemId);
            context.SystemConfigs.Add(systemConfig);
            context.SaveChanges();
        }

        private static void CheckCreateAdminUser(CenterDataContext context)
        {
            var role = CheckCreateSuperuserRole(context);

            var admin =
                context.Users.Include(u => u.AssociationTenantUserUserRoles)
                    .FirstOrDefault(u => u.Username == CommonNames.AdminUsername);
            if (admin == null)
            {
                var tenant = CheckCreateAdminTenant(context);

                var password = "luminator!";//SecurityUtility.GenerateRandomPassword(10);
                Trace.TraceWarning(
                    "Couldn't find user '{0}', creating it with the automatically generated password {1}",
                    CommonNames.AdminUsername,
                    password);
                Logger.Warn(
                    "Couldn't find user '{0}', creating it with the automatically generated password {1}",
                    CommonNames.AdminUsername,
                    password);

                admin = new User
                            {
                                Username = CommonNames.AdminUsername,
                                Description = "Default administrator user",
                                CreatedOn = TimeProvider.Current.UtcNow,
                                Culture = "en-US",
                                FirstName = "Administrator",
                                LastName = "Gorba",
                                IsEnabled = true,
                                HashedPassword = SecurityUtility.Md5(password),
                                OwnerTenant = tenant,
                                AssociationTenantUserUserRoles = new List<AssociationTenantUserUserRole>()
                            };
                admin = context.Users.Add(admin);
            }

            if (context.AssociationTenantUserUserRoles.Any(a => a.User.Id == admin.Id && a.UserRole.Id == role.Id))
            {
                return;
            }

            Logger.Info(
                "Adding missing association between '{0}' and '{1}' for null-tenant", admin.Username, role.Name);
            admin.AssociationTenantUserUserRoles.Add(new AssociationTenantUserUserRole
                                                         {
                                                             CreatedOn = TimeProvider.Current.UtcNow,
                                                             Tenant = null,
                                                             User = admin,
                                                             UserRole = role
                                                         });
            context.SaveChanges();
        }

        private static UserRole CheckCreateSuperuserRole(CenterDataContext context)
        {
            var role =
                context.UserRoles.Include(r => r.Authorizations)
                    .FirstOrDefault(u => u.Name == CommonNames.SuperuserRoleName);
            if (role == null)
            {
                Logger.Info("Couldn't find user role '{0}', creating it", CommonNames.SuperuserRoleName);
                role = new UserRole
                           {
                               CreatedOn = TimeProvider.Current.UtcNow,
                               Name = CommonNames.SuperuserRoleName,
                               Description = "User role that has all possible rights",
                               Authorizations = new List<Authorization>()
                           };
                role = context.UserRoles.Add(role);
            }

            var allDataScopes = Enum.GetValues(typeof(DataScope)).Cast<DataScope>().ToList();
            var allPermissions = Enum.GetValues(typeof(Permission)).Cast<Permission>().ToList();
            foreach (var dataScope in allDataScopes)
            {
                foreach (var permission in allPermissions)
                {
                    if (!role.Authorizations.Any(a => a.DataScope == dataScope && a.Permission == permission))
                    {
                        Logger.Info(
                            "Adding missing authorization to role '{0}': {1} for {2}",
                            role.Name,
                            permission,
                            dataScope);
                        role.Authorizations.Add(new Authorization
                                                    {
                                                        CreatedOn = TimeProvider.Current.UtcNow,
                                                        DataScope = dataScope,
                                                        Permission = permission
                                                    });
                    }
                }
            }

            context.SaveChanges();
            return role;
        }

#if __UseLuminatorTftDisplay

        private static void CheckCreateTenantAdminUser(CenterDataContext context)
        {
            UserRole role = CheckCreateTenantAdminRole(context);

            User admin =
                context.Users.Include(u => u.AssociationTenantUserUserRoles)
                    .FirstOrDefault(u => u.Username == CommonNames.TenantAdminUsername);
            if (admin == null)
            {
                var tenant = CheckCreateTenant(context, CommonNames.TenantAdminRoleName);

                var password = "luminator!";//SecurityUtility.GenerateRandomPassword(10);
                Trace.TraceWarning(
                    "Couldn't find user '{0}', creating it with the automatically generated password {1}",
                    CommonNames.TenantAdminUsername,
                    password);
                Logger.Warn(
                    "Couldn't find user '{0}', creating it with the automatically generated password {1}",
                    CommonNames.TenantAdminUsername,
                    password);

                admin = new User
                {
                    Username = CommonNames.TenantAdminUsername,
                    Description = "Default tenant administrator user",
                    CreatedOn = TimeProvider.Current.UtcNow,
                    Culture = "en-US",
                    FirstName = "Tenant Administrator",
                    LastName = "Luminator",
                    IsEnabled = true,
                    HashedPassword = SecurityUtility.Md5(password),
                    OwnerTenant = tenant,
                    AssociationTenantUserUserRoles = new List<AssociationTenantUserUserRole>()
                };
                admin = context.Users.Add(admin);
            }

            if (context.AssociationTenantUserUserRoles.Any(a => a.User.Id == admin.Id && a.UserRole.Id == role.Id))
            {
                return;
            }

            Logger.Info(
                "Adding missing association between '{0}' and '{1}' for null-tenant", admin.Username, role.Name);
            admin.AssociationTenantUserUserRoles.Add(new AssociationTenantUserUserRole
            {
                CreatedOn = TimeProvider.Current.UtcNow,
                Tenant = null,
                User = admin,
                UserRole = role
            });
            context.SaveChanges();
        }
        
        private static void CheckCreateUnitAdminUser(CenterDataContext context)
        {
            UserRole role = CheckCreateUnitAdminRole(context);

            User admin =
                context.Users.Include(u => u.AssociationTenantUserUserRoles)
                    .FirstOrDefault(u => u.Username == CommonNames.UnitAdminUsername);
            if (admin == null)
            {
                var tenant = CheckCreateTenant(context, CommonNames.UnitAdminRoleName);

                var password = "luminator!";//SecurityUtility.GenerateRandomPassword(10);
                Trace.TraceWarning(
                    "Couldn't find user '{0}', creating it with the automatically generated password {1}",
                    CommonNames.UnitAdminUsername,
                    password);
                Logger.Warn(
                    "Couldn't find user '{0}', creating it with the automatically generated password {1}",
                    CommonNames.UnitAdminUsername,
                    password);

                admin = new User
                {
                    Username = CommonNames.UnitAdminUsername,
                    Description = "Default unit administrator user",
                    CreatedOn = TimeProvider.Current.UtcNow,
                    Culture = "en-US",
                    FirstName = "Unit Administrator",
                    LastName = "Luminator",
                    IsEnabled = true,
                    HashedPassword = SecurityUtility.Md5(password),
                    OwnerTenant = tenant,
                    AssociationTenantUserUserRoles = new List<AssociationTenantUserUserRole>()
                };
                admin = context.Users.Add(admin);
            }

            if (context.AssociationTenantUserUserRoles.Any(a => a.User.Id == admin.Id && a.UserRole.Id == role.Id))
            {
                return;
            }

            Logger.Info(
                "Adding missing association between '{0}' and '{1}' for null-tenant", admin.Username, role.Name);
            admin.AssociationTenantUserUserRoles.Add(new AssociationTenantUserUserRole
            {
                CreatedOn = TimeProvider.Current.UtcNow,
                Tenant = null,
                User = admin,
                UserRole = role
            });
            context.SaveChanges();
        }
        
        private static UserRole CheckCreateTenantAdminRole(CenterDataContext context)
        {
            UserRole role =
                context.UserRoles.Include(r => r.Authorizations)
                    .FirstOrDefault(u => u.Name == CommonNames.TenantAdminRoleName);

            if (role == null)
            {
                Logger.Info("Couldn't find user role '{0}', creating it", CommonNames.TenantAdminRoleName);
                role = new UserRole
                {
                    CreatedOn = TimeProvider.Current.UtcNow,
                    Name = CommonNames.TenantAdminRoleName,
                    Description = "User role that manages tenants",
                    Authorizations = new List<Authorization>()
                };
                role = context.UserRoles.Add(role);
            }

            List<DataScope> dataScopes = new List<DataScope>();
            dataScopes.Add(DataScope.Tenant);
            dataScopes.Add(DataScope.Unit);
            dataScopes.Add(DataScope.Update);
            dataScopes.Add(DataScope.UnitConfiguration);
            dataScopes.Add(DataScope.MediaConfiguration);
            dataScopes.Add(DataScope.Meta);
            dataScopes.Add(DataScope.CenterAdmin);
            dataScopes.Add(DataScope.CenterMedia);
            dataScopes.Add(DataScope.CenterDiag);

            List<Permission> permissions = new List<Permission>();
            permissions.Add(Permission.Create);
            permissions.Add(Permission.Read);
            permissions.Add(Permission.Write);
            permissions.Add(Permission.Delete);
            permissions.Add(Permission.Interact);
            permissions.Add(Permission.Abort);

            foreach (var dataScope in dataScopes)
            {
                foreach (var permission in permissions)
                {
                    if (!role.Authorizations.Any(a => a.DataScope == dataScope && a.Permission == permission))
                    {
                        Logger.Info(
                            "Adding missing authorization to role '{0}': {1} for {2}",
                            role.Name,
                            permission,
                            dataScope);
                        role.Authorizations.Add(new Authorization
                        {
                            CreatedOn = TimeProvider.Current.UtcNow,
                            DataScope = dataScope,
                            Permission = permission
                        });
                    }
                }
            }

            context.SaveChanges();
            return role;
        }
        
        private static UserRole CheckCreateUnitAdminRole(CenterDataContext context)
        {
            UserRole role =
                context.UserRoles.Include(r => r.Authorizations)
                    .FirstOrDefault(u => u.Name == CommonNames.UnitAdminRoleName);

            if (role == null)
            {
                Logger.Info("Couldn't find user role '{0}', creating it", CommonNames.UnitAdminRoleName);
                role = new UserRole
                {
                    CreatedOn = TimeProvider.Current.UtcNow,
                    Name = CommonNames.UnitAdminRoleName,
                    Description = "User role that manages units",
                    Authorizations = new List<Authorization>()
                };
                role = context.UserRoles.Add(role);
            }

            List<DataScope> dataScopes = new List<DataScope>();
            dataScopes.Add(DataScope.Unit);
            dataScopes.Add(DataScope.Update);
            dataScopes.Add(DataScope.UnitConfiguration);
            dataScopes.Add(DataScope.MediaConfiguration);
            dataScopes.Add(DataScope.Meta);
            dataScopes.Add(DataScope.CenterAdmin);
            dataScopes.Add(DataScope.CenterMedia);
            dataScopes.Add(DataScope.CenterDiag);

            List<Permission> permissions = new List<Permission>();
            permissions.Add(Permission.Create);
            permissions.Add(Permission.Read);
            permissions.Add(Permission.Write);
            permissions.Add(Permission.Delete);
            permissions.Add(Permission.Interact);
            permissions.Add(Permission.Abort);

            foreach (var dataScope in dataScopes)
            {
                foreach (var permission in permissions)
                {
                    if (!role.Authorizations.Any(a => a.DataScope == dataScope && a.Permission == permission))
                    {
                        Logger.Info(
                            "Adding missing authorization to role '{0}': {1} for {2}",
                            role.Name,
                            permission,
                            dataScope);
                        role.Authorizations.Add(new Authorization
                        {
                            CreatedOn = TimeProvider.Current.UtcNow,
                            DataScope = dataScope,
                            Permission = permission
                        });
                    }
                }
            }

            context.SaveChanges();
            return role;
        }

        private static Tenant CheckCreateTenant(CenterDataContext context, string name)
        {
            Tenant tenant = context.Tenants.FirstOrDefault(t => t.Name == name);
            if (tenant != null)
            {
                return tenant;
            }

            Logger.Info("Couldn't find tenant '{0}', creating it", name);

            tenant = new Tenant
            {
                Name = name,
                Description = "Default administrator tenant; shouldn't be used for real operations",
                CreatedOn = TimeProvider.Current.UtcNow
            };
            return context.Tenants.Add(tenant);
        }

        private void CheckCreateProductTypes(CenterDataContext context)
        {
            string productType1 = @"Luminator TFT 18.5""";

            ProductType productType = context.ProductTypes.FirstOrDefault(p => p.Name == productType1);
            if (productType == null)
            {
                Logger.Info("Couldn't find product '{0}', creating it", productType1);

                // Create a hardware descriptor
                var descriptor = new HardwareDescriptor
                {
                    Name = productType1,
                    Platform =
                        new InfoTransitPlatformDescriptor
                        {
                            DisplayAdapters = { new DisplayAdapterDescriptor(0, DisplayConnectionType.Lvds) },
                            HasGenericButton = false,
                            HasGenericLed = true,
                            BuiltInScreen = new DisplayDescriptor(1366, 768, 1366, 768),
                            Inputs =
                                {
                                    new InputDescriptor(0, "IN0 (D-sub 9: pin 1)")
                                },
                            Outputs =
                                {
                                    new OutputDescriptor(1, "Audio Port 1"),
                                    new OutputDescriptor(2, "Audio Port 2"),
                                }
                        },
                    OperatingSystem =
                        new WindowsEmbeddedDescriptor
                        {
                            Version = OperatingSystemVersion.WindowsEmbedded8Standard
                        }
                };

                XmlData data = new XmlData(descriptor);
                Model.XmlData productData = new Model.XmlData()
                {
                    Xml = data.Xml,
                    Type = data.Type
                };

                productType = new ProductType()
                {
                    Name = productType1,
                    HardwareDescriptor = productData,
                    UnitType = 0,
                    Version = 1,
                    CreatedOn = TimeProvider.Current.UtcNow
                };

                context.ProductTypes.Add(productType);
                context.SaveChanges();
            }
        }

#endif

        private static Tenant CheckCreateAdminTenant(CenterDataContext context)
        {
            var tenant = context.Tenants.FirstOrDefault(t => t.Name == CommonNames.AdminTenantName);
            if (tenant != null)
            {
                return tenant;
            }

            Logger.Info("Couldn't find tenant '{0}', creating it", CommonNames.AdminTenantName);

            tenant = new Tenant
                         {
                             Name = CommonNames.AdminTenantName,
                             Description = "Default administrator tenant; shouldn't be used for real operations",
                             CreatedOn = TimeProvider.Current.UtcNow
                         };
            return context.Tenants.Add(tenant);
        }

    }
}