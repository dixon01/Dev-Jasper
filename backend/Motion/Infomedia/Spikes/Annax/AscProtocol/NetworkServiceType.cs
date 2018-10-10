namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol
{
    using System;

    [Flags]
    public enum NetworkServiceType
    {
        Ackd = 0x00,
        UnAck = 0x01,
        Request = 0x02,
        Response = 0x06,

        AnswerAckd = 0x10,
        AnswerRequest = 0x12,
        AnswerResponse = 0x16,
    }
}