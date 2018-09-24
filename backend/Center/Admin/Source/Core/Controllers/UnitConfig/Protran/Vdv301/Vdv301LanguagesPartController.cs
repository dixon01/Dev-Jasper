// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301LanguagesPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301LanguagesPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Protran.VDV301;
    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// The VDV 301 languages part controller.
    /// </summary>
    public class Vdv301LanguagesPartController : FilteredPartControllerBase
    {
        private const string Dash = "-";

        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301LanguagesPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public Vdv301LanguagesPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Vdv301Protocol.Languages, parent)
        {
        }

        /// <summary>
        /// Gets all defined language mappings.
        /// </summary>
        /// <returns>
        /// The list of all valid mappings.
        /// </returns>
        public IEnumerable<LanguageMappingConfig> GetLanguageMappings()
        {
            return
                this.GetEditors()
                    .Where(e => e.Text != Dash)
                    .Select(e => new LanguageMappingConfig { Vdv301Input = e.Text, XimpleOutput = e.Language.Name });
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            foreach (var editor in this.GetEditors())
            {
                editor.Text = partData.GetValue(Dash, editor.Language.Name);
            }

            this.UpdateErrors();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            foreach (var editor in this.GetEditors())
            {
                partData.SetValue(editor.Text, editor.Language.Name);
            }
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = new MultiEditorPartViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_Vdv301_Languages;
            viewModel.Description = AdminStrings.UnitConfig_Vdv301_Languages_Description;

            var languages = new GenericUsageEditorViewModel().Dictionary.Languages;
            foreach (var language in languages)
            {
                var editor = new LanguageTextEditorViewModel(language);
                viewModel.Editors.Add(editor);
            }

            return viewModel;
        }

        /// <summary>
        /// Raises the <see cref="PartControllerBase.ViewModelUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseViewModelUpdated(EventArgs e)
        {
            base.RaiseViewModelUpdated(e);
            this.UpdateErrors();
        }

        private IEnumerable<LanguageTextEditorViewModel> GetEditors()
        {
            return this.ViewModel.Editors.OfType<LanguageTextEditorViewModel>();
        }

        private void UpdateErrors()
        {
            var errorState = this.GetEditors().Any(e => e.Text != Dash)
                                 ? ErrorState.Ok
                                 : (this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing);
            foreach (var editor in this.GetEditors())
            {
                editor.SetError("Text", errorState, AdminStrings.Errors_NoLanguageDefined);
            }
        }

        private class LanguageTextEditorViewModel : TextEditorViewModel
        {
            public LanguageTextEditorViewModel(Language language)
            {
                this.Language = language;
                this.Label = language.Description;
            }

            public Language Language { get; private set; }
        }
    }
}