// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DevMode.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The information about the graphics mode of a display device.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Structs
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The information about the graphics mode of a display device.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    public partial struct DevMode
    {
        /// <summary>
        /// The friendly name of the display device.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        [FieldOffset(0)]
        public string DeviceName;

        /// <summary>
        /// The version number of the initialization data specification on which the structure is based.
        /// </summary>
        [FieldOffset(32)]
        public short SpecVersion;

        /// <summary>
        /// The driver version number assigned by the driver developer.
        /// </summary>
        [FieldOffset(34)]
        public short DriverVersion;

        /// <summary>
        /// Specifies the size, in bytes, of the DEVMODE structure, not including any private driver-specific data
        /// that might follow the structure's public members
        /// </summary>
        [FieldOffset(36)]
        public short Size;

        /// <summary>
        /// Contains the number of bytes of private driver-data that follow this structure.
        /// </summary>
        [FieldOffset(38)]
        public short DriverExtra;

        /// <summary>
        /// Specifies whether certain members of the DEVMODE structure have been initialized.
        /// </summary>
        [FieldOffset(40)]
        public int Fields;

        /// <summary>
        /// For printer devices only, selects the orientation of the paper.
        /// </summary>
        [FieldOffset(44)]
        public short Orientation;

        /// <summary>
        /// For printer devices only, selects the size of the paper to print on.
        /// </summary>
        [FieldOffset(46)]
        public short PaperSize;

        /// <summary>
        /// For printer devices only, overrides the length of the paper.
        /// </summary>
        [FieldOffset(48)]
        public short PaperLength;

        /// <summary>
        /// For printer devices only, overrides the width of the paper.
        /// </summary>
        [FieldOffset(50)]
        public short PaperWidth;

        /// <summary>
        /// Specifies the factor by which the printed output is to be scaled..
        /// </summary>
        [FieldOffset(52)]
        public short Scale;

        /// <summary>
        /// Selects the number of copies printed if the device supports multiple-page copies.
        /// </summary>
        [FieldOffset(54)]
        public short Copies;

        /// <summary>
        /// Specifies the paper source.
        /// </summary>
        [FieldOffset(56)]
        public short DefaultSource;

        /// <summary>
        /// Specifies the printer resolution.
        /// </summary>
        [FieldOffset(58)]
        public short PrintQuality;

        /// <summary>
        /// For display devices only, a POINTL structure that indicates the positional
        /// coordinates of the display device in reference to the desktop area. The primary
        /// display device is always located at coordinates (0,0).
        /// </summary>
        [FieldOffset(44)]
        public PointL Position;

        /// <summary>
        /// For display devices only, the orientation at which images should be presented.
        /// </summary>
        [FieldOffset(52)]
        public int DisplayOrientation;

        /// <summary>
        /// For fixed-resolution display devices only, how the display presents a low-resolution mode on a
        /// higher-resolution display.
        /// </summary>
        [FieldOffset(56)]
        public int DisplayFixedOutput;

        /// <summary>
        /// Switches between color and monochrome on color printers.
        /// </summary>
        [FieldOffset(60)]
        public short Color;

        /// <summary>
        /// Selects duplex or double-sided printing for printers capable of duplex printing.
        /// </summary>
        [FieldOffset(62)]
        public short Duplex;

        /// <summary>
        /// Specifies the y-resolution, in dots per inch, of the printer.
        /// </summary>
        [FieldOffset(64)]
        public short YResolution;

        /// <summary>
        /// Specifies how TrueType fonts should be printed.
        /// </summary>
        [FieldOffset(66)]
        public short TTOption;

        /// <summary>
        /// Specifies whether collation should be used when printing multiple copies.
        /// </summary>
        [FieldOffset(68)]
        public short Collate;

        /// <summary>
        /// A zero-terminated character array that specifies the name of the form to use.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        [FieldOffset(72)]
        public string FormName;

        /// <summary>
        /// The number of pixels per logical inch.
        /// </summary>
        [FieldOffset(102)]
        public short LogPixels;

        /// <summary>
        /// Specifies the color resolution, in bits per pixel, of the display device.
        /// </summary>
        [FieldOffset(104)]
        public short BitsPerPel;

        /// <summary>
        /// Specifies the width, in pixels, of the visible device surface.
        /// </summary>
        [FieldOffset(108)]
        public int PelsWidth;

        /// <summary>
        /// Specifies the height, in pixels, of the visible device surface.
        /// </summary>
        [FieldOffset(112)]
        public int PelsHeight;

        /// <summary>
        /// Specifies the device's display mode.
        /// </summary>
        [FieldOffset(116)]
        public int DisplayFlags;

        /// <summary>
        /// Specifies where the NUP is done.
        /// </summary>
        [FieldOffset(116)]
        public int Nup;

        /// <summary>
        /// Specifies the frequency, in hertz (cycles per second), of the display device in a particular mode.
        /// </summary>
        [FieldOffset(120)]
        public int DisplayFrequency;
    }
}
