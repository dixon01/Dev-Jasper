// Gorba.Motion.Protran
// Luminator.Protran.MessagingProtocol
// $Rev::                                   
// 

namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces
{
    using Luminator.AdhocMessaging.Interfaces;

    public interface IAdHocMessageService
    {
        void Configure(IAdHocMessageServiceConfig config, IAdhocManager adhocManager);
        
        IAdHocMessages GetUnitAdHocMessages(IAdHocGetMessagesRequest request);

        IAdHocRegistrationResponse RegisterVehicleAndUnit(IAdHocRegisterRequest request);
    }
}