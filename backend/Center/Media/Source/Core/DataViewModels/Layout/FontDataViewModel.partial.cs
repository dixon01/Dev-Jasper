// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Properties;

    /// <summary>
    /// Partial implementation of the TextElementDataViewModel.
    /// </summary>
    public partial class FontDataViewModel
    {
        partial void Initialize(FontDataModel dataModel)
        {
            if (dataModel == null)
            {
                this.SetDefaultValues();
            }
        }

        private void SetDefaultValues()
        {
            this.Color = new DataValue<string>(Settings.Default.FontColor);
            this.Face = new DataValue<string>(Settings.Default.Font);
            this.Height = new DataValue<int>(Settings.Default.FontHeight);
            this.Weight = new DataValue<int>(Settings.Default.FontWeight);
        }
    }
}
