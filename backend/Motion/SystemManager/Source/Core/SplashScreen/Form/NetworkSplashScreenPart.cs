// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkSplashScreenPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NetworkSplashScreenPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using NLog;

    // [WES] we use an #if here because like this we can reuse the entire code without having to write it twice
#if WindowsCE
    using OpenNETCF.Net.NetworkInformation;
#else
    using GatewayIPAddressInformation = System.Net.NetworkInformation.GatewayIPAddressInformation;
    using INetworkInterface = System.Net.NetworkInformation.NetworkInterface;
    using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;
    using NetworkInterfaceType = System.Net.NetworkInformation.NetworkInterfaceType;
    using OperationalStatus = System.Net.NetworkInformation.OperationalStatus;
    using UnicastIPAddressInformation = System.Net.NetworkInformation.UnicastIPAddressInformation;

#endif

    /// <summary>
    /// Splash screen part that shows information about the network configuration.
    /// </summary>
    public class NetworkSplashScreenPart : SplashScreenPartBase
    {
        private const string StringMeasureSpacer = "WM";

        private const int MaxNameLength = 50;

        private static readonly Logger Logger = LogHelper.GetLogger<NetworkSplashScreenPart>();

        private readonly NetworkSplashScreenItem config;

        private Font font;

        private SolidBrush brush;

        private int nameWidth;

        private int addressWidth;

        private int gatewayWidth;

        private int macWidth;

        private int statusWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSplashScreenPart"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public NetworkSplashScreenPart(NetworkSplashScreenItem config)
        {
            this.config = config;
        }

        private delegate string StringGetter(INetworkInterface netIf);

        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }

            set
            {
                base.ForeColor = value;
                this.brush = new SolidBrush(value);
            }
        }

        /// <summary>
        /// Paints this part.
        /// </summary>
        /// <param name="g">
        /// The graphics object.
        /// </param>
        /// <param name="rect">
        /// The rectangle into which we should paint.
        /// </param>
        public override void Paint(Graphics g, Rectangle rect)
        {
            var spaceLeft = rect.Width - this.nameWidth - this.addressWidth - this.gatewayWidth - this.macWidth
                            - this.statusWidth;

            var x = rect.X + (spaceLeft / 2);
            var y = rect.Y;

            var netIfs = this.GetNetworkInterfaces();

            x = this.DrawString(g, netIfs, this.GetNameString, x, y, this.nameWidth, this.config.Name);
            x = this.DrawString(g, netIfs, this.GetIpAddressString, x, y, this.addressWidth, this.config.Ip);
            x = this.DrawString(g, netIfs, this.GetGatewayAddressString, x, y, this.gatewayWidth, this.config.Gateway);
            x = this.DrawString(g, netIfs, this.GetMacAddressString, x, y, this.macWidth, this.config.Mac);
            this.DrawString(g, netIfs, n => n.OperationalStatus.ToString(), x, y, this.statusWidth, this.config.Status);
        }

        /// <summary>
        /// Implementation of the scaling.
        /// </summary>
        /// <param name="factor">
        /// The factor (0.0 .. 1.0).
        /// </param>
        /// <param name="graphics">
        /// The graphics to calculate the scaling.
        /// </param>
        /// <returns>
        /// The calculated height of this part with the used scaling factor.
        /// </returns>
        protected override Size DoScale(double factor, Graphics graphics)
        {
            if (this.font != null)
            {
                this.font.Dispose();
            }

            this.font = new Font(FontFamily.GenericSansSerif, (float)(20 * factor), FontStyle.Bold);

            var netIfs = this.GetNetworkInterfaces();

            var size = this.MeasureString(graphics, netIfs, n => n.Name);
            this.nameWidth = this.config.Name ? size.Width : 0;
            this.addressWidth = this.GetStringWidth(graphics, netIfs, this.GetIpAddressString, this.config.Ip);
            this.gatewayWidth = this.GetStringWidth(
                graphics, netIfs, this.GetGatewayAddressString, this.config.Gateway);
            this.macWidth = this.GetStringWidth(graphics, netIfs, this.GetMacAddressString, this.config.Mac);
            this.statusWidth = this.GetStringWidth(
                graphics, netIfs, n => n.OperationalStatus.ToString(), this.config.Status);

            return new Size
                       {
                           Height = size.Height,
                           Width = this.nameWidth + this.addressWidth + this.gatewayWidth + this.macWidth
                            + this.statusWidth
                       };
        }

        private IList<INetworkInterface> GetNetworkInterfaces()
        {
            var all = NetworkInterface.GetAllNetworkInterfaces();
            var interfaces = new List<INetworkInterface>(all.Length);
            if (string.IsNullOrEmpty(this.config.StatusFilter))
            {
                foreach (var netIf in all)
                {
                    if (netIf.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        interfaces.Add(netIf);
                    }
                }

                return interfaces;
            }

            var statuses = ArrayUtil.ConvertAll(this.config.StatusFilter.Split(','), s => s.Trim());
            var filter = new List<OperationalStatus>(statuses.Length);
            foreach (var status in statuses)
            {
                try
                {
                    filter.Add((OperationalStatus)Enum.Parse(typeof(OperationalStatus), status, true));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't parse status: " + status);
                }
            }

            foreach (var netIf in all)
            {
                if (netIf.NetworkInterfaceType != NetworkInterfaceType.Loopback
                    && filter.Contains(netIf.OperationalStatus))
                {
                    interfaces.Add(netIf);
                }
            }

            return interfaces;
        }

        private string GetNameString(INetworkInterface netif)
        {
            var name = netif.Name;
            if (name.Length > MaxNameLength)
            {
                name = name.Substring(0, MaxNameLength - 2) + "...";
            }

            return name;
        }

        private string GetIpAddressString(INetworkInterface netIf)
        {
            var sb = new StringBuilder();
            var addresses = new List<UnicastIPAddressInformation>(netIf.GetIPProperties().UnicastAddresses);
            foreach (var addr in addresses.FindAll(a => a.Address.AddressFamily == AddressFamily.InterNetwork))
            {
                sb.Append(addr.Address);
                if (addr.IPv4Mask != null)
                {
                    var maskBits = this.GetIpMaskBits(addr.IPv4Mask);
                    sb.Append("/").Append(maskBits);
                }

                sb.Append(", ");
            }

            if (sb.Length == 0)
            {
                return string.Empty;
            }

            sb.Length -= 2;

            return sb.ToString();
        }

        private int GetIpMaskBits(IPAddress mask)
        {
            var bits = 0;
            foreach (var b in mask.GetAddressBytes())
            {
                for (int i = 7; i >= 0; i--)
                {
                    if ((b & (1 << i)) == 0)
                    {
                        return bits;
                    }

                    bits++;
                }
            }

            return bits;
        }

        private string GetGatewayAddressString(INetworkInterface netIf)
        {
            var sb = new StringBuilder();
            var addresses = new List<GatewayIPAddressInformation>();
            foreach (GatewayIPAddressInformation addr in netIf.GetIPProperties().GatewayAddresses)
            {
                addresses.Add(addr);
            }

            foreach (var addr in addresses.FindAll(a => a.Address.AddressFamily == AddressFamily.InterNetwork))
            {
                sb.Append(addr.Address);

                sb.Append(", ");
            }

            if (sb.Length == 0)
            {
                return string.Empty;
            }

            sb.Length -= 2;

            return sb.ToString();
        }

        private string GetMacAddressString(INetworkInterface netIf)
        {
            return BitConverter.ToString(netIf.GetPhysicalAddress().GetAddressBytes());
        }

        private int GetStringWidth(
            Graphics graphics, IEnumerable<INetworkInterface> netIfs, StringGetter getter, bool draw)
        {
            if (!draw)
            {
                return 0;
            }

            return this.MeasureString(graphics, netIfs, getter).Width;
        }

        private Size MeasureString(Graphics graphics, IEnumerable<INetworkInterface> netIfs, StringGetter getter)
        {
            var measureString = new StringBuilder();
            foreach (var netIf in netIfs)
            {
                this.AppendLine(measureString.Append(getter(netIf)), StringMeasureSpacer);
            }

            var size = graphics.MeasureString(measureString.ToString(), this.font);
            return new Size((int)System.Math.Ceiling(size.Width), (int)System.Math.Ceiling(size.Height));
        }

        private int DrawString(
            Graphics graphics,
            IEnumerable<INetworkInterface> netIfs,
            StringGetter getter,
            int x,
            int y,
            int width,
            bool draw)
        {
            if (!draw)
            {
                return x;
            }

            var sb = new StringBuilder();
            foreach (var netIf in netIfs)
            {
                this.AppendLine(sb, getter(netIf));
            }

            graphics.DrawString(
                sb.ToString(),
                this.font,
                this.brush,
                new RectangleF(x, y, width, this.Size.Height),
                SplashScreenPartBase.DefaultStringFormat);
            return x + width;
        }
    }
}