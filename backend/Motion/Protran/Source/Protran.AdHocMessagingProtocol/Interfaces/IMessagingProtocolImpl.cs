// Gorba.Motion.Protran
// Luminator.Protran.MessagingProtocol
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Protran.Core.Protocols;

    /// <summary>The MessagingProtocolImpl interface.</summary>
    public interface IMessagingProtocolImpl : IProtocol, IManageableObject, IDisposable
    {
        IAdHocMessageService AdHocMessageService { get; set; }

        IAdHocMessagingProtocolConfig Config { get; set; }

        VehicleUnitInfo CurrentVehicleUnitInfo { get; set; }

        Dictionary Dictionary { get; }

        ServiceRunState ServiceRunState { get; set; }

        Ximple CreateXimpleAdHocMessage(IList<IXimpleAdHocMessage> messages);

        void Start(ServiceRunState runState);

        void Configure(Dictionary dictionary, IAdHocMessagingProtocolConfig config, IAdHocMessageService adHocMessageService);
    }
}