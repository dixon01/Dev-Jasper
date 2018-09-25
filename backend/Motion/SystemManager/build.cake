var repositoryDirectory = Directory("../..");

#l "../../headers.cake"
#l "../../tasks.cake"

var sourceDirectoryString = string.IsNullOrEmpty(sourcePath) ? "./Source/ConsoleApp/bin/" + configuration : sourcePath;
sourceDirectory = Directory(sourceDirectoryString);

var solutionPaths = new[] { File("./Gorba.Motion.SystemManager.sln") };
manifestId = "Gorba.Motion.SystemManager";
progsName = "SystemManager";

RunTarget(target);