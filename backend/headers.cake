var target = Argument("target", "Default");
var buildVerbosity = Argument("buildVerbosity", Verbosity.Minimal);
var productVersion = Argument("productVersion", "1.0");
var buildId = Argument("buildId", string.Empty);
var configuration = Argument("configuration", "Debug");
var packagesTag = Argument("packagesTag", "dev");
var packagesToken = Argument("packagesToken", string.Empty);
var prefix = Argument("prefix", string.Empty);
var outputPath = Argument("outputPath", string.Empty);
var linuxOutputPath = Argument("linuxOutputPath", string.Empty);
var applicationName = Argument("applicationName", string.Empty);
var progsName = applicationName;

/*
* CenterCli
*/
var centerCliPassword = Argument("centerCliPassword", string.Empty);
var centerCliLegacyServices = Argument("centerCliLegacyServices", new Uri("net.tcp://localhost:808/"));
var centerCliExePath = Argument("centerCliExePath", "CenterCliExePath");
var centerCliExeFile = File(centerCliExePath);

var sourcePath = Argument("sourcePath", string.Empty);
var manifestId = Argument("manifestId", string.Empty);
PlatformTarget? platform = PlatformTarget.MSIL;

var configurationDirectory = Directory(configuration);

/*
* Publish and deploy
*/
var publishPath = Argument("publishPath", string.Empty);
var publishDirectory = string.IsNullOrEmpty(publishPath) ?
    repositoryDirectory + Directory("./publish") : Directory(publishPath);
var deployPath = Argument("deployPath", string.Empty);
var deployDirectory = string.IsNullOrEmpty(deployPath) ?
    repositoryDirectory + Directory("./deploy") : Directory(deployPath);
var excludePublishSymbols = Argument("excludePublishSymbols", false);
var excludeDeploySymbols = Argument("excludeDeploySymbols", false);
var includes = excludePublishSymbols ? new [] { "**/*.dll", "**/*.exe" } : new [] { "**/*.dll", "**/*.exe", "**/*.pdb" };
var publishOptions = new PublishOptions
{
    Includes = includes
};

var excludes = excludeDeploySymbols ? new [] { "**/*.pdb" } : new string[0];
var deployOptions = new PublishOptions
{
    Includes = new [] { "**/*" },
    Excludes = excludes
};

/*
* Linux specific
*/

var linuxPublishPath = Argument("linuxPublishPath", string.Empty);
var linuxPublishDirectory = string.IsNullOrEmpty(linuxPublishPath) ?
    repositoryDirectory + Directory("./publish-linux") : Directory(linuxPublishPath);

var linuxSourceDirectories = new List<DirectoryPath>
{
    repositoryDirectory + Directory("./Common/Utility/Source/OSWrappers/bin/") + configurationDirectory
};
var linuxIncludes =
    excludePublishSymbols
        ? new [] { "**/*.posix.dll", "**/*linux*.dll", "**/*.Cu.dll" }
        : new [] { "**/*.posix.dll", "**/*.posix.pdb", "**/*linux*.dll", "**/*linux*.pdb", "**/*.Cu.dll", "**/*.Cu.pdb" };
var renameItems = new List<RenameItem>
{
    new RenameItem("Gorba.Common.Utility.OSWrappers.POSIX.dll", "Gorba.Common.Utility.OSWrappers.dll")
};
if (!excludePublishSymbols)
{
    renameItems.Add(new RenameItem("Gorba.Common.Utility.OSWrappers.POSIX.pdb", "Gorba.Common.Utility.OSWrappers.pdb"));
}

var linuxPublishOptions = new PublishOptions
{
    Includes = linuxIncludes
};

Information("Registering file variables");

var solutionAssemblyProductInfoFile = File("./Source/SolutionAssemblyProductInfo.cs");
var applicationVersionReferenceFileSearchPattern = Argument("applicationVersionReferenceFileSearchPattern", string.Empty);


var nugetConfigFile = File(repositoryDirectory.Path.FullPath + "/NuGet.config");
var packagesDirectory = repositoryDirectory + Directory("3rdParty/packages");

var applicationVersion = GetApplicationVersion(productVersion, buildId, applicationVersionReferenceFileSearchPattern);
var packageVersion = GetPackageVersion(applicationVersion, packagesTag);
var nugetOutput = repositoryDirectory + Directory("localPackages");

var productName = string.Empty;
FilePath clickOncePath = null;

FilePath solutionPath;
FilePath serviceFabricProject = null;
DirectoryPath sourceDirectory = null;
var toolPaths = new ConvertableFilePath[0];
var clickOnceName = string.Empty;

var templateProjects = new ConvertableFilePath[0];

var nugetProjects = new FilePath[0];
var nugetPackages = new string[0];

Information("Searching bin directories");
var binDirectories = GetDirectories(repositoryDirectory.Path.FullPath + "/**/Source/**/bin/" + configuration)
    .Select(p => p.Combine("..")).ToList();
WriteInformation("Found " + binDirectories.Count + " directory/ies");
var testDirectories = GetDirectories("./Source/*Tests");
WriteInformation("Found " + testDirectories.Count + " test directory/ies");