// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301ProtocolCategoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301ProtocolCategoryController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Protran.VDV301;

    /// <summary>
    /// The VDV 301 protocol category controller.
    /// </summary>
    public class Vdv301ProtocolCategoryController : CategoryControllerBase
    {
        private readonly List<IFilteredPartController> controllers = new List<IFilteredPartController>();

        private IncomingPartController incoming;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301ProtocolCategoryController"/> class.
        /// </summary>
        public Vdv301ProtocolCategoryController()
            : base(UnitConfigKeys.Vdv301Protocol.Category)
        {
            this.DataItemsRoot = new CheckableTreeNodeViewModel();
        }

        /// <summary>
        /// Gets the root node of the data items tree.
        /// </summary>
        public CheckableTreeNodeViewModel DataItemsRoot { get; private set; }

        /// <summary>
        /// Asynchronously prepares this category controller and all its children with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public override Task PrepareAsync(HardwareDescriptor descriptor)
        {
            this.incoming = this.Parent.GetPart<IncomingPartController>();
            this.incoming.ViewModelUpdated += (s, e) => this.UpdateVisibility();
            this.UpdateVisibility();

            return base.PrepareAsync(descriptor);
        }

        /// <summary>
        /// Creates the protocol configuration with the data from all part controllers.
        /// </summary>
        /// <returns>
        /// The <see cref="Vdv301ProtocolConfig"/>.
        /// </returns>
        public Vdv301ProtocolConfig CreateConfig()
        {
            var config = new Vdv301ProtocolConfig();

            var general = this.controllers.OfType<Vdv301GeneralPartController>().Single();
            config.Services.ValidateHttpRequests = general.ValidateHttpRequests;
            config.Services.ValidateHttpResponses = general.ValidateHttpResponses;
            config.Services.VerifyVersion = general.VerifyVersion;

            var transformations = new List<string>();
            foreach (var node in this.DataItemsRoot.Children)
            {
                this.CreateServiceConfig(config.Services, general.SubscriptionTimeout, node, transformations);
            }

            var languages = this.controllers.OfType<Vdv301LanguagesPartController>().Single();
            config.Languages = languages.GetLanguageMappings().ToList();

            var transGeneral = this.Parent.GetPart<TransformationsGeneralPartController>();
            var category = (TransformationsCategoryController)transGeneral.Parent;
            config.Transformations =
                category.TransformationPartControllers.Where(c => transformations.Contains(c.ViewModel.ChainId))
                    .Select(part => part.CreateChain())
                    .ToList();

            return config;
        }

        /// <summary>
        /// Creates all part controllers.
        /// </summary>
        /// <returns>
        /// An enumeration of the part controllers of this category.
        /// </returns>
        protected override IEnumerable<PartControllerBase> CreatePartControllers()
        {
            this.controllers.Add(new Vdv301GeneralPartController(this));
            this.controllers.Add(new Vdv301LanguagesPartController(this));
            this.controllers.Add(new DataItemSelectionPartController(this));
            this.CreateDataControllers();
            this.ModifyControllers();
            return this.controllers.Cast<PartControllerBase>();
        }

        /// <summary>
        /// Prepares the category view model (e.g. setting its name, ...).
        /// </summary>
        protected override void PrepareViewModel()
        {
            this.ViewModel.DisplayName = AdminStrings.UnitConfig_Vdv301;
        }

        private void CreateServiceConfig(
            ServicesConfig services,
            TimeSpan? subscriptionTimeout,
            CheckableTreeNodeViewModel node,
            List<string> transformations)
        {
            if (node.IsChecked.HasValue && !node.IsChecked.Value)
            {
                return;
            }

            var property = (PropertyInfo)node.Value;
            var servicePart =
                this.controllers.OfType<Vdv301ServicePartController>().Single(p => p.ServiceName == property.Name);
            var config = (ServiceConfigBase)Activator.CreateInstance(property.PropertyType);
            if (!servicePart.UseDnsSd)
            {
                config.Host = servicePart.Host;
                config.Port = servicePart.Port;
                config.Path = servicePart.Path;
            }

            property.SetValue(services, config);

            this.CreateConfig(config, new[] { property.Name }, node, transformations);

            foreach (var operationProperty in node.Children.Select(n => n.Value).Cast<PropertyInfo>())
            {
                var operation = operationProperty.GetValue(config) as OperationConfigBase;
                if (operation != null)
                {
                    operation.Subscribe = true;
                    operation.SubscriptionTimeout = subscriptionTimeout ?? TimeSpan.Zero;
                }
            }
        }

        private void CreateConfig(
            object parentConfig, string[] parentPath, CheckableTreeNodeViewModel parent, List<string> transformations)
        {
            foreach (var node in parent.Children.Where(n => !n.IsChecked.HasValue || n.IsChecked.Value))
            {
                var property = (PropertyInfo)node.Value;
                var path = parentPath.Concat(new[] { property.Name }).ToArray();
                var key = string.Format(UnitConfigKeys.Vdv301Protocol.DataItemFormat, string.Join(".", path));
                var itemPart = this.controllers.OfType<DataItemPartControllerBase>().FirstOrDefault(p => p.Key == key);
                if (itemPart != null)
                {
                    var propertyValue = (IList)property.GetValue(parentConfig);
                    var itemConfig = itemPart.CreateConfig();
                    propertyValue.Add(itemConfig);

                    if (!string.IsNullOrEmpty(itemConfig.TransfRef))
                    {
                        transformations.Add(itemConfig.TransfRef);
                    }

                    continue;
                }

                var config = Activator.CreateInstance(property.PropertyType);
                property.SetValue(parentConfig, config);
                this.CreateConfig(config, path, node, transformations);
            }
        }

        private void CreateDataControllers()
        {
            this.DataItemsRoot.Children.Clear();
            foreach (
                var service in
                    typeof(ServicesConfig).GetProperties()
                        .Where(p => typeof(ServiceConfigBase).IsAssignableFrom(p.PropertyType)))
            {
                var node = new CheckableTreeNodeViewModel();
                node.Label = service.Name;
                node.Value = service;
                node.IsCheckboxVisible = false;
                this.DataItemsRoot.Children.Add(node);

                var part = new Vdv301ServicePartController(service.Name, this);
                node.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "IsChecked")
                        {
                            part.ShouldShow = !node.IsChecked.HasValue || node.IsChecked.Value;
                        }
                    };
                this.controllers.Add(part);

                this.CreateDataControllers(service.PropertyType, new[] { service.Name }, node);
            }
        }

        private void ModifyControllers()
        {
            // do some manual changes that can't be deduced from the config structure
            // 1. remove the "Row" editor for multi-row parts
            foreach (var part in
                this.controllers.OfType<DataItemPartControllerBase>()
                    .Where(p => p.Key.Contains(".StopSequence.") && !p.Key.Contains(".DisplayContent.")))
            {
                part.ShouldShowRow = false;
            }
        }

        private void CreateDataControllers(Type type, string[] parentPath, CheckableTreeNodeViewModel parent)
        {
            foreach (
                var property in
                    type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            {
                var path = parentPath.Concat(new[] { property.Name }).ToArray();
                var node = new CheckableTreeNodeViewModel { Label = property.Name, Value = property };
                parent.Children.Add(node);
                if (property.PropertyType.IsGenericType
                    && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    DataItemPartControllerBase controller;
                    if (property.PropertyType.GenericTypeArguments[0] == typeof(DateTimeDataItemConfig))
                    {
                        controller = new DateTimeDataItemPartController(path, this);
                    }
                    else if (property.PropertyType.GenericTypeArguments[0] == typeof(InternationalTextDataItemConfig))
                    {
                        controller = new InternationalTextDataItemPartController(path, this);
                    }
                    else
                    {
                        controller = new DataItemPartController(path, this);
                    }

                    node.IsCheckboxVisible = true;
                    node.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == "IsChecked")
                            {
                                controller.ShouldShow = node.IsChecked.HasValue && node.IsChecked.Value;
                            }
                        };
                    this.controllers.Add(controller);
                }
                else
                {
                    node.IsCheckboxVisible = false;
                    this.CreateDataControllers(property.PropertyType, path, node);
                }
            }
        }

        private void UpdateVisibility()
        {
            if (this.incoming == null)
            {
                return;
            }

            var visible = this.incoming.HasSelected(IncomingData.Vdv301);
            foreach (var part in this.controllers)
            {
                part.UpdateVisibility(visible);
            }
        }
    }
}
