// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandCompositionKeys.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CommandCompositionKeys type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core
{
    /// <summary>
    /// Defines the command keys used for composition.
    /// </summary>
    internal static class CommandCompositionKeys
    {
        /// <summary>
        /// Project commands.
        /// </summary>
        public static class Project
        {
            /// <summary>
            /// The New command.
            /// </summary>
            public const string New = "Media.Project.New";

            /// <summary>
            /// The Open command.
            /// </summary>
            public const string Open = "Media.Project.Open";

            /// <summary>
            /// The Save command.
            /// </summary>
            public const string Save = "Media.Project.Save";

            /// <summary>
            /// The SaveAs command.
            /// </summary>
            public const string SaveAs = "Media.Project.SaveAs";

            /// <summary>
            /// The check in command.
            /// </summary>
            public const string CheckIn = "Media.Project.CheckIn";

            /// <summary>
            /// The rename.
            /// </summary>
            public const string RenameRecentlyUsed = "Media.Project.RenameRecentlyUsed";

            /// <summary>
            /// The delete project command.
            /// </summary>
            public const string DeleteProject = "Media.Project.DeleteProject";

            /// <summary>
            /// The AddResource command.
            /// </summary>
            public const string AddResource = "Media.Project.AddResource";

            /// <summary>
            /// The AddResourceReference command.
            /// </summary>
            public const string AddResourceReference = "Media.Project.AddResourceReference";

            /// <summary>
            /// The DeleteResource command.
            /// </summary>
            public const string DeleteResource = "Media.Project.DeleteResource";

            /// <summary>
            /// The DeleteResourceReference command.
            /// </summary>
            public const string DeleteResourceReference = "Media.Project.DeleteResourceReference";

            /// <summary>
            /// The SelectResource command.
            /// </summary>
            public const string SelectResource = "Media.Project.SelectResource";

            /// <summary>
            /// The AddMediaPool command.
            /// </summary>
            public const string CreateMediaPool = "Media.Project.CreateMediaPool";

            /// <summary>
            /// The DeleteMediaPool command.
            /// </summary>
            public const string DeleteMediaPool = "Media.Project.DeleteMediaPool";

            /// <summary>
            /// The UpdateMediaPool command.
            /// </summary>
            public const string UpdateResourceListElement = "Media.Project.UpdateResourceListElement";

            /// <summary>
            /// The CreateReplacement command.
            /// </summary>
            public const string CreateReplacement = "Media.Project.CreateReplacement";

            /// <summary>
            /// The DeleteReplacement command.
            /// </summary>
            public const string DeleteReplacement = "Media.Project.DeleteReplacement";

            /// <summary>
            /// The UpdateReplacement command.
            /// </summary>
            public const string UpdateReplacement = "Media.Project.UpdateReplacement";

            /// <summary>
            /// The CreatePredefinedFormula command.
            /// </summary>
            public const string CreatePredefinedFormula = "Media.Project.CreatePredefinedFormula";

            /// <summary>
            /// The DeletePredefinedFormula command.
            /// </summary>
            public const string DeletePredefinedFormula = "Media.Project.DeletePredefinedFormula";

            /// <summary>
            /// The UpdatePredefinedFormula command.
            /// </summary>
            public const string UpdatePredefinedFormula = "Media.Project.UpdatePredefinedFormula";

            /// <summary>
            /// The ClonePredefinedFormula command.
            /// </summary>
            public const string ClonePredefinedFormula = "Media.Project.ClonePredefinedFormula";

            /// <summary>
            /// The RenamePredefinedFormula command.
            /// </summary>
            public const string RenamePredefinedFormula = "Media.Project.RenamePredefinedFormula";

            /// <summary>
            /// The publish document.
            /// </summary>
            public const string PublishDocument = "Media.Project.PublishDocument";

            /// <summary>
            /// The import command.
            /// </summary>
            public const string Import = "Media.Project.Import";

            /// <summary>
            /// The project transfer command.
            /// </summary>
            public const string Transfer = "Media.Project.Transfer";

            /// <summary>
            /// The run compatibility check.
            /// </summary>
            public const string RunCompatibilityCheck = "Media.Project.RunCompatibilityCheck";

            /// <summary>
            /// Navigation commands.
            /// </summary>
            public static class CsvMapping
            {
                /// <summary>
                /// The create csv mapping command.
                /// </summary>
                public const string CreateCsv = "Media.Project.CsvMapping.CreateCsv";

                /// <summary>
                /// The delete csv mapping command.
                /// </summary>
                public const string DeleteCsv = "Media.Project.CsvMapping.DeleteCsv";

                /// <summary>
                /// The import csv mapping command.
                /// </summary>
                public const string ImportCsv = "Media.Project.CsvMapping.ImportCsv";

                /// <summary>
                /// The edit csv mapping command.
                /// </summary>
                public const string EditCsv = "Media.Project.CsvMapping.EditCsv";
            }
        }

        /// <summary>
        /// Commands within the Shell scope.
        /// </summary>
        public static class Shell
        {
            /// <summary>
            /// Navigation commands.
            /// </summary>
            public static class Navigation
            {
                /// <summary>
                /// The GoToConsistencyProblem command.
                /// </summary>
                public const string GoToConsistencyProblem = "Media.Shell.Navigation.GoToConsistencyProblem";
            }

            /// <summary>
            /// UI commands.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            public static class UI
            {
                /// <summary>
                /// The ShowMainMenu command.
                /// </summary>
                public const string ShowMainMenu = "Media.Shell.UI.ShowMainMenu";

                /// <summary>
                /// The ShowEditMenu command.
                /// </summary>
                public const string ShowEditMenu = "Media.Shell.UI.ShowEditMenu";

                /// <summary>
                /// The ShowViewMenu command.
                /// </summary>
                public const string ShowViewMenu = "Media.Shell.UI.ShowViewMenu";

                /// <summary>
                /// The ShowFormulaEditor command.
                /// </summary>
                public const string ShowFormulaEditor = "Media.Shell.UI.ShowFormulaEditor";

                /// <summary>
                /// The ShowAnimationEditor command.
                /// </summary>
                public const string ShowAnimationEditor = "Media.Shell.UI.ShowAnimationEditor";

                /// <summary>
                /// The ShowNavigationFormulaEditor command.
                /// </summary>
                public const string ShowNavigationFormulaEditor = "Media.Shell.UI.ShowNavigationFormulaEditor";

                /// <summary>
                /// The ShowResolutionFormulaEditor command.
                /// </summary>
                public const string ShowResolutionFormulaEditor = "Media.Shell.UI.ShowResolutionFormulaEditor";

                /// <summary>
                /// The ShowNavigationTriggerEditor command.
                /// </summary>
                public const string ShowNavigationTriggerEditor = "Media.Shell.UI.ShowNavigationTriggerEditor";

                /// <summary>
                /// The ShowNavigationAnimationEditor command.
                /// </summary>
                public const string ShowNavigationAnimationEditor = "Media.Shell.UI.ShowNavigationAnimationEditor";

                /// <summary>
                /// The ShowLayoutNavigation command.
                /// </summary>
                public const string ShowLayoutNavigation = "Media.Shell.UI.ShowLayoutNavigation";

                /// <summary>
                /// The ShowResolutionNavigation command.
                /// </summary>
                public const string ShowResolutionNavigation = "Media.Shell.UI.ShowResolutionNavigation";

                /// <summary>
                /// The show dictionary selector command.
                /// </summary>
                public const string ShowDictionarySelector = "Media.Shell.UI.ShowDictionarySelector";

                /// <summary>
                /// The show trigger editor dictionary selector command.
                /// </summary>
                public const string ShowTriggerEditorDictionarySelector =
                    "Media.Shell.UI.ShowTriggerEditorDictionarySelector";

                /// <summary>
                /// The ShowMediaSelector.
                /// </summary>
                public const string ShowMediaSelector = "Media.Shell.UI.ShowMediaSelector";

                /// <summary>
                /// The show consistency dialog.
                /// </summary>
                public const string ShowConsistencyDialog = "Media.Shell.UI.ShowConsistencyDialog";

                /// <summary>
                /// The show check in dialog.
                /// </summary>
                public const string ShowCheckinDialog = "Media.Shell.UI.ShowCheckinDialog";

                /// <summary>
                /// The show about screen dialog.
                /// </summary>
                public const string ShowAboutScreen = "Media.Shell.UI.ShowAboutScreen";

                /// <summary>
                /// The show options dialog.
                /// </summary>
                public const string ShowOptionsDialog = "Media.Shell.UI.ShowOptionsDialog";

                /// <summary>
                /// The menu.
                /// </summary>
                public static class Menu
                {
                    /// <summary>
                    /// The show menu entry.
                    /// </summary>
                    public const string ShowMenuEntry = "Media.Shell.UI.Menu.ShowMenuEntry";

                    /// <summary>
                    /// The start simulation.
                    /// </summary>
                    public const string ToggleSimulation = "Media.Shell.UI.Menu.ToggleSimulation";

                    /// <summary>
                    /// The toggle edge magnet.
                    /// </summary>
                    public const string ToggleEdgeSnap = "Media.Shell.UI.Menu.ToggleEdgeSnap";

                    /// <summary>
                    /// The close main menu.
                    /// </summary>
                    public const string CloseMainMenu = "Media.Shell.UI.Menu.CloseMainMenu";
                }
            }

            /// <summary>
            /// Preview commands.
            /// </summary>
            public static class Preview
            {
                /// <summary>
                /// The Play command.
                /// </summary>
                public const string Play = "Media.Shell.Preview.Play";

                /// <summary>
                /// The Pause command.
                /// </summary>
                public const string Pause = "Media.Shell.Preview.Pause";

                /// <summary>
                /// The Stop command.
                /// </summary>
                public const string Stop = "Media.Shell.Preview.Stop";
            }

            /// <summary>
            /// Layout commands.
            /// </summary>
            public static class Layout
            {
                /// <summary>
                /// The CreateElements command.
                /// </summary>
                public const string DeleteElements = "Media.Shell.Layout.DeleteElements";

                /// <summary>
                /// The DeleteSelectedElements command.
                /// </summary>
                public const string DeleteSelectedElements = "Media.Shell.Layout.DeleteSelectedElements";

                /// <summary>
                /// The DeleteSelectedElements command.
                /// </summary>
                public const string RenameLayoutElement = "Media.Shell.Layout.RenameLayoutElement";

                /// <summary>
                /// The UpdateElement command.
                /// Updates all the properties of an Layout Element including Position, size, zIndex and Grouping.
                /// </summary>
                public const string UpdateElement = "Media.Shell.Layout.UpdateElement";

                /// <summary>
                /// The SelectElements command.
                /// Needs a Position and optionally a Size.
                /// </summary>
                public const string SelectElements = "Media.Shell.Layout.SelectElements";

                /// <summary>
                /// The SelectElement command.
                /// Needs a Position
                /// </summary>
                public const string SelectElement = "Media.Shell.Layout.SelectElement";

                /// <summary>
                /// The MoveSelectedElements command.
                /// Needs MoveElementCommandParameters.
                /// </summary>
                public const string MoveSelectedElements = "Media.Shell.Layout.MoveSelectedElements";

                /// <summary>
                /// The ResizeElement command.
                /// </summary>
                public const string ResizeElement = "Media.Shell.Layout.ResizeElement";

                /// <summary>
                /// The remove formula command.
                /// </summary>
                public const string RemoveFormula = "Media.Shell.Layout.RemoveFormula";

                /// <summary>
                /// The remove animation command.
                /// </summary>
                public const string RemoveAnimation = "Media.Shell.Layout.RemoveAnimation";

                /// <summary>
                /// The navigate to layout command.
                /// </summary>
                public const string NavigateTo = "Media.Shell.Layout.NavigateTo";

                /// <summary>
                /// The create TFT layout command.
                /// </summary>
                public const string CreateTftLayout = "Media.Shell.Layout.CreateTftLayout";

                /// <summary>
                /// The create LED layout command.
                /// </summary>
                public const string CreateLedLayout = "Media.Shell.Layout.CreateLedLayout";

                /// <summary>
                /// The create audio layout command.
                /// </summary>
                public const string CreateAudioLayout = "Media.Shell.Layout.CreateAudioLayout";

                /// <summary>
                /// The delete layout command.
                /// </summary>
                public const string DeleteLayout = "Media.Shell.Layout.DeleteLayout";

                /// <summary>
                /// The clone layout command.
                /// </summary>
                public const string CloneLayout = "Media.Shell.Layout.CloneLayout";

                /// <summary>
                /// The rename layout command.
                /// </summary>
                public const string Rename = "Media.Shell.Layout.RenameLayout";

                /// <summary>
                /// Commands for the TFT editor.
                /// </summary>
                public static class Tft
                {
                    /// <summary>
                    /// The create element command.
                    /// Needs a Position and Size.
                    /// </summary>
                    public const string CreateElement = "Media.Shell.Layout.Tft.CreateElement";

                    /// <summary>
                    /// The ShowLayoutEditPopup command.
                    /// </summary>
                    public const string ShowLayoutEditPopup = "Media.Shell.Layout.Tft.ShowLayoutEditPopup";
                }

                /// <summary>
                /// Commands for the led editor.
                /// </summary>
                public static class Led
                {
                    /// <summary>
                    /// The create element command.
                    /// Needs a Position and Size.
                    /// </summary>
                    public const string CreateElement = "Media.Shell.Layout.Led.CreateElement";

                    /// <summary>
                    /// The ShowLayoutEditPopup command.
                    /// </summary>
                    public const string ShowLayoutEditPopup = "Media.Shell.Layout.Led.ShowLayoutEditPopup";
                }

                /// <summary>
                /// Commands for the audio editor.
                /// </summary>
                public static class Audio
                {
                    /// <summary>
                    /// The create element command.
                    /// </summary>
                    public const string CreateElement = "Media.Shell.Layout.Audio.CreateElement";

                    /// <summary>
                    /// The ShowLayoutEditPopup command.
                    /// </summary>
                    public const string ShowLayoutEditPopup = "Media.Shell.Layout.Audio.ShowLayoutEditPopup";
                }
            }

            /// <summary>
            /// Presentation commands.
            /// </summary>
            public static class Presentation
            {
                /// <summary>
                /// The command to export a presentation to a server.
                /// </summary>
                public const string ExportServer = "Media.Shell.Presentation.ExportServer";

                /// <summary>
                /// The command to export a presentation to a local destination.
                /// </summary>
                public const string ExportLocal = "Media.Shell.Presentation.ExportLocal";

                /// <summary>
                /// The SelectExportFile command.
                /// </summary>
                public const string SelectExportFile = "Media.Shell.Presentation.SelectExportFile";
            }

            /// <summary>
            /// Cycle commands.
            /// </summary>
            public static class Cycle
            {
                /// <summary>
                /// The CreateNew Cycle command.
                /// </summary>
                public const string CreateNew = "Media.Shell.Cycle.CreateNew";

                /// <summary>
                /// The CreateReference Cycle command.
                /// </summary>
                public const string CreateReference = "Media.Shell.Cycle.CreateReference";

                /// <summary>
                /// The Delete Cycle command.
                /// </summary>
                public const string Delete = "Media.Shell.Cycle.Delete";

                /// <summary>
                /// The Clone Cycle command.
                /// </summary>
                public const string Clone = "Media.Shell.Cycle.Clone";

                /// <summary>
                /// The Choose Cycle command.
                /// </summary>
                public const string Choose = "Media.Shell.Cycle.Choose";

                /// <summary>
                /// The Rename Cycle command.
                /// </summary>
                public const string Rename = "Media.Shell.Cycle.Rename";

                /// <summary>
                /// The ShowCycleTypeSelection Cycle command.
                /// </summary>
                public const string ShowCycleTypeSelection = "Media.Shell.Cycle.ShowCycleTypeSelection";

                /// <summary>
                /// The remove formula command.
                /// </summary>
                public const string RemoveFormula = "Media.Shell.Cycle.RemoveFormula";

                /// <summary>
                /// The remove animation command.
                /// </summary>
                public const string RemoveAnimation = "Media.Shell.Cycle.RemoveAnimation";
            }

            /// <summary>
            /// Section commands.
            /// </summary>
            public static class Section
            {
                /// <summary>
                /// The CreateNew Section command.
                /// </summary>
                public const string CreateNew = "Media.Shell.Section.CreateNew";

                /// <summary>
                /// The Delete Section command.
                /// </summary>
                public const string Delete = "Media.Shell.Section.Delete";

                /// <summary>
                /// The Clone Section command.
                /// </summary>
                public const string Clone = "Media.Shell.Section.Clone";

                /// <summary>
                /// The Choose Section command.
                /// </summary>
                public const string Choose = "Media.Shell.Section.Choose";

                /// <summary>
                /// The Rename Section command.
                /// </summary>
                public const string Rename = "Media.Shell.Section.Rename";

                /// <summary>
                /// The ShowSectionTypeSelection Section command.
                /// </summary>
                public const string ShowSectionTypeSelection = "Media.Shell.Section.ShowSectionTypeSelection";

                /// <summary>
                /// The remove formula command.
                /// </summary>
                public const string RemoveFormula = "Media.Shell.Section.RemoveFormula";

                /// <summary>
                /// The remove animation command.
                /// </summary>
                public const string RemoveAnimation = "Media.Shell.Section.RemoveAnimation";
            }

            /// <summary>
            /// Cycle package commands.
            /// </summary>
            public static class CyclePackage
            {
                /// <summary>
                /// The Choose CyclePackage command.
                /// </summary>
                public const string Choose = "Media.Shell.CyclePackage.Choose";

                /// <summary>
                /// The Rename CyclePackage command.
                /// </summary>
                public const string Rename = "Media.Shell.CyclePackage.Rename";
            }

            /// <summary>
            /// Commands for physical screens.
            /// </summary>
            public static class PhysicalScreen
            {
                /// <summary>
                /// The CreateNew PhysicalScreen command.
                /// </summary>
                public const string CreateNew = "Media.Shell.PhysicalScreen.CreateNew";

                /// <summary>
                /// The Delete PhysicalScreen command.
                /// </summary>
                public const string Delete = "Media.Shell.PhysicalScreen.Delete";

                /// <summary>
                /// The Choose PhysicalScreen command.
                /// </summary>
                public const string Choose = "Media.Shell.PhysicalScreen.Choose";

                /// <summary>
                /// The Clone PhysicalScreen command.
                /// </summary>
                public const string Clone = "Media.Shell.PhysicalScreen.Clone";

                /// <summary>
                /// The Rename PhysicalScreen command.
                /// </summary>
                public const string Rename = "Media.Shell.PhysicalScreen.Rename";

                /// <summary>
                /// The remove formula command.
                /// </summary>
                public const string RemoveFormula = "Media.Shell.PhysicalScreen.RemoveFormula";

                /// <summary>
                /// The show create physical screen popup command.
                /// </summary>
                public const string ShowCreatePhysicalScreenPopup =
                    "Media.Shell.PhysicalScreen.ShowCreatePhysicalScreenPopup";
            }

            /// <summary>
            /// Commands for virtual display.
            /// </summary>
            public static class VirtualDisplay
            {
                /// <summary>
                /// The Choose VirtualDisplay command.
                /// </summary>
                public const string Choose = "Media.Shell.VirtualDisplay.Choose";
            }
        }

        /// <summary>
        /// Default commands.
        /// </summary>
        public static class Default
        {
            /// <summary>
            /// The Undo command.
            /// </summary>
            public const string Undo = "Media.Default.Undo";

            /// <summary>
            /// The Redo command.
            /// </summary>
            public const string Redo = "Media.Default.Redo";

            /// <summary>
            /// The Cut command.
            /// </summary>
            public const string Cut = "Media.Default.Cut";

            /// <summary>
            /// The Copy command.
            /// </summary>
            public const string Copy = "Media.Default.Copy";

            /// <summary>
            /// The Paste command.
            /// </summary>
            public const string Paste = "Media.Default.Paste";

            /// <summary>
            /// The Delete command.
            /// </summary>
            public const string Delete = "Media.Default.Delete";

            /// <summary>
            /// The SelectAll command.
            /// </summary>
            public const string SelectAll = "Media.Default.SelectAll";
        }
    }
}
