// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Operation.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceBench.ViewModels
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The operation.
    /// </summary>
    public class Operation : ViewModelBase
    {
        /// <summary>
        /// Stores the id of the operation.
        /// </summary>
        private int id;

        /// <summary>
        /// Stores the name of the operation.
        /// </summary>
        private string name;

        /// <summary>
        /// Gets or sets the id of the operation.
        /// </summary>
        /// <value>
        /// The identifier of the operation.
        /// </value>
        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                if (value != this.id)
                {
                    this.id = value;
                    this.RaisePropertyChanged(() => this.Id);
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the operation.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.RaisePropertyChanged(() => this.Name);
                }
            }
        }
    }
}