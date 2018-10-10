Rem
Rem ***************************************************************************
Rem  ... Copyright (c) 2016 Luminator
Rem  ... All rights reserved.
Rem  ... Any use without the prior written consent of Luminator
Rem      is strictly prohibited.
Rem ***************************************************************************
Rem ***************************************************************************
Rem
Rem  Filename:       Startup.vbs
Rem
Rem  Description:    This is the first, of our code, to execute on a boot up
Rem                  of an INFOtransit display.  This code scans possible USB
Rem                  drives for a zip file with a specific name format and,
Rem                  if found, unzips it at the base of the D:\ drive.  If a
Rem                  Finalize.vbs script was left by the update, it is run,
Rem                  deleted and the system rebooted.  After any updates,
Rem                  main program, for INFOtransit is executed.
Rem
Rem  Revision History:
Rem  Date        Name            Ver     Remarks
Rem  10/28/2016  Vincent Larsen  1       Original
Rem
Rem  Notes:
Rem
Rem     This file goes in the D:\Support\OnBoot directory.
Rem
Rem ***************************************************************************

Boot     = "D:\Support\OnBoot"
Set oSA  = CreateObject ("Shell.Application")
Set oWS  = WScript.CreateObject ("WScript.Shell")
Set oFS  = CreateObject ("Scripting.FileSystemObject")

Rem
Rem If the "d" key is being pressed, while booting, do
Rem  not start the system normally.  Instead, run the
Rem  debug script.
Rem
debugScript = Boot&"\Debug.vbs"
If oFS.FileExists (debugScript) Then
    ret = oWS.Run (Boot&"\GoToDebug.exe", 0, true)
Else
    ret = 0
End If

If ret = 1 Then
    oWS.Run debugScript, 0, false
Else
    Rem
    Rem See if an update file can be found in the D:\ftproot directory.
    Rem  If one is found, uncompress it to the root directory of D:\.
    Rem
    For Each name In oFS.GetFolder ("D:\ftproot").Files
        If UseThisFile (Mid (name, 12)) Then
            ApplyUpdate name
        End If
    Next
    
    Rem
    Rem See if an update file can be found on any of the external drive.
    Rem  If one is found, uncompress it to the root directory of D:\.
    Rem
    For Drive = Asc ("E") To Asc ("Z")
        Letter = Chr (Drive)
    
        If oFS.DriveExists (Letter) Then
            For Each name In oFS.GetFolder (Letter&":\").Files
                If UseThisFile (Mid (name, 4)) Then
                    ApplyUpdate name
                    oFS.DeleteFile name, True
                End If
            Next
        End If
    Next

    Rem
    Rem If a verify script is present, run it.
    Rem
    Verify = Boot&"\Verify.vbs"
    If oFS.FileExists (Verify) Then
        oWS.Run Verify, 0, True
    End If

    Rem Start the system.
    Rem
    oWS.Exec "D:\Progs\SystemManager\SystemManagerShell.exe"
End If

Rem
Rem In order to be considered an update, the file name
Rem  must be in the format of ######-<anything>.zip.
Rem
Rem That is six digits, followed by a hyphen, creator
Rem  defined information, ending in ".zip."
Rem
Function UseThisFile (who)
    UseThisFile = False
    
    If Not UCase (Right (who, 4)) = ".ZIP" Then
        Exit Function
    End If
    
    If BadValue (Mid (who, 1, 1)) Then
        Exit Function
    End If
    
    If BadValue (Mid (who, 2, 1)) Then
        Exit Function
    End If
    
    If BadValue (Mid (who, 3, 1)) Then
        Exit Function
    End If
    
    If BadValue (Mid (who, 4, 1)) Then
        Exit Function
    End If
    
    If BadValue (Mid (who, 5, 1)) Then
        Exit Function
    End If
    
    If BadValue (Mid (who, 6, 1)) Then
        Exit Function
    End If
    
    If Not Mid (who, 7, 1) = "-" Then
        Exit Function
    End If
    
    UseThisFile = True
End Function

Rem
Rem Return whether the passed value is a numeric
Rem  digit.
Rem
Function BadValue (what)
    BadValue = (what < "0") Or (what > "9")
End Function

Rem
Rem Unzip the passed file name, to the root of the D:\
Rem  drive and reboot the system.
Rem
Sub ApplyUpdate (name)
    zip = name
    
    Set dst = oSA.NameSpace ("D:\")
    Set src = oSA.NameSpace (zip).Items
    
    dst.CopyHere src, 1024+512+256+16
    
    Rem
    Rem If the update left behind some finalize code,
    Rem  run the code now.  When finalize is finished,
    Rem  delete it and reboot.  No reboot happens, if
    Rem  there is no finalize.
    Rem
    Restart = False
    Finalize = "D:\Finalize.vbs"
    If oFS.FileExists (Finalize) Then
        Restart = True
        oWS.Run Finalize, 1, True
        oFS.DeleteFile Finalize, True
    End If
    
    If Restart Then
        Rem
        Rem Update complete.  Let the user know and reboot.
        Rem
        MsgBox "Please remove the USB flash drive and press OK to reboot.", vbOKOnly, "Update Complete!"

        oWS.Run "shutdown /r /t 1", 1, True
    End If
End Sub
