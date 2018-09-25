// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WinMmVolumeWrapper.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WinMmVolumeWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Common
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <see cref="VolumeWrapperBase"/> implementation that uses <code>winmm.dll</code>.
    /// </summary>
    internal partial class WinMmVolumeWrapper : VolumeWrapperBase
    {
        /// <summary>
        /// Gets the current overall system volume.
        /// </summary>
        /// <returns>
        /// The volume between 0 and 100.
        /// </returns>
        public override int GetVolume()
        {
            int mixer;
            NativeMethods.MIXERCONTROL volCtrl;
            int currentVol;
            NativeMethods.mixerOpen(out mixer, 0, 0, 0, 0);
            GetVolumeControl(
                mixer,
                NativeMethods.MIXERLINE_COMPONENTTYPE_DST_SPEAKERS,
                NativeMethods.MIXERCONTROL_CONTROLTYPE_VOLUME,
                out volCtrl,
                out currentVol);
            NativeMethods.mixerClose(mixer);
            var volume = (currentVol * 100) / volCtrl.lMaximum;
            return volume;
        }

        /// <summary>
        /// Sets the current overall system volume.
        /// </summary>
        /// <param name="volume">
        /// The volume between 0 and 100.
        /// </param>
        public override void SetVolume(int volume)
        {
            int mixer;
            NativeMethods.MIXERCONTROL volCtrl;
            int currentVol;
            NativeMethods.mixerOpen(out mixer, 0, 0, 0, 0);
            GetVolumeControl(
                mixer,
                NativeMethods.MIXERLINE_COMPONENTTYPE_DST_SPEAKERS,
                NativeMethods.MIXERCONTROL_CONTROLTYPE_VOLUME,
                out volCtrl,
                out currentVol);

            int mappedVolume = volume * (volCtrl.lMaximum / 100);

            if (mappedVolume > volCtrl.lMaximum)
            {
                mappedVolume = volCtrl.lMaximum;
            }

            if (mappedVolume < volCtrl.lMinimum)
            {
                mappedVolume = volCtrl.lMinimum;
            }

            SetVolumeControl(mixer, volCtrl, mappedVolume);
            GetVolumeControl(
                mixer,
                NativeMethods.MIXERLINE_COMPONENTTYPE_DST_SPEAKERS,
                NativeMethods.MIXERCONTROL_CONTROLTYPE_VOLUME,
                out volCtrl,
                out currentVol);
            if (mappedVolume != currentVol)
            {
                throw new InvalidOperationException("Cannot Set Volume");
            }

            NativeMethods.mixerClose(mixer);
        }

        private static bool GetVolumeControl(
            int hmixer, int componentType, int ctrlType, out NativeMethods.MIXERCONTROL mxc, out int currentVol)
        {
            // This function attempts to obtain a mixer control.
            // Returns True if successful.
            var mxlc = new NativeMethods.MIXERLINECONTROLS();
            var mxl = new NativeMethods.MIXERLINE();
            var pmxcd = new NativeMethods.MIXERCONTROLDETAILS();
            mxc = new NativeMethods.MIXERCONTROL();
            currentVol = -1;
            mxl.cbStruct = Marshal.SizeOf(mxl);
            mxl.dwComponentType = componentType;
            int rc = NativeMethods.mixerGetLineInfoA(hmixer, ref mxl, NativeMethods.MIXER_GETLINEINFOF_COMPONENTTYPE);
            if (NativeMethods.MMSYSERR_NOERROR != rc)
            {
                return false;
            }

            const int SizeOfMixerControl = 152;
            ////int ctrl = Marshal.SizeOf(typeof(NativeMethods.MIXERCONTROL));
            mxlc.pamxctrl = Marshal.AllocCoTaskMem(SizeOfMixerControl);
            mxlc.cbStruct = Marshal.SizeOf(mxlc);
            mxlc.dwLineID = mxl.dwLineID;
            mxlc.dwControl = ctrlType;
            mxlc.cControls = 1;
            mxlc.cbmxctrl = SizeOfMixerControl;

            // Allocate a buffer for the control
            mxc.cbStruct = SizeOfMixerControl;

            // Get the control
            rc = NativeMethods.mixerGetLineControlsA(
                hmixer, ref mxlc, NativeMethods.MIXER_GETLINECONTROLSF_ONEBYTYPE);
            bool retValue;
            if (NativeMethods.MMSYSERR_NOERROR == rc)
            {
                retValue = true;

                // Copy the control into the destination structure
                mxc =
                    (NativeMethods.MIXERCONTROL)
                    Marshal.PtrToStructure(mxlc.pamxctrl, typeof(NativeMethods.MIXERCONTROL));
            }
            else
            {
                retValue = false;
            }

            pmxcd.cbStruct = Marshal.SizeOf(typeof(NativeMethods.MIXERCONTROLDETAILS));
            pmxcd.dwControlID = mxc.dwControlID;
            pmxcd.paDetails = Marshal.AllocCoTaskMem(
                Marshal.SizeOf(typeof(NativeMethods.MIXERCONTROLDETAILS_UNSIGNED)));
            pmxcd.cChannels = 1;
            pmxcd.item = 0;
            pmxcd.cbDetails = Marshal.SizeOf(typeof(NativeMethods.MIXERCONTROLDETAILS_UNSIGNED));
            rc = NativeMethods.mixerGetControlDetailsA(
                hmixer, ref pmxcd, NativeMethods.MIXER_GETCONTROLDETAILSF_VALUE);
            var du =
                (NativeMethods.MIXERCONTROLDETAILS_UNSIGNED)
                Marshal.PtrToStructure(pmxcd.paDetails, typeof(NativeMethods.MIXERCONTROLDETAILS_UNSIGNED));
            currentVol = du.dwValue;
            return retValue;
        }

        private static bool SetVolumeControl(int hmixer, NativeMethods.MIXERCONTROL mxc, int volume)
        {
            // This function sets the value for a volume control.
            // Returns True if successful
            var mxcd = new NativeMethods.MIXERCONTROLDETAILS();
            var vol = new NativeMethods.MIXERCONTROLDETAILS_UNSIGNED();
            mxcd.item = 0;
            mxcd.dwControlID = mxc.dwControlID;
            mxcd.cbStruct = Marshal.SizeOf(mxcd);
            mxcd.cbDetails = Marshal.SizeOf(vol);

            // Allocate a buffer for the control value buffer
            mxcd.cChannels = 1;
            vol.dwValue = volume;

            // Copy the data into the control value buffer
            mxcd.paDetails = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(NativeMethods.MIXERCONTROLDETAILS_UNSIGNED)));
            Marshal.StructureToPtr(vol, mxcd.paDetails, false);

            // Set the control value
            int rc = NativeMethods.mixerSetControlDetails(
                hmixer, ref mxcd, NativeMethods.MIXER_SETCONTROLDETAILSF_VALUE);

            return NativeMethods.MMSYSERR_NOERROR == rc;
        }

        private static class NativeMethods
        {
#pragma warning disable 169,649

            // ReSharper disable InconsistentNaming
            // ReSharper disable UnusedMember.Local
            // ReSharper disable UnassignedField.Local
            // ReSharper disable NotAccessedField.Local
            // ReSharper disable MemberCanBePrivate.Local
            public const int MMSYSERR_NOERROR = 0;
            public const int MAXPNAMELEN = 32;
            public const int MIXER_LONG_NAME_CHARS = 64;
            public const int MIXER_SHORT_NAME_CHARS = 16;

            public const int MIXER_GETLINEINFOF_COMPONENTTYPE = 0x3;
            public const int MIXER_GETCONTROLDETAILSF_VALUE = 0x0;
            public const int MIXER_GETLINECONTROLSF_ONEBYTYPE = 0x2;
            public const int MIXER_SETCONTROLDETAILSF_VALUE = 0x0;
            public const int MIXERLINE_COMPONENTTYPE_DST_FIRST = 0x0;
            public const int MIXERLINE_COMPONENTTYPE_SRC_FIRST = 0x1000;

            public const int MIXERLINE_COMPONENTTYPE_DST_SPEAKERS = MIXERLINE_COMPONENTTYPE_DST_FIRST + 4;
            public const int MIXERLINE_COMPONENTTYPE_SRC_MICROPHONE = MIXERLINE_COMPONENTTYPE_SRC_FIRST + 3;
            public const int MIXERLINE_COMPONENTTYPE_SRC_LINE = MIXERLINE_COMPONENTTYPE_SRC_FIRST + 2;
            public const int MIXERCONTROL_CT_CLASS_FADER = 0x50000000;
            public const int MIXERCONTROL_CT_UNITS_UNSIGNED = 0x30000;
            public const int MIXERCONTROL_CONTROLTYPE_FADER =
                MIXERCONTROL_CT_CLASS_FADER | MIXERCONTROL_CT_UNITS_UNSIGNED;

            public const int MIXERCONTROL_CONTROLTYPE_VOLUME = MIXERCONTROL_CONTROLTYPE_FADER + 1;

            [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
            public static extern int mixerClose(int hmx);

            [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
            public static extern int mixerGetControlDetailsA(
                int hmxobj, ref MIXERCONTROLDETAILS pmxcd, int fdwDetails);

            [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
            public static extern int mixerGetDevCapsA(int uMxId, MIXERCAPS pmxcaps, int cbmxcaps);

            [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
            public static extern int mixerGetID(int hmxobj, int pumxID, int fdwId);

            [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
            public static extern int mixerGetLineControlsA(int hmxobj, ref MIXERLINECONTROLS pmxlc, int fdwControls);

            [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
            public static extern int mixerGetLineInfoA(int hmxobj, ref MIXERLINE pmxl, int fdwInfo);

            [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
            public static extern int mixerGetNumDevs();

            [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
            public static extern int mixerMessage(int hmx, int uMsg, int dwParam1, int dwParam2);

            [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
            public static extern int mixerOpen(out int phmx, int uMxId, int dwCallback, int dwInstance, int fdwOpen);

            [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
            public static extern int mixerSetControlDetails(int hmxobj, ref MIXERCONTROLDETAILS pmxcd, int fdwDetails);

            public struct MIXERCAPS
            {
                public int wMid;
                public int wPid;
                public int vDriverVersion;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
                public string szPname;
                public int fdwSupport;
                public int cDestinations;
            }

            public struct MIXERCONTROL
            {
                public int cbStruct;
                public int dwControlID;
                public int dwControlType;
                public int fdwControl;
                public int cMultipleItems;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_SHORT_NAME_CHARS)]
                public string szShortName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_LONG_NAME_CHARS)]
                public string szName;
                public int lMinimum;
                public int lMaximum;
                [MarshalAs(UnmanagedType.U4, SizeConst = 10)]
                public int reserved;
            }

            public struct MIXERCONTROLDETAILS
            {
                public int cbStruct;
                public int dwControlID;
                public int cChannels;
                public int item;
                public int cbDetails;
                public IntPtr paDetails;
            }

            public struct MIXERCONTROLDETAILS_UNSIGNED
            {
                public int dwValue;
            }

            public struct MIXERLINE
            {
                public int cbStruct;
                public int dwDestination;
                public int dwSource;
                public int dwLineID;
                public int fdwLine;
                public int dwUser;
                public int dwComponentType;
                public int cChannels;
                public int cConnections;
                public int cControls;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_SHORT_NAME_CHARS)]
                public string szShortName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_LONG_NAME_CHARS)]
                public string szName;
                public int dwType;
                public int dwDeviceID;
                public int wMid;
                public int wPid;
                public int vDriverVersion;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
                public string szPname;
            }

            public struct MIXERLINECONTROLS
            {
                public int cbStruct;
                public int dwLineID;
                public int dwControl;
                public int cControls;
                public int cbmxctrl;
                public IntPtr pamxctrl;
            }

            // ReSharper restore MemberCanBePrivate.Local
            // ReSharper restore NotAccessedField.Local
            // ReSharper restore UnassignedField.Local
            // ReSharper restore UnusedMember.Local
            // ReSharper restore InconsistentNaming
#pragma warning restore 169, 649
        }
    }
}