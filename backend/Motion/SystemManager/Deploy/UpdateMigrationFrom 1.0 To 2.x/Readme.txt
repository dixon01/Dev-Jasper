Update migration from version 1.0 or 1.2 to Basic system 2.2

Steps to perform the migration:
===============================
1. Make a backup of the customer specific datas before you are starting.
2. Do the steps in Steps to create ImageUpdate directory.
		  ======================================
2. Copy the folder ImageUpdate to the root of the USB stick
3. Shutdown the ATOM Topbox to be migrated.
4. Plug in the USB stick and restart the ATOM Topbox
5. The ATOM Topbox will perform an update and at the system will restart twice.
6. After the second restarts, The USB stick can be removed. 
   The system will start with SystemManagerShell.exe starting. SystemManagerShell.exe starts
   the rest of the Basic System of Update and HardwareManager applications.
6. A file "MigratedSystems.txt" is available at the end of the process on the root of the USB stick. 
   It contains the updated name of the TFT system that was just migrated, based on the HardwareManager configuration.

Important:
==========
All customer datas will be deleted. 
If the migration is attempted on a PM600, a message prompts saying “Install not intended for this hardware! Please remove the USB stick.” 
And no migration is performed.

Topbox PM 600 wil be not supported anymore,due to less processing power. 



Steps to create ImageUpdate directory:
======================================
1. Create a directory called ImageUpdate
2. Create a folder called Progs under ImageUpdate. The Progs directory shall have the folders SystemManager, Update and HardwareManager under it.
3. Create a folder called Config under ImageUpdate. The Progs directory shall have the folders SystemManager, Update and HardwareManager under it.
4. Place the all the files in "Batch and Registry Files" directly in the root of the diretcory ImageUpdate.
5. Place the released binaries of the applications SystemManager, Update and HardwareManager in the respective folders under Progs.
6. Place the configuration files of SystemManager, Update and HardwareManager suitable for Inform ATOM.