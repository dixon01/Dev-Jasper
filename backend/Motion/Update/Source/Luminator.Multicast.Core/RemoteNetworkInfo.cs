namespace Luminator.Multicast.Core
{
    using System;
    using System.IO;
    using System.Net;
    using System.Xml.Serialization;

    [Serializable]
    public class RemoteNetworkInfo
    {
        #region Public Properties

        public string Host
        {
            get
            {
                return this.RemoteIpAddress?.ToString() ?? string.Empty;
            }
            set
            {
                this.RemoteIpAddress = IPAddress.Parse(value);
            }
        }

        public string SubnetMaskString
        {
            get
            {
                return this.SubnetMask?.ToString() ?? string.Empty;
            }
            set
            {
                this.SubnetMask = IPAddress.Parse(value);
            }
        }

        public string Password { get; set; }

        [XmlIgnore]
        public IPAddress RemoteIpAddress { get; set; }


        public string Username { get; set; }

        [XmlIgnore]
        public IPAddress SubnetMask { get; set; }

        public bool UpdateComplete { get; set; }

        #endregion

        public bool Save(string path)
        {
            try
            {
                var xs = new XmlSerializer(typeof(RemoteNetworkInfo));
                TextWriter tw = new StreamWriter(path);
                xs.Serialize(tw, this);
                tw.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        public RemoteNetworkInfo Load(string path)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(RemoteNetworkInfo));
                using (var sr = new StreamReader(path))
                {
                    var remoteNetworkInfo1 = (RemoteNetworkInfo)xs.Deserialize(sr);
                    return remoteNetworkInfo1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public override  string ToString()
        {
            return $" Host: {this.Host}, User : {this.Username}, Pass: {this.Password}, Subnet: {this.SubnetMaskString}, UpdateComplete : {this.UpdateComplete}";
        }
    }
}