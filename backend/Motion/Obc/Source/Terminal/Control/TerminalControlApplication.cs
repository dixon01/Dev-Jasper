// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TerminalControlApplication.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TerminalControlApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Common;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The terminal control application.
    /// </summary>
    public class TerminalControlApplication : ApplicationBase, IManageable
    {
        /// <summary>
        /// The management name.
        /// </summary>
        public static readonly string ManagementName = "TerminalControl";

        private Context context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalControlApplication"/> class.
        /// </summary>
        public TerminalControlApplication()
        {
            var root = MessageDispatcher.Instance.ManagementProviderFactory.LocalRoot;
            var provider = MessageDispatcher.Instance.ManagementProviderFactory.CreateManagementProvider(
                ManagementName, root, this);
            root.AddChild(provider);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        /// <summary>
        /// The do run.
        /// </summary>
        protected override void DoRun()
        {
            TimeProvider.Current = new GpsTimeProvider();

            RemoteEventHandler.Initialize();

            // ReSharper disable once ObjectCreationAsStatement
            new ml();
            SetupUiFactory();

            this.context = new Context();
            this.context.Start();
            this.context.UiRoot.Run();
        }

        /// <summary>
        /// The do stop.
        /// </summary>
        protected override void DoStop()
        {
            this.context.UiRoot.Stop();
        }

        private static void SetupUiFactory()
        {
#if WindowsCE
            UiFactory.CreateInstance<Gui.GuiFactory>();
#else
            UiFactory.CreateInstance<C74.C74UiFactory>();
#endif
        }
    }
}
