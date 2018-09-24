var repositoryDirectory = Directory("../..");

#l "../../headers.cake"
#l "../../tasks.cake"

var sourceDirectoryString = string.IsNullOrEmpty(sourcePath) ? "./Source/ConsoleApp/bin/" + configuration : sourcePath;
sourceDirectory = Directory(sourceDirectoryString);

var solutionPaths = new[] { File("./Gorba.Motion.HardwareManager.sln") };
manifestId = "Gorba.Motion.HardwareManager";
progsName = "HardwareManager";

linuxSourceDirectories.Add(
    repositoryDirectory + Directory("./Motion/HardwareManager/Source/LinuxIO/bin/") + configurationDirectory);
linuxSourceDirectories.Add(
    repositoryDirectory + Directory("./Motion/HardwareManager/Source/Cu/bin/") + configurationDirectory);
linuxSourceDirectories.Add(
    repositoryDirectory + Directory("./Motion/HardwareManager/Source/Os/bin/") + configurationDirectory);
linuxSourceDirectories.Add(
    repositoryDirectory + Directory("./Motion/HardwareManager/Source/LinuxTft/bin/") + configurationDirectory);	
renameItems.Add(new RenameItem("Gorba.Motion.HardwareManager.Os.Posix.dll", "Gorba.Motion.HardwareManager.Os.dll"));
if (!excludePublishSymbols)
{
    renameItems.Add(new RenameItem("Gorba.Motion.HardwareManager.Os.Posix.pdb", "Gorba.Motion.HardwareManager.Os.pdb"));
}

RunTarget(target);