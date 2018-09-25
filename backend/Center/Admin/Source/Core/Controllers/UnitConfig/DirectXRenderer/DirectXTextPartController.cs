// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectXTextPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectXTextPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.DirectXRenderer
{
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;

    /// <summary>
    /// The DirectX text part controller.
    /// </summary>
    public class DirectXTextPartController : MultiEditorPartControllerBase
    {
        private const string TextModeKey = "TextMode";

        private const string FontQualityKey = "FontQuality";

        private SelectionEditorViewModel textMode;

        private SelectionEditorViewModel fontQuality;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectXTextPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public DirectXTextPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.DirectXRenderer.Text, parent)
        {
        }

        /// <summary>
        /// Gets the text mode.
        /// </summary>
        public TextMode TextMode
        {
            get
            {
                return this.textMode.SelectedValue is TextMode
                           ? (TextMode)this.textMode.SelectedValue
                           : TextMode.FontSprite;
            }
        }

        /// <summary>
        /// Gets the text mode.
        /// </summary>
        public FontQualities FontQuality
        {
            get
            {
                return this.fontQuality.SelectedValue is FontQualities
                           ? (FontQualities)this.fontQuality.SelectedValue
                           : FontQualities.ClearType;
            }
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            this.textMode.SelectValue(partData.GetEnumValue(TextMode.FontSprite, TextModeKey));
            this.fontQuality.SelectValue(partData.GetEnumValue(FontQualities.Default, FontQualityKey));
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetEnumValue(this.TextMode, TextModeKey);
            partData.SetEnumValue(this.FontQuality, FontQualityKey);
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = new MultiEditorPartViewModel
                            {
                                DisplayName = AdminStrings.UnitConfig_DirectX_Text,
                                Description = AdminStrings.UnitConfig_DirectX_Text_Description
                            };

            this.AddTextModeEditor(viewModel);
            this.AddFontQualityEditor(viewModel);

            return viewModel;
        }

        private void AddFontQualityEditor(MultiEditorPartViewModel viewModel)
        {
            this.fontQuality = new SelectionEditorViewModel
                               {
                                   Label = AdminStrings.UnitConfig_DirectX_Text_FontQuality
                               };

            this.fontQuality.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_DefaultFontSprite,
                    FontQualities.Default,
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_Default_Description));

            this.fontQuality.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_Draft,
                    FontQualities.Draft,
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_Draft_Desciption));

            this.fontQuality.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_Proof,
                    FontQualities.Proof,
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_Proof_Description));

            this.fontQuality.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_NonAntiAliased,
                    FontQualities.NonAntiAliased,
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_NonAntiAliased_Description));

            this.fontQuality.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_AntiAliased,
                    FontQualities.AntiAliased,
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_AntiAliased_Description));

            this.fontQuality.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_ClearType,
                    FontQualities.ClearType,
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_ClearType_Description));

            this.fontQuality.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_ClearTypeNatural,
                    FontQualities.ClearTypeNatural,
                    AdminStrings.UnitConfig_DirectX_Text_FontQuality_ClearTypeNatural_Description));

            viewModel.Editors.Add(this.fontQuality);
        }

        private void AddTextModeEditor(MultiEditorPartViewModel viewModel)
        {
            this.textMode = new SelectionEditorViewModel { Label = AdminStrings.UnitConfig_DirectX_Text_TextMode };
            this.textMode.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_DirectX_Text_TextMode_DirectX,
                    TextMode.FontSprite));
            this.textMode.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_DirectX_Text_TextMode_Gdi,
                    TextMode.Gdi));
            viewModel.Editors.Add(this.textMode);
            this.textMode.PropertyChanged += this.OnSelectedTextModeChanged;
        }

        private void OnSelectedTextModeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedOption")
            {
                return;
            }

            if (this.textMode.SelectedValue is TextMode)
            {
                var defaultQuality =
                        this.fontQuality.Options.FirstOrDefault(q => ((FontQualities)q.Value == FontQualities.Default));
                if (defaultQuality != null)
                {
                    var selectedValue = (TextMode)this.textMode.SelectedValue;
                    if (selectedValue == TextMode.FontSprite)
                    {
                        defaultQuality.Label = AdminStrings.UnitConfig_DirectX_Text_FontQuality_DefaultFontSprite;
                    }
                    else if (selectedValue == TextMode.Gdi)
                    {
                        defaultQuality.Label = AdminStrings.UnitConfig_DirectX_Text_FontQuality_DefaultFontGdi;
                    }
                }
            }
        }
    }
}