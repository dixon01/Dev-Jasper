#tool "nuget:?package=Mono.TextTransform"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var repositoryPath = Directory("../..");
var packagesPath = repositoryPath + Directory("3rdParty/packages");
Information("Using configuration " + configuration);
var platform = PlatformTarget.MSIL;

var solutionPath = File("./Gorba.Motion.Obc.sln");
Task("Clean")
    .Does(() =>{
        // cleaning up all bin directories in the repo
        var binDirectories = GetDirectories(repositoryPath.ToString() + "/**/Source/**/bin");
        foreach (var binDirectory in binDirectories)
        {
            CleanDirectory(binDirectory);
        }

        CleanDirectory(packagesPath);
});
Task("CleanTestResults")
    .Does(() =>{
        // cleaning up all test result files under current directory
        var traceFiles = GetFiles("./**/*.trx");
        foreach (var traceFile in traceFiles)
        {
            DeleteFile(traceFile);
        }
});
Task("Restore")
    .Does(() =>{
        NuGetRestore(solutionPath);
});
Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>{
        var settings = new MSBuildSettings {
            Verbosity = Verbosity.Normal,
            ToolVersion = MSBuildToolVersion.VS2015,
            Configuration = configuration,
            PlatformTarget = platform
            };
        MSBuild(solutionPath, settings);
});

Task("TransformText")
    .Does(() =>{
        var visualStudioProjectDirectory = Directory("../../Common/VisualStudio");
        var visualStudioSolution = File(visualStudioProjectDirectory.ToString() + "./Gorba.Common.VisualStudio.sln");
        var visualStudioOutput = visualStudioProjectDirectory + Directory("./Source/T4Directives/bin/" + configuration);
        NuGetRestore(visualStudioSolution);
        MSBuild(visualStudioSolution, s => s.SetConfiguration(configuration));
        var files = GetFiles(@"./Source/**/*.tt");
        var assembly = visualStudioOutput.ToString() + "./Gorba.Common.VisualStudio.T4Directives.dll";
        var assemblyName = "Gorba.Common.VisualStudio.T4Directives";
        var settings = new TextTransformSettings();
        foreach (var file in files)
        {
            Information("Transforming file " + file);
            settings.ArgumentCustomization = args => {
                                            args.Clear();
                                            return args
                                                .Append("-P")
                                                .Append(visualStudioOutput)
                                                .Append("-r")
                                                .Append("System.Core.dll")
                                                .Append("-r")
                                                .Append(assembly.ToString())
                                                .Append("-dp")
                                                .Append(@"CompileDirectiveProcessor!Gorba.Common.VisualStudio.T4Directives.Compiler.CompileDirectiveProcessor!" + assembly)
                                                .Append("-dp")
                                                .Append(@"ImportProjectDirectiveProcessor!Gorba.Common.VisualStudio.T4Directives.ImportProject.ImportProjectDirectiveProcessor!" + assembly)
                                                .Append(file.ToString());
                                            };
            TransformTemplate(file, settings);
        }
    });

Task("Rebuild")
    .IsDependentOn("Clean")
    .IsDependentOn("TransformText")
    .IsDependentOn("Build");

Task("RunTests")
    .IsDependentOn("Rebuild")
    .Does(() =>{
        var directories = GetDirectories("./Source/**/*.Tests");
        foreach (var directory in directories)
        {
            Information("Tests for directory " + directory);
            var settings = new VSTestSettings {
                WorkingDirectory = directory.ToString()
            };
            settings.WithVisualStudioLogger();
            Information("Tests for directory name " + directory.GetDirectoryName());
            var assemblies = GetFiles(directory.ToString() + "/bin/" + configuration + "/*" + directory.GetDirectoryName() + ".dll");
            VSTest(assemblies, settings);
        }
    });

Task("Vsts")
    .IsDependentOn("CleanTestResults")
    .IsDependentOn("RunTests")
    .Does(() => {
        Information("Run VSTS");
    });

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);