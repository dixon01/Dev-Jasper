var repositoryDirectory = Directory("../..");

#l "../../headers.cake"
#l "../../tasks.cake"

var solutionPaths = new[] { File("./Gorba.Motion.Common.sln") };

RunTarget(target);