// -----------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ProtectedOperationsTester.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Text;

    using ProtectedOperationsTester.ViewModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Bootstrapper
    {
        public Shell Bootstrap()
        {
            var assemblyCatalog = new AssemblyCatalog(this.GetType().Assembly);
            var compositionContainer = new CompositionContainer(assemblyCatalog);
            return compositionContainer.GetExportedValue<Shell>();
        }
    }
}
