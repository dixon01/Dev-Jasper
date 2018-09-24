// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalScreenController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using NLog;

    /// <summary>
    /// Implementation for the <see cref="IPhysicalScreenController"/>.
    /// </summary>
    public class PhysicalScreenController : IPhysicalScreenController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalScreenController"/> class.
        /// </summary>
        /// <param name="mediaShellController">
        /// The Media Shell Controller.
        /// </param>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public PhysicalScreenController(
            IMediaShellController mediaShellController, IMediaShell mediaShell, ICommandRegistry commandRegistry)
        {
            this.MediaShell = mediaShell;
            this.Parent = mediaShellController;
            this.CommandRegistry = commandRegistry;
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.PhysicalScreen.Choose,
                new RelayCommand<PhysicalScreenConfigDataViewModel>(this.ChoosePhysicalScreen));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.PhysicalScreen.Clone,
                new RelayCommand<PhysicalScreenConfigDataViewModel>(
                    this.ClonePhysicalScreen,
                    this.UserHasWritePermission));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.PhysicalScreen.CreateNew,
                new RelayCommand<CreatePhysicalScreenParameters>(
                    this.CreatePhysicalScreen,
                    this.UserHasWritePermission));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.PhysicalScreen.Delete,
                new RelayCommand(this.DeletePhysicalScreen, this.UserHasWritePermission));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.PhysicalScreen.Rename,
                new RelayCommand<RenameReusableEntityParameters>(
                    this.RenamePhysicalScreen,
                    this.UserHasWritePermission));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.PhysicalScreen.ShowCreatePhysicalScreenPopup,
                new RelayCommand(this.ShowCreatePhysicalScreenPopup, this.UserHasWritePermission));
            this.CommandRegistry.RegisterCommand(
              CommandCompositionKeys.Shell.UI.ShowResolutionFormulaEditor,
              new RelayCommand<ContextMenu>(this.ShowResolutionFormulaEditor));
            this.CommandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.VirtualDisplay.Choose,
                new RelayCommand<VirtualDisplayRefConfigDataViewModel>(this.ChooseVirtualDisplay));
        }

        /// <summary>
        /// Gets or sets the media shell.
        /// </summary>
        /// <value>
        /// The media shell.
        /// </value>
        public IMediaShell MediaShell { get; set; }

        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        public IMediaShellController Parent { get; set; }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        protected ICommandRegistry CommandRegistry { get; private set; }

        /// <summary>
        /// Generates a unique layout name in the format:
        /// "Layout {index} ({width}x{height}".
        /// </summary>
        /// <param name="resolutionWidth">
        /// The resolution width.
        /// </param>
        /// <param name="resolutionHeight">
        /// The resolution height.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="screenType">
        /// The physical screen type.
        /// </param>
        /// <param name="isEventLayout">
        /// Indicates a value whether the layout is linked to an event cycle (only to be used with screen type Audio).
        /// </param>
        /// <returns>
        /// The unique name.
        /// </returns>
        public string GenerateLayoutName(
            int resolutionWidth,
            int resolutionHeight,
            int index,
            PhysicalScreenType screenType,
            bool isEventLayout = false)
        {
            string name;
            if (screenType != PhysicalScreenType.Audio)
            {
                name = string.Format(
                    "{0} {1} ({2}x{3})",
                    MediaStrings.PhysicalScreenController_DefaultLayoutName,
                    index,
                    resolutionWidth,
                    resolutionHeight);
                var isUnique = this.IsLayoutNameUnique(name);
                while (!isUnique)
                {
                    index++;
                    name = string.Format(
                        "{0} {1} ({2}x{3})",
                        MediaStrings.PhysicalScreenController_DefaultLayoutName,
                        index,
                        resolutionWidth,
                        resolutionHeight);
                    isUnique = this.IsLayoutNameUnique(name);
                }
            }
            else
            {
                var namePrefix = isEventLayout
                                     ? MediaStrings.PhysicalScreenController_DefaultEventAudioLayoutName
                                     : MediaStrings.PhysicalScreenController_DefaultStandardAudioLayoutName;
                name = string.Format("{0} {1}", namePrefix, index);
                var isUnique = this.IsLayoutNameUnique(name);
                while (!isUnique)
                {
                    index++;
                    name = string.Format("{0} {1}", namePrefix, index);
                    isUnique = this.IsLayoutNameUnique(name);
                }
            }

            return name;
        }

        /// <summary>
        /// Generates a unique cycle package name in the format:
        /// "CyclePackage {PhysicalScreenType} {VirtualDisplayResolution} ({DuplicateIndex})".
        /// </summary>
        /// <param name="name">
        /// The name without the DuplicateIndex.
        /// </param>
        /// <returns>
        /// The unique name.
        /// </returns>
        public string GenerateCyclePackageName(string name)
        {
            var index = 1;
            var isUnique = this.IsCyclePackageNameUnique(name);
            var nameWithoutIndex = name;
            while (!isUnique)
            {
                index++;
                name = nameWithoutIndex + string.Format(Settings.Default.DuplicatedMediaPostfix, index);
                isUnique = this.IsCyclePackageNameUnique(name);
            }

            return name;
        }

        /// <summary>
        /// Generates a unique virtual display name in the format:
        /// "VirtualDisplay {identifier} ({DuplicateIndex})".
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The unique name.
        /// </returns>
        public string GenerateVirtualDisplayName(string identifier)
        {
            var index = 1;
            var namePrefix = MediaStrings.ProjectController_DefaultVirtualDisplayName + identifier;
            var isUnique = this.IsVirtualDisplayNameUnique(namePrefix);
            var name = namePrefix;
            while (!isUnique)
            {
                index++;
                name = namePrefix + string.Format(Settings.Default.DuplicatedMediaPostfix, index);
                isUnique = this.IsVirtualDisplayNameUnique(name);
            }

            return name;
        }

        /// <summary>
        /// Generates a unique identifier for physical screens.
        /// </summary>
        /// <returns>
        /// The unique identifier.
        /// </returns>
        public string GenerateIdentifier()
        {
            var identifier = 0;
            var isUnique = this.IsIdentifierUnique(identifier.ToString(CultureInfo.InvariantCulture));
            while (!isUnique)
            {
                identifier++;
                isUnique = this.IsIdentifierUnique(identifier.ToString(CultureInfo.InvariantCulture));
            }

            return identifier.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Generates a unique physical screen name in the format:
        /// {resolution} ({DuplicateIndex})
        /// </summary>
        /// <param name="resolution">
        /// The resolution of the screen.
        /// </param>
        /// <returns>
        /// The unique name.
        /// </returns>
        public string GeneratePhysicalScreenName(string resolution)
        {
            var index = 1;
            var newName = resolution;
            var isUnique = this.IsNameUnique(newName);

            while (!isUnique)
            {
                index++;
                newName = resolution + string.Format(Settings.Default.DuplicatedMediaPostfix, index);
                isUnique = this.IsNameUnique(newName);
            }

            return newName;
        }

        private static int GetCurrentWidth(int currentIndex, MasterLayout masterLayout, int widthWithoutGaps)
        {
            if (masterLayout.ColumnWidths[currentIndex] != "*")
            {
                return int.Parse(masterLayout.ColumnWidths[currentIndex]);
            }

            if (currentIndex == 0)
            {
                return widthWithoutGaps
                    - ((int)Math.Floor(masterLayout.GapWidths.ElementAtOrDefault(currentIndex) / 2.0f));
            }

            if (currentIndex == masterLayout.ColumnWidths.Count - 1)
            {
                return widthWithoutGaps
                    - ((int)Math.Ceiling(masterLayout.GapWidths.ElementAtOrDefault(currentIndex - 1) / 2.0f));
            }

            return widthWithoutGaps
                   - ((int)Math.Ceiling(masterLayout.GapWidths.ElementAtOrDefault(currentIndex - 1) / 2.0f))
                   - ((int)Math.Floor(masterLayout.GapWidths.ElementAtOrDefault(currentIndex) / 2.0f));
        }

        private static int GetCurrentHeight(int currentIndex, MasterLayout masterLayout, int heightWithoutGaps)
        {
            if (masterLayout.RowHeights[currentIndex] != "*")
            {
                return int.Parse(masterLayout.RowHeights[currentIndex]);
            }

            if (currentIndex == 0)
            {
                return heightWithoutGaps
                    - ((int)Math.Floor(masterLayout.GapHeights.ElementAtOrDefault(currentIndex) / 2.0f));
            }

            if (currentIndex == masterLayout.RowHeights.Count - 1)
            {
                return heightWithoutGaps
                    - ((int)Math.Ceiling(masterLayout.GapHeights.ElementAtOrDefault(currentIndex - 1) / 2.0f));
            }

            return heightWithoutGaps
                - ((int)Math.Ceiling(masterLayout.GapHeights.ElementAtOrDefault(currentIndex - 1) / 2.0f))
                - ((int)Math.Floor(masterLayout.GapHeights.ElementAtOrDefault(currentIndex) / 2.0f));
        }

        private static IDynamicDataValue GetFormulaDataValueFromContextMenu(ContextMenu contextMenu)
        {
            IDynamicDataValue dataValue = null;

            if (contextMenu != null)
            {
                var item = contextMenu.PlacementTarget as PropertyGridItem;
                if (item != null)
                {
                    var dataSource = item.Tag as IDataValue;

                    var value = dataSource as IDynamicDataValue;
                    if (value != null)
                    {
                        dataValue = value;
                    }
                }
            }

            return dataValue;
        }

        private bool UserHasWritePermission(object obj)
        {
            return this.UserHasWritePermission();
        }

        private bool UserHasWritePermission()
        {
            return this.MediaShell != null
                   && this.MediaShell.MediaApplicationState != null
                   && this.MediaShell.PermissionController.HasPermission(
                       Permission.Write,
                       DataScope.MediaConfiguration);
        }

        private void ChooseVirtualDisplay(VirtualDisplayRefConfigDataViewModel virtualDisplay)
        {
            if (virtualDisplay == null)
            {
                this.MediaShell.MediaApplicationState.CurrentVirtualDisplay = null;
                return;
            }

            this.MediaShell.MediaApplicationState.CurrentVirtualDisplay = virtualDisplay.Reference;
            var cyclePackage = virtualDisplay.Reference.CyclePackage;
            if (cyclePackage != null)
            {
                var cycle = cyclePackage.StandardCycles.FirstOrDefault();
                if (cycle != null)
                {
                    var section = cycle.Reference.Sections.FirstOrDefault();
                    var cmd = this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo);
                    if (cmd != null)
                    {
                        if (section != null)
                        {
                            var layout = (LayoutConfigDataViewModel)section.Layout;
                            if (layout != null)
                            {
                                layout.AddCurrentResolution();
                            }
                        }

                        cmd.Execute(section != null ? section.Layout : null);
                    }
                }
                else
                {
                    this.MediaShell.CycleNavigator.CurrentCycle = null;

                    this.MediaShell.CycleNavigator.CurrentSection = null;
                }
            }
            else
            {
                this.MediaShell.CycleNavigator.CurrentCyclePackage = null;
                this.MediaShell.CycleNavigator.CurrentCycle = null;
                this.MediaShell.CycleNavigator.CurrentSection = null;
            }
        }

        private void ShowResolutionFormulaEditor(ContextMenu data)
        {
            var dataValue = GetFormulaDataValueFromContextMenu(data);

            Logger.Debug("Request to show the Formula Editor for resolutions.");

            if (dataValue == null)
            {
                Logger.Debug("No datavalue found to show in Formula Editor.");
                return;
            }

            InteractionManager<FormulaResolutionEditorPrompt>.Current.Raise(
                new FormulaResolutionEditorPrompt(this.MediaShell, dataValue, this.CommandRegistry) { IsOpen = true },
                prompt =>
                InteractionManager<UpdateResolutionDetailsPrompt>.Current.Raise(new UpdateResolutionDetailsPrompt()));
        }

        private void DeletePhysicalScreen(object obj)
        {
            if (!(obj is IEnumerable) && obj is PhysicalScreenConfigDataViewModel)
            {
                var screenref =
                    this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation
                        .MasterLayouts.First()
                        .PhysicalScreens.FirstOrDefault(
                            p => p.ReferenceName == ((PhysicalScreenConfigDataViewModel)obj).Name.Value);
                if (screenref != null)
                {
                    obj = new List<PhysicalScreenRefConfigDataViewModel> { screenref };
                }
            }

            if (this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.PhysicalScreens.Count == 1)
            {
                MessageBox.Show(MediaStrings.ResolutionNavigationDialog_CantDeleteLastPhysicalScreen);
                return;
            }

            var screens = obj as List<PhysicalScreenRefConfigDataViewModel>;

            if (screens != null)
            {
                var historyEntry = new DeletePhysicalScreenHistoryEntry(
                    screens,
                    this.MediaShell.MediaApplicationState,
                    this.CommandRegistry,
                    MediaStrings.ResolutionNavigationDialog_DeleteScreen);
                this.Parent.ChangeHistoryController.AddHistoryEntry(historyEntry);
            }
        }

        private void ShowCreatePhysicalScreenPopup()
        {
            Logger.Debug("Request to show the physical screen creation popup.");

            var prompt = new CreatePhysicalScreenPrompt(this.CommandRegistry);

            InteractionManager<CreatePhysicalScreenPrompt>.Current.Raise(prompt);
        }

        private void CreatePhysicalScreen(CreatePhysicalScreenParameters parameters)
        {
            var resolution = parameters.Resolution ?? new ResolutionConfiguration();
            var name = this.GeneratePhysicalScreenName(
                parameters.Type == PhysicalScreenType.Audio ? "Audio" : resolution.Text);
            var layout = parameters.MasterLayout ?? this.MediaShell.MediaApplicationState.DefaultMasterLayout;
            var physicalScreen = new PhysicalScreenConfigDataViewModel(this.MediaShell)
            {
                Width = new DataValue<int>(resolution.Width),
                Height = new DataValue<int>(resolution.Height),
                DisplayText = name,
                Name = new DataValue<string>(name),
                Type = new DataValue<PhysicalScreenType>(parameters.Type),
                Visible = new AnimatedDynamicDataValue<bool>(true),
                SelectedMasterLayout = layout,
                IsMonochromeScreen = parameters.IsMonochrome,
                IsInEditMode = true
            };
            List<VirtualDisplayRefConfigDataViewModel> virtualRefDisplays;
            var virtualDisplays = this.CreateVirtualDisplays(physicalScreen, out virtualRefDisplays).ToList();
            var cyclePackages = new List<CyclePackageConfigDataViewModel>();
            var virtualDisplayIndex = 0;
            foreach (var virtualDisplay in virtualDisplays)
            {
                var cyclePackage = this.CreateCyclePackageConfigDataViewModel(
                    parameters.Type, virtualDisplayIndex, virtualDisplay);
                cyclePackages.Add(cyclePackage);
                virtualDisplay.CyclePackage = cyclePackage;
                virtualDisplayIndex++;
            }

            var historyEntry = new CreatePhysicalScreenHistoryEntry(
                this,
                this.CommandRegistry,
                physicalScreen,
                cyclePackages,
                virtualDisplays,
                virtualRefDisplays,
                MediaStrings.PhysicalScreenController_CreatePhysicalScreenHistoryEntry);
            this.Parent.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private IEnumerable<VirtualDisplayConfigDataViewModel> CreateVirtualDisplays(
            PhysicalScreenConfigDataViewModel physicalScreen,
            out List<VirtualDisplayRefConfigDataViewModel> refConfigDataViewModels)
        {
            var virtualDisplays = new List<VirtualDisplayConfigDataViewModel>();
            refConfigDataViewModels = new List<VirtualDisplayRefConfigDataViewModel>();

            var screenHeight = physicalScreen.Height.Value;
            var screenWidth = physicalScreen.Width.Value;
            var masterLayout = physicalScreen.SelectedMasterLayout;

            var rowCount = masterLayout.RowHeights.Count;
            var columnCount = masterLayout.ColumnWidths.Count;

            var widthWithoutGapsFloor = screenWidth / columnCount;
            var surplusWidth = screenWidth % columnCount;

            var heightWithoutGapsFloor = screenHeight / rowCount;
            var surplusHeight = screenHeight % rowCount;

            var virtualScreenIndex = 0;
            var offsetY = 0;
            for (var row = 0; row < rowCount; row++)
            {
                var heightWithoutGaps = heightWithoutGapsFloor + ((row < surplusHeight) ? 1 : 0);
                var virtualHeight = GetCurrentHeight(row, masterLayout, heightWithoutGaps);

                var offsetX = 0;
                for (var column = 0; column < columnCount; column++)
                {
                    var widthWithoutGaps = widthWithoutGapsFloor + ((column < surplusWidth) ? 1 : 0);
                    var virtualWidth = GetCurrentWidth(column, masterLayout, widthWithoutGaps);

                    var name = string.Format("{0}_{1}", physicalScreen.Identifier.Value, virtualScreenIndex + 1);
                    var display = this.CreateVirtualDisplay(virtualWidth, virtualHeight, name);
                    virtualDisplays.Add(display);
                    refConfigDataViewModels.Add(this.CreateVirtualDisplayRef(offsetX, offsetY, display));

                    offsetX = offsetX + virtualWidth + masterLayout.GapWidths.ElementAtOrDefault(column);
                    virtualScreenIndex++;
                }

                offsetY = offsetY + virtualHeight + masterLayout.GapHeights.ElementAtOrDefault(row);
            }

            return virtualDisplays;
        }

        private VirtualDisplayRefConfigDataViewModel CreateVirtualDisplayRef(
            int x, int y, VirtualDisplayConfigDataViewModel reference)
        {
            return new VirtualDisplayRefConfigDataViewModel(this.MediaShell)
                                {
                                    Reference = reference,
                                    X = { Value = x },
                                    Y = { Value = y }
                                };
        }

        private VirtualDisplayConfigDataViewModel CreateVirtualDisplay(int width, int height, string name)
        {
            var virtualName = this.GenerateVirtualDisplayName(name);
            var virtualDisplay = new VirtualDisplayConfigDataViewModel(this.MediaShell)
            {
                Height = new DataValue<int>(height),
                Name = new DataValue<string>(virtualName),
                Width = new DataValue<int>(width),
                DisplayText = virtualName,
            };

            return virtualDisplay;
        }

        private StandardCycleConfigDataViewModel CreateDefaultStandardCycleConfigDataViewModel()
        {
            return new StandardCycleConfigDataViewModel(this.MediaShell)
            {
                Enabled = new DynamicDataValue<bool>(true),
                Name = new DataValue<string>(MediaStrings.CycleController_NewCycleName),
                Sections = new ExtendedObservableCollection<SectionConfigDataViewModelBase>
                        {
                            new StandardSectionConfigDataViewModel(this.MediaShell)
                                {
                                    Duration = new DataValue<TimeSpan>(TimeSpan.FromSeconds(10))
                                }
                        }
            };
        }

        private StandardCycleConfigDataViewModel CreateAudioStandardCycleConfigDataViewModel()
        {
            return new StandardCycleConfigDataViewModel(this.MediaShell)
            {
                Enabled = new DynamicDataValue<bool>(true),
                Name = new DataValue<string>(MediaStrings.CycleController_NewCycleName),
                Sections = new ExtendedObservableCollection<SectionConfigDataViewModelBase>
                        {
                            new StandardSectionConfigDataViewModel(this.MediaShell)
                                {
                                    Duration = new DataValue<TimeSpan>(TimeSpan.FromHours(10))
                                }
                        }
            };
        }

        private EventCycleConfigDataViewModel CreateAudioEventCycleConfigDataViewModel()
        {
            return new EventCycleConfigDataViewModel(this.MediaShell)
            {
                Enabled = new DynamicDataValue<bool>(true),
                Name = new DataValue<string>(MediaStrings.CycleController_NewEventCycleName),
                Sections = new ExtendedObservableCollection<SectionConfigDataViewModelBase>
                        {
                            new StandardSectionConfigDataViewModel(this.MediaShell)
                                {
                                    Duration = new DataValue<TimeSpan>(TimeSpan.FromSeconds(1))
                                }
                        }
            };
        }

        private CyclePackageConfigDataViewModel CreateCyclePackageConfigDataViewModel(
            PhysicalScreenType physicalScreenType,
            int virtualDisplayIndex,
            VirtualDisplayConfigDataViewModel virtualDisplay,
            bool addReferenceToCycle = true)
        {
            var namePrefix = string.Format(
                "{0} {1} {2}x{3}",
                MediaStrings.PhysicalScreenController_DefaultCyclePackageName,
                physicalScreenType,
                virtualDisplay.Width.Value,
                virtualDisplay.Height.Value);
            var name = this.GenerateCyclePackageName(namePrefix);
            if (physicalScreenType == PhysicalScreenType.Audio)
            {
                var standardAudioCycle = this.CreateAudioStandardCycleConfigDataViewModel();
                standardAudioCycle.Sections[0].Layout = this.CreateLayoutConfigDataViewModel(
                    0,
                    0,
                    virtualDisplayIndex,
                    physicalScreenType);
                var eventAudioCycle = this.CreateAudioEventCycleConfigDataViewModel();
                eventAudioCycle.Sections[0].Layout = this.CreateLayoutConfigDataViewModel(
                    0,
                    0,
                    virtualDisplayIndex,
                    physicalScreenType,
                    true);
                var standardAudioCycleRef = new StandardCycleRefConfigDataViewModel(this.MediaShell)
                {
                    Reference = standardAudioCycle
                };
                var standardAudioCycleRefList = new ExtendedObservableCollection<StandardCycleRefConfigDataViewModel>
                                           {
                                               standardAudioCycleRef
                                           };
                var eventAudioCycleRef = new EventCycleRefConfigDataViewModel(this.MediaShell)
                {
                    Reference = eventAudioCycle
                };
                var eventAudioCycleRefList = new ExtendedObservableCollection<EventCycleRefConfigDataViewModel>
                                           {
                                               eventAudioCycleRef
                                           };

                var audioCyclePackage = new CyclePackageConfigDataViewModel(this.MediaShell)
                {
                    Name = new DataValue<string>(name),
                    StandardCycles = standardAudioCycleRefList,
                    EventCycles = eventAudioCycleRefList
                };
                if (addReferenceToCycle)
                {
                    standardAudioCycle.CyclePackageReferences.Add(audioCyclePackage);
                    eventAudioCycle.CyclePackageReferences.Add(audioCyclePackage);
                }

                return audioCyclePackage;
            }

            var standardCycle = this.CreateDefaultStandardCycleConfigDataViewModel();
            standardCycle.Sections[0].Layout = this.CreateLayoutConfigDataViewModel(
                virtualDisplay.Width.Value, virtualDisplay.Height.Value, virtualDisplayIndex, physicalScreenType);
            var standardCycleRef = new StandardCycleRefConfigDataViewModel(this.MediaShell)
                                       {
                                           Reference = standardCycle
                                       };
            var standardCycleRefList = new ExtendedObservableCollection<StandardCycleRefConfigDataViewModel>
                                           {
                                               standardCycleRef
                                           };
            var cyclePackage = new CyclePackageConfigDataViewModel(this.MediaShell)
                                   {
                                       Name = new DataValue<string>(name),
                                       StandardCycles =
                                           standardCycleRefList
                                   };
            if (addReferenceToCycle)
            {
                standardCycle.CyclePackageReferences.Add(cyclePackage);
            }

            return cyclePackage;
        }

        private LayoutConfigDataViewModel CreateLayoutConfigDataViewModel(
            int resolutionWidth,
            int resolutionHeight,
            int index,
            PhysicalScreenType screenType,
            bool isAudioEventLayout = false)
        {
            var resolution = new ResolutionConfigDataViewModel(this.MediaShell)
                                 {
                                     Width = { Value = resolutionWidth },
                                     Height = { Value = resolutionHeight }
                                 };
            var name =
                this.GenerateLayoutName(resolutionWidth, resolutionHeight, index, screenType, isAudioEventLayout);
            var layout = new LayoutConfigDataViewModel(this.MediaShell)
                             {
                                 Name = { Value = name },
                                 DisplayText = name,
                                 Resolutions =
                                     new ExtendedObservableCollection<ResolutionConfigDataViewModel> { resolution }
                             };
            return layout;
        }

        private bool IsLayoutNameUnique(string name)
        {
            return
               this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Layouts.All(
                   p => p.Name.Value != name);
        }

        private bool IsVirtualDisplayNameUnique(string name)
        {
            return
                this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.VirtualDisplays.All(
                    p => p.Name.Value != name);
        }

        private bool IsCyclePackageNameUnique(string name)
        {
            return
                this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.CyclePackages.All(
                    p => p.Name.Value != name);
        }

        private bool IsIdentifierUnique(string identifier)
        {
            return
                this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.PhysicalScreens.All(
                    p => p.Identifier.Value != identifier);
        }

        private bool IsNameUnique(string newName)
        {
            return
                this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.PhysicalScreens.All(
                    p => p.Name.Value != newName);
        }

        private void ClonePhysicalScreen(PhysicalScreenConfigDataViewModel screen)
        {
            var masterLayout =
                this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation
                    .MasterLayouts.FirstOrDefault();
            if (masterLayout == null)
            {
                Logger.Debug("No masterLayout defined.");
                return;
            }

            var screenRef = masterLayout.PhysicalScreens.FirstOrDefault(s => s.ReferenceName == screen.Name.Value);
            if (screenRef == null)
            {
                Logger.Debug("No physical screen ref found for screen {0}.", screen.Name.Value);
                return;
            }

            var historyEntry = new ClonePhysicalScreenHistoryEntry(
                screenRef,
                this.MediaShell.MediaApplicationState.CurrentProject,
                this.MediaShell,
                this.CommandRegistry,
                MediaStrings.ResolutionNavigationDialog_CloneScreen);
            this.Parent.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private void ChoosePhysicalScreen(PhysicalScreenConfigDataViewModel screen)
        {
            if (screen == null)
            {
                this.MediaShell.MediaApplicationState.CurrentPhysicalScreen = null;
                this.MediaShell.MediaApplicationState.CurrentVirtualDisplay = null;
                return;
            }

            if (screen.Type.Value != PhysicalScreenType.TFT)
            {
                this.MediaShell.SimulationIsVisible = false;
            }

            this.MediaShell.MediaApplicationState.CurrentPhysicalScreen = screen;
            ((MediaShell)this.MediaShell).SetCurrentEditor(screen.Type.Value);
            var masterLayout =
                this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.MasterPresentation
                    .MasterLayouts.FirstOrDefault();
            if (masterLayout == null)
            {
                Logger.Debug("No masterLayout defined.");
                return;
            }

            var screenRef = masterLayout.PhysicalScreens.FirstOrDefault(s => s.ReferenceName == screen.Name.Value);
            if (screenRef == null)
            {
                Logger.Debug("No physical screen ref found for screen {0}.", screen.Name.Value);
                return;
            }

            var virtualDisplay = screenRef.VirtualDisplays.FirstOrDefault();
            if (virtualDisplay == null)
            {
                Logger.Debug("No virtual displays defined for physical screen {0}.", screen.Name.Value);
                return;
            }

            this.MediaShell.MediaApplicationState.CurrentVirtualDisplay = virtualDisplay.Reference;
            var cyclePackage = virtualDisplay.Reference.CyclePackage;
            if (cyclePackage != null)
            {
                this.MediaShell.CycleNavigator.CurrentCyclePackage = cyclePackage;

                // 1st audio std cycle is not editable, so do not select it by default
                CycleRefConfigDataViewModelBase cycle;
                if (screen.Type.Value == PhysicalScreenType.Audio)
                {
                    cycle = cyclePackage.EventCycles.FirstOrDefault()
                            ?? (CycleRefConfigDataViewModelBase)cyclePackage.StandardCycles.FirstOrDefault();
                }
                else
                {
                    cycle = cyclePackage.StandardCycles.FirstOrDefault();
                }

                if (cycle != null)
                {
                    this.MediaShell.CycleNavigator.CurrentCycle = cycle.Reference;
                    var section = cycle.Reference.Sections.FirstOrDefault();
                    this.MediaShell.CycleNavigator.CurrentSection = section;
                    var cmd = this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo);
                    if (cmd != null)
                    {
                        cmd.Execute(section != null ? section.Layout : null);
                    }
                }
                else
                {
                    this.MediaShell.CycleNavigator.CurrentCycle = null;
                    this.MediaShell.CycleNavigator.CurrentSection = null;
                }
            }
            else
            {
                this.MediaShell.CycleNavigator.CurrentCyclePackage = null;
                this.MediaShell.CycleNavigator.CurrentCycle = null;
                this.MediaShell.CycleNavigator.CurrentSection = null;
            }
        }

        private void RenamePhysicalScreen(RenameReusableEntityParameters parameters)
        {
            var physicalScreen = parameters.Entity as PhysicalScreenConfigDataViewModel;
            if (physicalScreen != null && parameters.NewName != physicalScreen.Name.Value)
            {
                var historyEntry = new RenamePhysicalScreenHistoryEntry(
                    physicalScreen,
                    parameters.NewName,
                    MediaStrings.CycleController_RenameCycleHistoryEntry);
                this.Parent.ChangeHistoryController.AddHistoryEntry(historyEntry);
            }
        }
    }
}
