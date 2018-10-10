namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Holds the settings for creating a device
    /// </summary>
    public class DeviceSettings : ICloneable
    {
        public uint AdapterOrdinal;
        public DeviceType DeviceType;
        public Format AdapterFormat;
        public CreateFlags BehaviorFlags;
        public PresentParameters presentParams;

        #region ICloneable Members
        /// <summary>Clone this object</summary>
        public DeviceSettings Clone()
        {
            DeviceSettings clonedObject = new DeviceSettings();
            clonedObject.presentParams = (PresentParameters)this.presentParams.Clone();
            clonedObject.AdapterFormat = this.AdapterFormat;
            clonedObject.AdapterOrdinal = this.AdapterOrdinal;
            clonedObject.BehaviorFlags = this.BehaviorFlags;
            clonedObject.DeviceType = this.DeviceType;

            return clonedObject;
        }
        /// <summary>Clone this object</summary>
        object ICloneable.Clone() { throw new NotSupportedException("Use the strongly typed overload instead."); }
        #endregion
    }
}