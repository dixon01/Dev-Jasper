Rem
Rem ***************************************************************************
Rem  ... Copyright (c) 2016 Luminator
Rem  ... All rights reserved.
Rem  ... Any use without the prior written consent of Luminator
Rem      is strictly prohibited.
Rem ***************************************************************************
Rem ***************************************************************************
Rem
Rem  Filename:       Debug.vbs
Rem
Rem  Description:    This script performs the debug function at startup.
Rem                  If the user presses and holds the "d" key, as the
Rem                  system starts, this script is executed.
Rem
Rem  Revision History:
Rem  Date        Name            Ver     Remarks
Rem  12/13/2016  Vincent Larsen  1       Original
Rem
Rem  Notes:
Rem
Rem ***************************************************************************

Set oWS = CreateObject ("WScript.Shell")  

oWS.Run "explorer D:\"
oWS.Run "cmd /K D: && cd \"