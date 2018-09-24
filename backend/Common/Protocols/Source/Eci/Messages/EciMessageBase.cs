// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciMessageBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    using System.ComponentModel;
    using System.Text;

    /// <summary>
    /// Base class for ECI messages
    /// </summary>
    public abstract class EciMessageBase
    {
        /// <summary>
        /// Gets or sets the vehicle id.
        /// </summary>
        public int VehicleId { get; set; }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public abstract EciMessageCode MessageType { get; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(this);
            StringBuilder builder = new StringBuilder();
            foreach (PropertyDescriptor pd in coll)
            {
                var value = pd.GetValue(this);
                if (value != null)
                {
                    builder.Append(string.Format("{0} : {1}", pd.Name, value));
                }
            }

            return builder.ToString();
        }
    }
}
