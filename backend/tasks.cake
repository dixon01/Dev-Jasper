#l "./helpers.cake"

EnsureDirectoryExists(nugetOutput);

WriteInformation("Using configuration " + configuration);

ConfigureNuGet(packagesToken);

Task("Clean")
    .Does(() =>{
        // cleaning up all bin directories in the repo
        WriteInformation("Cleaning " + binDirectories.Count + " directory/ies");
        foreach (var binDirectory in binDirectories)
        {
            CleanDirectory(binDirectory);
        }

        CleanDirectory(packagesDirectory);
});

Task("CleanPackages")
    .Does(() =>
        {
            if (!DirectoryExists(nugetOutput))
            {
                return;
            }

            CleanDirectory(nugetOutput);
        });

Task("CleanTestResults")
    .Does(() =>{
        // cleaning up all test result files under current directory
        var traceFiles = GetFiles(repositoryDirectory.Path.FullPath + "/**/*.trx");
        WriteInformation("Removing " + traceFiles.Count + " test results file(s)");
        foreach (var traceFile in traceFiles)
        {
            DeleteFile(traceFile);
        }
});

Task("Restore")
    .Does(() =>{
        foreach (var solutionPath in solutionPaths)
        {
            WriteInformation("Restoring solution " + solutionPath);
            NuGetRestore(solutionPath);
        }
});

Task("SetName")
    .Does(() => {
        if (clickOncePath == null)
        {
            Debug("ClickOnce path not defined");
            return;
        }

        Information(
            "Replacing Product and Assembly names with '" + prefix + productName
            + "' in file " + clickOncePath);
        var regexProductName = productName.Replace(".", @"\.");
        ReplaceRegexInFiles(
            clickOncePath.FullPath,
            @"<ProductName>.*" + regexProductName + "</ProductName>",
            "<ProductName>" + prefix + productName + "</ProductName>");
        ReplaceRegexInFiles(
            clickOncePath.FullPath,
            @"<AssemblyName>.*" + regexProductName + "</AssemblyName>",
            "<AssemblyName>" + prefix + productName +"</AssemblyName>");
});

Task("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("SetName")
    .Does(() =>{
        var settings = new MSBuildSettings {
            Verbosity = buildVerbosity,
            ToolVersion = MSBuildToolVersion.VS2017,
            Configuration = configuration,
            PlatformTarget = platform
            };
            foreach (var solutionPath in solutionPaths)
            {
                WriteInformation("Building solution '" + solutionPath + "' with verbosity " + buildVerbosity);
                MSBuild(solutionPath, settings);
            }
});

Task("BuildSF")
    .IsDependentOn("Restore")
    .Does(() =>{
        var settings = new MSBuildSettings {
            Verbosity = buildVerbosity,
            ToolVersion = MSBuildToolVersion.VS2017,
            Configuration = configuration,
            PlatformTarget = PlatformTarget.x64
            };
            settings.WithTarget("Package")
                .WithProperty("TargetProfile", "Cloud");
        MSBuild(serviceFabricProject, settings);
});

Task("TransformText")
    .Does(() =>{
        var visualStudioProjectDirectory = repositoryDirectory + Directory("Common/VisualStudio");
        var visualStudioSolution = File(visualStudioProjectDirectory.ToString() + "./Gorba.Common.VisualStudio.sln");
        var visualStudioOutput = visualStudioProjectDirectory + Directory("./Source/T4Directives/bin/" + configuration);
        NuGetRestore(visualStudioSolution);
        MSBuild(visualStudioSolution, s => s.SetConfiguration(configuration).SetVerbosity(Verbosity.Quiet));
        var files = GetFiles(@"./Source/**/*.tt");
        var assembly = visualStudioOutput.ToString() + "./Gorba.Common.VisualStudio.T4Directives.dll";
        var assemblyName = "Gorba.Common.VisualStudio.T4Directives";
        foreach (var templateProject in templateProjects)
        {
            MSBuild(templateProject, s => s.SetConfiguration(configuration).SetVerbosity(buildVerbosity));
        }

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
                                                .Append("Microsoft.CSharp.dll")
                                                .Append("-r")
                                                .Append(@"System.Xml.dll")
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

Task("Publish")
    .Description("Publishes the content required to run an application")
    .Does(() =>{
        EnsureDirectoryExists(publishDirectory);
        CleanDirectory(publishDirectory);
        Publish(sourceDirectory, publishDirectory, publishOptions);
    });

Task("PublishLinux")
    .Description("Publishes the content required to run a Linux application")
    .Does(() =>{
        Information("Linux specific directories: " + linuxSourceDirectories.Count);
        EnsureDirectoryExists(linuxPublishDirectory);
        CleanDirectory(linuxPublishDirectory);
        Publish(sourceDirectory, linuxPublishDirectory, publishOptions);
        if (!linuxSourceDirectories.Any())
        {
            Warning("No linux source directories");
            return;
        }

        Information("Completed standard publish. Publishing linux-specific items");
        foreach (var linuxSourceDirectory in linuxSourceDirectories.Take(linuxSourceDirectories.Count - 1))
        {
            Publish(linuxSourceDirectory, linuxPublishDirectory, linuxPublishOptions);
        }

        linuxPublishOptions.RenameItems = renameItems.ToArray();
        Publish(linuxSourceDirectories.Last(), linuxPublishDirectory, linuxPublishOptions);
    });

Task("DeploySoftwarePackage")
    .Description("Publishes the content required to run an application")
    .Does(() =>{
        Publish(publishDirectory, deployDirectory, deployOptions);
        UseCenterCliEnvironment(centerCliExeFile.Path.GetDirectory(), centerCliLegacyServices);
        UploadLegacyPackage(deployDirectory,
                            centerCliExeFile.Path,
                            centerCliPassword,
                            manifestId,
                            applicationVersion);
    });

Task("Deploy")
    .Description("Alias for legacy reasons. To be removed as soon as all builds are updated to support new tasks")
    .IsDependentOn("Publish")
    .IsDependentOn("DeploySoftwarePackage");

Task("Rebuild")
    .IsDependentOn("Clean")
    .IsDependentOn("TransformText")
    .IsDependentOn("Build");

Task("RunTests")
    .IsDependentOn("Rebuild")
    .Does(() =>{
        foreach (var directory in testDirectories)
        {
            WriteInformation("Tests for directory " + directory);
            var settings = new VSTestSettings {
                WorkingDirectory = directory.ToString()
            };
            settings.WithVisualStudioLogger();
            var assemblies =
                GetFiles(directory.ToString() + "/bin/" + configuration + "/*" + directory.GetDirectoryName() + ".dll");
            if (!assemblies.Any())
            {
                return;
            }

            VSTest(assemblies, settings);
        }
    });

Task("RunContinousTesting")
    .IsDependentOn("Build")
    .Does(() =>{
        foreach (var directory in testDirectories)
        {
            WriteInformation("Tests for directory " + directory);
            var settings = new VSTestSettings {
                WorkingDirectory = directory.ToString()
            };
            settings.WithVisualStudioLogger();
            var assemblies =
                GetFiles(directory.ToString() + "/bin/" + configuration + "/*" + directory.GetDirectoryName() + ".dll");
            if (!assemblies.Any())
            {
                return;
            }

            VSTest(assemblies, settings);
        }
    });

Task("Vsts")
    .IsDependentOn("CleanTestResults")
    .IsDependentOn("RunTests")
    .Does(() => {
        Information("Run VSTS");
    });

Task("VersionAssemblies")
    .Does(() =>
    {
        UseApplicationVersion(
            applicationVersion,
            repositoryDirectory.Path.FullPath,
            solutionAssemblyProductInfoFile.Path.MakeAbsolute(Context.Environment).FullPath);
    });

Task("ClickOnce")
    .IsDependentOn("Restore")
    .IsDependentOn("SetName")
    .Does(() =>{
        var settings = new MSBuildSettings {
            Verbosity = buildVerbosity,
            ToolVersion = MSBuildToolVersion.VS2017,
            Configuration = configuration,
            PlatformTarget = platform
            };

        settings.WithTarget("publish");
        settings.WithProperty("ApplicationVersion", applicationVersion);
        MSBuild(clickOncePath, settings);
        DeleteFiles("./Source/WpfApplication/bin/" + configuration + "/app.publish/*." + clickOnceName + ".exe");
});

Task("Tools")
    .Does(() =>{
        foreach (var toolPath in toolPaths)
        {
            NuGetRestore(toolPath);
            var settings = new MSBuildSettings {
                Verbosity = buildVerbosity,
                ToolVersion = MSBuildToolVersion.VS2017,
                Configuration = configuration,
                PlatformTarget = platform
                };
            MSBuild(toolPath, settings);
        }
});

Task("NuGetPack")
    .IsDependentOn("CleanPackages")
    .Does(() =>
    {
        EnsureDirectoryExists(nugetOutput.Path);
        var nuGetPackSettings   = new NuGetPackSettings {
                                     Id                      = "Gorba.Common.Formats",
                                     Version                 =  packageVersion,
                                     Title                   = "LTG Format models",
                                     Authors                 = new[] {"Gorba AG"},
                                     Owners                  = new[] { "LTG BaSS Europe" },
                                     Description             = "Models to work with different formats",
                                     Summary                 = "This package contains the model libraries required to work with different formats",
                                     ProjectUrl              = new Uri("https://www.gorba.com/"),
                                     //IconUrl                 = new Uri("http://"),
                                     LicenseUrl              = new Uri("https://www.gorba.com/"),
                                     Copyright               = "Gorba AG 2017",
                                     ReleaseNotes            = new [] { "Initial definition" },
                                     Tags                    = new [] { "Formats" },
                                        IncludeReferencedProjects = true,
                                     RequireLicenseAcceptance= false,
                                     Symbols                 = false,
                                     NoPackageAnalysis       = true,
                                     OutputDirectory         = nugetOutput.Path.FullPath
                                 };
        nuGetPackSettings.Properties = new Dictionary<string, string>{ { "Configuration", configuration } };
        Warning("Added configuration property " + configuration);
        NuGetPack(nugetProjects, nuGetPackSettings);
    });

Task("NuGetPush")
    .IsDependentOn("NuGetPack")
    .Does(() =>
    {
        EnsureDirectoryExists(nugetOutput.Path);
        foreach (var package in nugetPackages)
        {
            var packagePath = nugetOutput.Path.FullPath + "/" + package + "." + packageVersion + ".nupkg";
            NuGetPush(packagePath, new NuGetPushSettings {
                 Source = "https://lef.pkgs.visualstudio.com/_packaging/Ltg.Libraries/nuget/v3/index.json",
                 ApiKey = "VSTS"
             });
        }
    });

Task("Default")
    .IsDependentOn("Build");