var repositoryDirectory = Directory("../..");

#l "../../headers.cake"
#l "../../tasks.cake"

var sourceDirectoryString = string.IsNullOrEmpty(sourcePath) ? "./Source/ConsoleApp/bin/" + configuration : sourcePath;
sourceDirectory = Directory(sourceDirectoryString);

var solutionPaths = new[] { File("./Gorba.Motion.Protran.sln") };
manifestId = "Gorba.Motion.Protran";
progsName = "Protran";

RunTarget(target);