var repositoryDirectory = Directory("../..");

#l "../../headers.cake"
#l "../../tasks.cake"

var sourceDirectoryString = string.IsNullOrEmpty(sourcePath) ? "./Source/ConsoleApp/bin/" + configuration : sourcePath;
sourceDirectory = Directory(sourceDirectoryString);

var solutionPaths = new[] { File("./Gorba.Motion.Update.sln") };
manifestId = "Gorba.Motion.Update";
progsName = "Update";

RunTarget(target);