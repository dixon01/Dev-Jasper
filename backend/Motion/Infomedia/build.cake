var repositoryDirectory = Directory("../..");

#l "../../headers.cake"
#l "../../tasks.cake"

string sourceDirectoryString;
var isDeploy = string.Equals(target, "Deploy", StringComparison.OrdinalIgnoreCase);
if (isDeploy && string.IsNullOrEmpty(sourcePath))
{
    Debug("Deploying Infomedia. Detecting source directory according to manifestId '" + manifestId + "'");
    switch (manifestId)
    {
        case "Gorba.Motion.Infomedia.Composer":
            sourceDirectoryString = "./Source/ComposerApp/bin/" + configuration;
            progsName = "Composer";
            break;
        case "Gorba.Motion.Infomedia.AudioRenderer":
            sourceDirectoryString = "./Source/AudioRendererApp/bin/" + configuration;
            progsName = "AudioRenderer";
            break;
        case "Gorba.Motion.Infomedia.DirectXRenderer":
            sourceDirectoryString = "./Source/DirectXRendererApp/bin/" + configuration;
            progsName = "DirectXRenderer";
            break;
        case "Gorba.Motion.Infomedia.MatrixRenderer":
            sourceDirectoryString = "./Source/MatrixRendererApp/bin/" + configuration;
            progsName = "MatrixRenderer";
            break;
        case "Linux.Motion.Infomedia.HtmlRenderer":
            sourceDirectoryString = "./Source/HtmlRendererApp/bin/" + configuration;
            progsName = "HtmlRenderer";
            break;
        default:
            throw new ArgumentOutOfRangeException("manifestId");
    }
}
else
{
    Debug("Running Infomedia with target '" + target + "'. Source directory path: '" + sourcePath + "'");
    sourceDirectoryString =
        string.IsNullOrEmpty(sourcePath) ? "./Source/ComposerApp/bin/" + configuration : sourcePath;
}

Information("Running Infomedia source directory path: '" + sourcePath + "', progs name: '" + progsName + "'");
sourceDirectory = Directory(sourceDirectoryString);

var solutionPaths = new[] { File("./Gorba.Motion.Infomedia.sln") };

RunTarget(target);