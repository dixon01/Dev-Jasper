namespace Gorba.Common.Configuration.Protran.XimpleProtocol
{
    using Gorba.Motion.Infomedia.Entities.Messages;

    public class AudioZonePresentationValues
    {
        public AudioZonePresentationValues(string interior, string exterior, string both)
        {
            this.Interior = interior;
            this.Exterior = exterior;
            this.Both = both;
            this.None = AudioZoneTypes.None.ToString();
        }

        public AudioZonePresentationValues() : this(((int)AudioZoneTypes.Interior).ToString(), ((int)AudioZoneTypes.Exterior).ToString(), ((int)AudioZoneTypes.Both).ToString())
        {
            this.None = AudioZoneTypes.None.ToString();
        }

        public string Interior { get; set; }
        public string Exterior { get; set; }
        public string Both { get; set; }
        public string None { get; set; }
    }
}