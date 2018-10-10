using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminator.AudioSwitch.WpfSerialPortTester.Models
{
    public class PinMeaning
    {
        public String Door1 { get; set; }
        public String Door2 { get; set; }
        public String StopRequest { get; set; }
        public String ADAStopRequest { get; set; }
        public String PushToTalk { get; set; }
        public String InteriorSelect { get; set; }
        public String ExteriorSelect { get; set; }
        public String Odometer { get; set; }
        public String Reverse { get; set; }
        public String InteriorSpeakerPriority { get; set; }
        public String ExteriorSpeakerPriority { get; set; }
        public String RadioSpeakerPriority { get; set; }

        public List<String> Options { get; set; }
    }
}
