namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces
{
    using System;

    public interface IAdHocGetMessagesRequest : IAdHocRequest
    {
        DateTime? UnitLocalTimeStamp { get; set; }
    }
}