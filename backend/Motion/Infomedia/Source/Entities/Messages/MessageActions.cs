namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;

    /// <summary>The message actions.</summary>
    [Serializable]
    [Flags]
    public enum MessageActions
    {
        /// <summary>The none.</summary>
        None = 0x0, 

        /// <summary>The read.</summary>
        Get = 0x1, 

        /// <summary>The write.</summary>
        Set = 0x2,

        /// <summary>The OK.</summary>
        OK = 0x4, 
    }
}