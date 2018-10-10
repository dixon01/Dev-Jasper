// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareDescriptors.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareDescriptors type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// All known hardware descriptors for Gorba products.
    /// </summary>
    public static class HardwareDescriptors
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// The Inform product line (old Gorba products).
        /// </summary>
        public static class Thoreb
        {
            /// <summary>
            /// Gets the c 90 c 74.
            /// </summary>
            public static HardwareDescriptor C90C74
            {
                get
                {
                    return new HardwareDescriptor
                               {
                                   Name = "Thoreb C90 C74",
                                   Platform =
                                       new ThorebPlatformDescriptor
                                           {
                                               // TODO replace fake entrys with correct ones
                                               Gps = new GpsConfigFake("GPS type", 42),
                                               Inputs =
                                                   {
                                                       new InputDescriptor(1, "CTS COM1"),
                                                       new InputDescriptor(2, "CTS COM2")
                                                   },
                                               Outputs =
                                                   {
                                                       new OutputDescriptor(1, "RTS COM1"),
                                                       new OutputDescriptor(2, "RTS COM2")
                                                   },
                                               SerialPorts =
                                                   {
                                                       new SerialPortDescriptor("COM1"),
                                                       new SerialPortDescriptor("COM2")
                                                   }
                                           },
                                   OperatingSystem =
                                       new LinuxEmbeddedDescriptor
                                           {
                                               Version = OperatingSystemVersion.LinuxFake
                                           }
                               };
                }
            }
        }

        /// <summary>
        /// The Inform product line (old Gorba products).
        /// </summary>
        public static class Inform
        {
            /// <summary>
            /// Gets the descriptor for Inform TFT Topbox PM600.
            /// </summary>
            public static HardwareDescriptor TopboxPM600
            {
                get
                {
                    return new HardwareDescriptor
                               {
                                   Name = "Inform TFT Topbox PM600",
                                   Platform =
                                       new InformPlatformDescriptor
                                           {
                                               DisplayAdapters =
                                                   {
                                                       new DisplayAdapterDescriptor(0, DisplayConnectionType.Dvi),
                                                       new DisplayAdapterDescriptor(1, DisplayConnectionType.Dvi)
                                                   },
                                               HasGenericButton = false,
                                               HasGenericLed = false,
                                               Inputs =
                                                   {
                                                       new InputDescriptor(1, "CTS COM1"),
                                                       new InputDescriptor(2, "CTS COM2")
                                                   },
                                               Outputs =
                                                   {
                                                       new OutputDescriptor(1, "RTS COM1"),
                                                       new OutputDescriptor(2, "RTS COM2")
                                                   },
                                               SerialPorts =
                                                   {
                                                       new SerialPortDescriptor("COM1"),
                                                       new SerialPortDescriptor("COM2")
                                                   }
                                           },
                                   OperatingSystem =
                                       new WindowsEmbeddedDescriptor
                                           {
                                               Version = OperatingSystemVersion.WindowsXPe
                                           }
                               };
                }
            }

            /// <summary>
            /// Gets the descriptor for Inform TFT Topbox PM800.
            /// </summary>
            public static HardwareDescriptor TopboxPM800
            {
                get
                {
                    var descriptor = TopboxPM600;
                    descriptor.Name = "Inform TFT Topbox PM800";
                    return descriptor;
                }
            }

            /// <summary>
            /// Gets the descriptor for Inform TFT Topbox Atom.
            /// </summary>
            public static HardwareDescriptor TopboxAtom
            {
                get
                {
                    var descriptor = TopboxPM600;
                    descriptor.Name = "Inform TFT Topbox Atom";
                    return descriptor;
                }
            }

            /// <summary>
            /// Gets the descriptor for Inform TFT Compact.
            /// </summary>
            public static HardwareDescriptor Compact
            {
                get
                {
                    var descriptor = TopboxAtom;
                    descriptor.Name = "Inform TFT Compact";
                    ((InformPlatformDescriptor)descriptor.Platform).BuiltInScreen = new DisplayDescriptor(1440, 900);
                    return descriptor;
                }
            }
        }

        /// <summary>
        /// The InfoVision product line (new MGI based products).
        /// </summary>
        public static class InfoVision
        {
            /// <summary>
            /// Gets the descriptor for InfoVision Topbox PC-2 Atom.
            /// </summary>
            [SuppressMessage("StyleCopPlus.StyleCopPlusRules",
                "SP2102:PropertyMustNotContainMoreLinesThan",
                Justification = "Required for nice layout.")]
            public static HardwareDescriptor TopboxPC2Atom
            {
                get
                {
                    return new HardwareDescriptor
                    {
                        Name = "InfoVision Topbox PC-2 Atom",
                        Platform =
                            new InfoVisionPlatformDescriptor
                            {
                                DisplayAdapters =
                                                   {
                                                       new DisplayAdapterDescriptor(0, DisplayConnectionType.Dvi),
                                                       new DisplayAdapterDescriptor(1, DisplayConnectionType.Dvi)
                                                   },
                                HasSharedRs485Port = false,
                                Transceivers =
                                                   {
                                                       new MultiProtocolTransceiverDescriptor(0),
                                                       new MultiProtocolTransceiverDescriptor(1)
                                                   },
                                HasGenericButton = true,
                                HasGenericLed = true,
                                Inputs =
                                                   {
                                                       new InputDescriptor(0, "IN0 (Pins 1/2)"),
                                                       new InputDescriptor(1, "IN1 (Pins 3/4)"),
                                                       new InputDescriptor(2, "IN2 (Pins 5/6)"),
                                                       new InputDescriptor(3, "IN3 (Pins 7/8)")
                                                   },
                                Outputs =
                                                   {
                                                       new OutputDescriptor(4, "OUT0 (Pins 9/10)"),
                                                       new OutputDescriptor(5, "OUT1 (Pins 11/12)"),
                                                       new OutputDescriptor(6, "OUT2 (Pins 13/14)"),
                                                       new OutputDescriptor(7, "OUT3 (Pins 15/16)")
                                                   },
                                SerialPorts =
                                                   {
                                                       new SerialPortDescriptor("COM3", true),
                                                       new SerialPortDescriptor("COM4")
                                                   }
                            },
                        OperatingSystem =
                            new WindowsEmbeddedDescriptor
                            {
                                Version = OperatingSystemVersion.WindowsEmbedded8Standard
                            }
                    };
                }
            }

            /// <summary>
            /// Gets the descriptor for InfoVision Topbox PC-2 I3.
            /// </summary>
            public static HardwareDescriptor TopboxPC2I3
            {
                get
                {
                    var descriptor = TopboxPC2Atom;
                    descriptor.Name = "InfoVision Topbox PC-2 I3";
                    return descriptor;
                }
            }

            /// <summary>
            /// Gets the descriptor for InfoVision Topbox Mini.
            /// </summary>
            public static HardwareDescriptor TopboxMini
            {
                get
                {
                    return new HardwareDescriptor
                    {
                        Name = "InfoVision Topbox Mini",
                        Platform =
                            new InfoVisionPlatformDescriptor
                            {
                                DisplayAdapters =
                                                   {
                                                       new DisplayAdapterDescriptor(1, DisplayConnectionType.Dvi)
                                                   },
                                HasSharedRs485Port = true,
                                HasGenericButton = false,
                                HasGenericLed = true,
                                Inputs =
                                                   {
                                                       new InputDescriptor(0, "IN0 (D-sub 15: pin 4)"),
                                                       new InputDescriptor(1, "IN1 (D-sub 15: pin 5)"),
                                                       new InputDescriptor(2, "IN2 (D-sub 15: pin 9)"),
                                                       new InputDescriptor(3, "IN3 (D-sub 15: pin 10)"),
                                                       new InputDescriptor(4, "IN4 (D-sub 15: pin 14)"),
                                                       new InputDescriptor(5, "IN5 (D-sub 15: pin 15)")
                                                   },
                                Outputs =
                                                   {
                                                       new OutputDescriptor(6, "OUT0 (D-sub 9: pin 4)"),
                                                       new OutputDescriptor(7, "OUT1 (D-sub 9: pin 5)")
                                                   },
                                SerialPorts =
                                                   {
                                                       new SerialPortDescriptor("COM3", true)
                                                   }
                            },
                        OperatingSystem =
                            new WindowsEmbeddedDescriptor
                            {
                                Version = OperatingSystemVersion.WindowsEmbedded8Standard
                            }
                    };
                }
            }

            /// <summary>
            /// Gets the descriptor for InfoVision TFT 18.5" Compact.
            /// </summary>
            public static HardwareDescriptor Compact185
            {
                get
                {
                    var descriptor = TopboxMini;
                    descriptor.Name = "InfoVision TFT 18.5\" Compact";
                    var platform = (InfoVisionPlatformDescriptor)descriptor.Platform;
                    platform.DisplayAdapters.Insert(0, new DisplayAdapterDescriptor(0, DisplayConnectionType.Lvds));
                    platform.BuiltInScreen = new DisplayDescriptor(1368, 768);
                    return descriptor;
                }
            }

            /// <summary>
            /// Gets the descriptor for InfoVision TFT 29" Compact.
            /// </summary>
            public static HardwareDescriptor Compact29
            {
                get
                {
                    var descriptor = Compact185;
                    descriptor.Name = "InfoVision TFT 29\" Compact";
                    var platform = (InfoVisionPlatformDescriptor)descriptor.Platform;
                    platform.BuiltInScreen = new DisplayDescriptor(1920, 630, 1920, 1080);
                    return descriptor;
                }
            }

            /// <summary>
            /// Gets the descriptor for InfoVision Quadro TFT 18.5" Compact.
            /// </summary>
            public static HardwareDescriptor Compact185Quadro
            {
                get
                {
                    var descriptor = Compact185;
                    descriptor.Name = "InfoVision Quadro TFT 18.5\" Compact";
                    var platform = (InfoVisionPlatformDescriptor)descriptor.Platform;
                    platform.Inputs.Clear();
                    platform.Inputs.Add(new InputDescriptor(0, "IN0 (D-sub 13W3: pins 1/6)"));
                    platform.Inputs.Add(new InputDescriptor(1, "IN1 (D-sub 13W3: pins 2/7)"));
                    platform.Inputs.Add(new InputDescriptor(2, "IN2 (D-sub 13W3: pins 3/8)"));
                    platform.Inputs.Add(new InputDescriptor(3, "IN3 (D-sub 13W3: pins 4/9)"));
                    platform.Inputs.Add(new InputDescriptor(4, "IN4 (D-sub 13W3: pins 5/10)"));
                    return descriptor;
                }
            }
        }

        /// <summary>
        /// The Power Unit product line (E-Paper).
        /// </summary>
        public static class PowerUnit
        {
            /// <summary>
            /// Gets the descriptor for Power Unit with solar power and 1 display unit.
            /// </summary>
            public static HardwareDescriptor PowerUnitSolar1
            {
                get
                {
                    return new HardwareDescriptor
                                         {
                                             Name = "PU.solar-1",
                                             Platform =
                                                 new PowerUnitPlatformDescriptor
                                                     {
                                                         DisplayUnits = { new DisplayUnitDescriptor(0, true) },
                                                         PowerType = PowerType.Solar,

                                                         // ToDo: Add correct values for inputs, outputs,...
                                                         Inputs =
                                                             {
                                                                 new InputDescriptor(0, "IN0 (D-sub 15: pin 4)")
                                                             },
                                                         Outputs =
                                                             {
                                                                 new OutputDescriptor(6, "OUT0 (D-sub 9: pin 4)")
                                                             }
                                                     },
                                             OperatingSystem =
                                                 new MicroControllerDescriptor
                                                     {
                                                         Version = OperatingSystemVersion.MicroController
                                                     }
                                         };
                }
            }

            /// <summary>
            /// Gets the descriptor for Power Unit with solar power and 2 display units.
            /// </summary>
            public static HardwareDescriptor PowerUnitSolar2
            {
                get
                {
                    var descriptor = PowerUnitSolar1;
                    descriptor.Name = "PU.solar-2";
                    ((PowerUnitPlatformDescriptor)descriptor.Platform).DisplayUnits.Add(
                        new DisplayUnitDescriptor(1, true));
                    return descriptor;
                }
            }

            /// <summary>
            /// Gets the descriptor for Power Unit with line power at night and 1 display unit.
            /// </summary>
            public static HardwareDescriptor PowerUnitNight1
            {
                get
                {
                    var descriptor = PowerUnitSolar1;
                    descriptor.Name = "PU.night-1";
                    ((PowerUnitPlatformDescriptor)descriptor.Platform).PowerType = PowerType.Line;
                    return descriptor;
                }
            }

            /// <summary>
            /// Gets the descriptor for Power Unit with line power at night and 2 display units.
            /// </summary>
            public static HardwareDescriptor PowerUnitNight2
            {
                get
                {
                    var descriptor = PowerUnitNight1;
                    descriptor.Name = "PU.night-2";
                    ((PowerUnitPlatformDescriptor)descriptor.Platform).DisplayUnits.Add(
                        new DisplayUnitDescriptor(1, true));
                    return descriptor;
                }
            }

            /// <summary>
            /// The get display unit count.
            /// </summary>
            /// <param name="descriptorName">
            /// The descriptor name.
            /// </param>
            /// <returns>
            /// The amount of display units.
            /// </returns>
            public static int GetDisplayUnitCount(string descriptorName)
            {
                if (PowerUnitNight1.Name.Equals(descriptorName))
                {
                    return 1;
                }

                if (PowerUnitNight2.Name.Equals(descriptorName))
                {
                    return 2;
                }

                if (PowerUnitSolar1.Name.Equals(descriptorName))
                {
                    return 1;
                }

                if (PowerUnitSolar2.Name.Equals(descriptorName))
                {
                    return 2;
                }

                return 0;
            }
        }

        // ReSharper restore InconsistentNaming
    }
}
