// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleSelectionAction.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultipleSelectionAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Helpers
{
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    using Gorba.Center.Common.Wpf.Framework.ViewModels.MasterDetails;

    using NLog;

    /// <summary>
    /// Defines the logic to ensure that an ItemsListBase object has the single item selected based on its selected
    /// items.
    /// </summary>
    public class MultipleSelectionAction : TriggerAction<DataGrid>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Invokes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void Invoke(object parameter)
        {
            var itemsList = this.AssociatedObject.DataContext as ItemsListBase;
            if (itemsList == null)
            {
                Logger.Debug("Null DataContext when executing the multiple selection action");
                return;
            }

            var items = itemsList.ListSelectedItems().ToList();
            if (items.Count == 1)
            {
                itemsList.Stage.SelectedItem = items[0];
                return;
            }

            itemsList.Stage.SelectedItem = null;
        }
    }
}
