// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdminShellFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AdminShellFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels
{
    using System.ComponentModel.Composition;

    using Gorba.Center.Admin.Core.Views;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// Factory to create <see cref="AdminShellWindow"/>s.
    /// </summary>
    [Export]
    public class AdminShellFactory : WindowFactory<AdminShellWindow>
    {
    }
}