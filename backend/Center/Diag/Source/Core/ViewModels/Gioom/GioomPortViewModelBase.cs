// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GioomPortViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GioomPortViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Gioom
{
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Diag.Core.ViewModels.Unit;
    using Gorba.Common.Medi.Core;

    /// <summary>
    /// The base class for all view models that represent a remote GIOoM port.
    /// </summary>
    public abstract class GioomPortViewModelBase : ViewModelBase
    {
        private UnitViewModelBase unit;

        private MediAddress address;

        private string name;

        private IOValueViewModel value;

        private bool isReadable;

        private bool isWritable;

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public IDiagShell Shell
        {
            get
            {
                return this.Unit.Shell;
            }
        }

        /// <summary>
        /// Gets or sets the unit to which the port belongs.
        /// </summary>
        public UnitViewModelBase Unit
        {
            get
            {
                return this.unit;
            }

            set
            {
                this.SetProperty(ref this.unit, value, () => this.Unit);
            }
        }

        /// <summary>
        /// Gets or sets the Medi address of the application to which the port belongs.
        /// </summary>
        public MediAddress Address
        {
            get
            {
                return this.address;
            }

            set
            {
                this.SetProperty(ref this.address, value, () => this.Address);
            }
        }

        /// <summary>
        /// Gets or sets the name of the port.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this port is readable.
        /// </summary>
        public bool IsReadable
        {
            get
            {
                return this.isReadable;
            }

            set
            {
                this.SetProperty(ref this.isReadable, value, () => this.IsReadable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this port is writable.
        /// </summary>
        public bool IsWritable
        {
            get
            {
                return this.isWritable;
            }

            set
            {
                this.SetProperty(ref this.isWritable, value, () => this.IsWritable);
            }
        }

        /// <summary>
        /// Gets or sets the I/O value of the port.
        /// </summary>
        public IOValueViewModel Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.SetProperty(ref this.value, value, () => this.Value))
                {
                    this.HandleValueChanged();
                }
            }
        }

        /// <summary>
        /// This method is called whenever <see cref="Value"/> changes.
        /// </summary>
        protected virtual void HandleValueChanged()
        {
        }
    }
}