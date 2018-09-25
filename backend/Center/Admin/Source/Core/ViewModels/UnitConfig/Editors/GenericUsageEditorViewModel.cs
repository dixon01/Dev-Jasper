// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericUsageEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericUsageEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// View model for the editor of the generic coordinates.
    /// </summary>
    public class GenericUsageEditorViewModel : EditorViewModelBase
    {
        private static Dictionary dictionary;

        private bool shouldShowRow;

        private bool shouldShowLanguage = true;

        private Column selectedColumn;

        private object selectedItem;

        private string tableColumnName;

        private bool isRowVisible;

        private bool isLanguageVisible;

        private int rowIndex;

        private Language selectedLanguage;

        private bool isNullable;

        private bool hasValue = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericUsageEditorViewModel"/> class.
        /// </summary>
        public GenericUsageEditorViewModel()
        {
            this.Label = AdminStrings.UnitConfig_Protran_GenericUsage;

            this.ShouldShowRow = true;
            this.ShouldShowLanguage = true;

            if (dictionary == null)
            {
                var manager = new ConfigManager<Dictionary>();
                manager.FileName = Path.Combine(
                    Path.GetDirectoryName(ApplicationHelper.GetEntryAssemblyLocation()) ?? Environment.CurrentDirectory,
                    "dictionary.xml");
                manager.XmlSchema = Dictionary.Schema;
                dictionary = manager.Config;
            }

            this.UpdateErrors();

            this.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "IsEnabled")
                    {
                        this.RaisePropertyChanged(() => this.IsEditorEnabled);
                    }
                };
        }

        /// <summary>
        /// Gets or sets the generic usage.
        /// This property should NOT be used in a view since it is not raising PropertyChanged events.
        /// </summary>
        public GenericUsage GenericUsage
        {
            get
            {
                if (!this.HasValue)
                {
                    return null;
                }

                var usage = new GenericUsage();
                if (this.IsLanguageVisible && this.SelectedLanguage != null)
                {
                    usage.Language = this.SelectedLanguage.Name;
                }

                var table = this.GetSelectedTable();
                if (table != null)
                {
                    usage.Table = table.Name;
                }

                if (this.SelectedColumn != null)
                {
                    usage.Column = this.SelectedColumn.Name;
                }

                if (!this.ShouldShowRow)
                {
                    usage.Row = "{0}";
                }
                else if (this.IsRowVisible)
                {
                    usage.Row = this.RowIndex.ToString(CultureInfo.InvariantCulture);
                }

                return usage;
            }

            set
            {
                if (this.IsNullable && value == null)
                {
                    this.HasValue = false;
                    return;
                }

                var usage = value ?? new GenericUsage();
                if (!string.IsNullOrEmpty(usage.Language))
                {
                    this.SelectedLanguage = this.Dictionary.GetLanguageForNameOrNumber(usage.Language);
                }
                else
                {
                    this.SelectedLanguage = null;
                }

                var table = string.IsNullOrEmpty(usage.Table)
                                ? null
                                : this.Dictionary.GetTableForNameOrNumber(usage.Table);
                Column column = null;
                if (table != null && !string.IsNullOrEmpty(usage.Column))
                {
                    column = table.GetColumnForNameOrNumber(usage.Column);
                }

                this.SelectedColumn = column;

                int row;
                if (int.TryParse(usage.Row, out row))
                {
                    this.RowIndex = row;
                }

                this.HasValue = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this editor is nullable.
        /// If it is nullable, a checkbox is shown allowing to enable/disable the entire editor.
        /// </summary>
        public bool IsNullable
        {
            get
            {
                return this.isNullable;
            }

            set
            {
                this.SetProperty(ref this.isNullable, value, () => this.IsNullable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this editor has a value.
        /// This can only be false if <see cref="IsNullable"/> is set to true.
        /// </summary>
        public bool HasValue
        {
            get
            {
                return this.hasValue;
            }

            set
            {
                if (!this.IsNullable)
                {
                    value = true;
                }

                if (this.SetProperty(ref this.hasValue, value, () => this.HasValue))
                {
                    this.MakeDirty();
                    this.UpdateErrors();
                    this.RaisePropertyChanged(() => this.IsEditorEnabled);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this editor is enabled.
        /// This takes into account the <see cref="EditorViewModelBase.IsEnabled"/> and
        /// <see cref="HasValue"/> flags.
        /// </summary>
        public bool IsEditorEnabled
        {
            get
            {
                return this.HasValue && this.IsEnabled;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the row editor should be shown at all.
        /// Even if this is set to true, the editor might not be visible when a single-row table was selected.
        /// </summary>
        public bool ShouldShowRow
        {
            get
            {
                return this.shouldShowRow;
            }

            set
            {
                if (this.SetProperty(ref this.shouldShowRow, value, () => this.ShouldShowRow))
                {
                    this.UpdateRowVisible();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the row editor is visible.
        /// </summary>
        public bool IsRowVisible
        {
            get
            {
                return this.isRowVisible;
            }

            private set
            {
                this.SetProperty(ref this.isRowVisible, value, () => this.IsRowVisible);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the language editor should be shown at all.
        /// Even if this is set to true, the editor might not be visible when a single-language table was selected.
        /// </summary>
        public bool ShouldShowLanguage
        {
            get
            {
                return this.shouldShowLanguage;
            }

            set
            {
                if (this.SetProperty(ref this.shouldShowLanguage, value, () => this.ShouldShowLanguage))
                {
                    this.UpdateLanguageVisible();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the language editor is visible.
        /// </summary>
        public bool IsLanguageVisible
        {
            get
            {
                return this.isLanguageVisible;
            }

            private set
            {
                this.SetProperty(ref this.isLanguageVisible, value, () => this.IsLanguageVisible);
            }
        }

        /// <summary>
        /// Gets the generic dictionary used by this editor.
        /// </summary>
        public Dictionary Dictionary
        {
            get
            {
                // this is actually one shared dictionary (should not be changed ever)
                return dictionary;
            }
        }

        /// <summary>
        /// Gets the name of the table and column selected by the user.
        /// This value should only be used for display, not for identifying a table or column.
        /// </summary>
        public string TableColumnName
        {
            get
            {
                return this.tableColumnName;
            }

            private set
            {
                this.SetProperty(ref this.tableColumnName, value, () => this.TableColumnName);
            }
        }

        /// <summary>
        /// Gets the column selected by the user.
        /// </summary>
        public Column SelectedColumn
        {
            get
            {
                return this.selectedColumn;
            }

            private set
            {
                if (!this.SetProperty(ref this.selectedColumn, value, () => this.SelectedColumn))
                {
                    return;
                }

                this.MakeDirty();
                this.UpdateRowVisible();
                this.UpdateLanguageVisible();
                this.UpdateErrors();
                if (value == null)
                {
                    this.TableColumnName = null;
                    return;
                }

                if (this.SelectedLanguage == null)
                {
                    this.SelectedLanguage = this.Dictionary.Languages.FirstOrDefault();
                }

                var table = this.GetSelectedTable();
                if (table == null)
                {
                    this.TableColumnName = value.Name;
                    return;
                }

                this.TableColumnName = table.Name + " > " + value.Name;
            }
        }

        /// <summary>
        /// Gets or sets the selected column or table.
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                if (this.SetProperty(ref this.selectedItem, value, () => this.SelectedItem))
                {
                    this.SelectedColumn = value as Column;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        public Language SelectedLanguage
        {
            get
            {
                return this.selectedLanguage;
            }

            set
            {
                if (this.SetProperty(ref this.selectedLanguage, value, () => this.SelectedLanguage))
                {
                    this.MakeDirty();
                    this.UpdateErrors();
                }
            }
        }

        /// <summary>
        /// Gets or sets the chosen row index.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }

            set
            {
                if (this.SetProperty(ref this.rowIndex, value, () => this.RowIndex))
                {
                    this.MakeDirty();
                }
            }
        }

        private Table GetSelectedTable()
        {
            return this.SelectedColumn == null
                       ? null
                       : this.Dictionary.Tables.FirstOrDefault(t => t.Columns.Contains(this.SelectedColumn));
        }

        private void UpdateRowVisible()
        {
            if (!this.ShouldShowRow)
            {
                this.IsRowVisible = false;
                return;
            }

            var table = this.GetSelectedTable();
            this.IsRowVisible = table == null || table.MultiRow;
        }

        private void UpdateLanguageVisible()
        {
            if (!this.ShouldShowLanguage)
            {
                this.IsLanguageVisible = false;
                return;
            }

            var table = this.GetSelectedTable();
            this.IsLanguageVisible = table == null || table.MultiLanguage;
        }

        private void UpdateErrors()
        {
            this.SetError(
                "SelectedItem",
                this.HasValue && this.SelectedColumn == null ? ErrorState.Error : ErrorState.Ok,
                AdminStrings.Errors_NoItemSelected);
            var errorState = this.HasValue && this.IsLanguageVisible && this.SelectedLanguage == null
                                 ? ErrorState.Error
                                 : ErrorState.Ok;
            this.SetError("SelectedLanguage", errorState, AdminStrings.Errors_NoItemSelected);
        }
    }
}
