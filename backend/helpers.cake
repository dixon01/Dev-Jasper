#addin nuget:?package=Cake.Json&version=1.0.2.13
#addin nuget:?package=Cake.VersionReader&version=2.0.0
#tool nuget:?package=Mono.TextTransform&version=1.0.0
#addin nuget:?package=Cake.FileHelpers&version=1.0.4

internal class PublishOptions
{
    public string[] Includes { get; set; }

    public string[] Excludes { get; set; }

    public RenameItem[] RenameItems { get; set; }
}

internal class RenameItem
{
    public RenameItem(string source, string destination)
    {
        this.Source = source;
        this.Destination = destination;
    }

    public RenameItem()
    {
    }

    public string Source { get; set; }

    public string Destination { get; set; }
}

void WriteInformation(string message)
{
    var lineSeparator = "--------------------------------------------------";
    Information(string.Empty);
    Information(lineSeparator);
    Information(message);
    Information(lineSeparator);
    Information(string.Empty);
}

void ConfigureNuGet(string token)
{
    var nugetSourceSettings = new NuGetSourcesSettings
                            {
                                IsSensitiveSource = true
                            };
    if (!string.IsNullOrEmpty(token))
    {
        nugetSourceSettings.UserName = "VSTS";
        nugetSourceSettings.Password = token;
        Information("Configured NuGet source to use token authentication");
    }

    var feed = new
                {
                    Name = "LTG.Libraries",
                    Source = "https://lef.pkgs.visualstudio.com/_packaging/LTG.Libraries/nuget/v3/index.json"
                };
    if (NuGetHasSource(feed.Source))
    {
        NuGetRemoveSource(
            name:feed.Name,
            source:feed.Source
        );
    }
            
    NuGetAddSource(
        name:feed.Name,
        source:feed.Source,
        settings:nugetSourceSettings
    );

    feed = new
                {
                    Name = "ClientLibraries",
                    Source = "https://lef.pkgs.visualstudio.com/_packaging/ClientLibraries/nuget/v3/index.json"
                };
    if (NuGetHasSource(feed.Source))
    {
        NuGetRemoveSource(
            name:feed.Name,
            source:feed.Source
        );
    }
            
    NuGetAddSource(
        name:feed.Name,
        source:feed.Source,
        settings:nugetSourceSettings
    );
}

string GetApplicationVersion(string productVersion, string buildId, string applicationVersionReferenceFileSearchPattern)
{
    Information("Getting application version");
    if (string.IsNullOrEmpty(applicationVersionReferenceFileSearchPattern))
    {
        // Original source: https://gallery.technet.microsoft.com/scriptcenter/Calculate-the-week-number-39e4b2c1

        /*
        * This presumes that weeks start with Monday. 
        * Week 1 is the 1st week of the year with a Thursday in it. 
        * Seriously cheat. If its Monday, Tuesday or Wednesday, then it'll 
        * be the same week-number as whatever Thursday, Friday or Saturday are, 
        * and we always get those right 
        */
        var date = DateTime.Today;
        var day = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date); 
        
        if (day == DayOfWeek.Monday || day == DayOfWeek.Tuesday || day == DayOfWeek.Wednesday)
        { 
            date = date.AddDays(3);
        } 
        
        // Return the week of our adjusted day 
        var week = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
            date,
            System.Globalization.CalendarWeekRule.FirstFourDayWeek,
            System.DayOfWeek.Monday);
        buildId = string.IsNullOrEmpty(buildId) ? "0" : buildId;
        return productVersion + "." + date.ToString("yy") + week.ToString("00") + "." + buildId;
    }

    Information("Using file pattern " + applicationVersionReferenceFileSearchPattern + " to get the version");
    var files = GetFiles(applicationVersionReferenceFileSearchPattern);
    if (files.Count != 1)
    {
        throw new Exception(
            "Couldn't find single file with name "
                + applicationVersionReferenceFileSearchPattern
                + " (found: " + files.Count + "). Couldn't evaluate application version");
    }

    var version = GetFullVersionNumber(files.Single());
    Information("Found version " + version + " from file " + files.Single());
    return version;
}

string GetPackageVersion(string applicationVersion, string tag)
{
    if (string.IsNullOrEmpty(tag))
    {
        return applicationVersion;
    }

    return applicationVersion + "-" + tag + "-" + GetUniqueNumber();
}

string GetUniqueNumber()
{
    // epoch: 01.11.2009 07:00
    var epoch = new DateTime(2009, 11, 1, 7, 0, 0, 0, DateTimeKind.Utc);
    return Math.Round(DateTime.UtcNow.Subtract(epoch).TotalSeconds).ToString();
}

void UseApplicationVersion(string applicationVersion, string repositoryPath, string currentFilePath)
{
    var buildNumberSuffix = applicationVersion.Substring(applicationVersion.IndexOf('.', applicationVersion.IndexOf('.') + 1));
    var files = GetFiles(repositoryPath + "/**/SolutionAssemblyProductInfo.cs");
    foreach (var file in files)
    {
        var replacement = string.Equals(file.FullPath, currentFilePath)
            ? applicationVersion : "$1.$2" + buildNumberSuffix;

        System.IO.File.WriteAllText(
            file.FullPath,
            System.Text.RegularExpressions.Regex.Replace(
                System.IO.File.ReadAllText(file.FullPath),
                @"AssemblyVersion\(""([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]+)""\)",
                "AssemblyVersion(\"" + replacement + "\")"));
        System.IO.File.WriteAllText(
            file.FullPath,
            System.Text.RegularExpressions.Regex.Replace(
                System.IO.File.ReadAllText(file.FullPath),
                @"AssemblyFileVersion\(""([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]+)""\)",
                "AssemblyFileVersion(\"" + replacement + "\")"));
    }

    // Required by VSTS to update build number. Keep in warning, so that it is written also with low verbosity levels
    Warning("##vso[build.updatebuildnumber]" + applicationVersion);
    Warning("##vso[task.setvariable variable=ApplicationVersion;]" + applicationVersion);
}

void UploadLegacyPackage(
    DirectoryPath deployDirectory,
    FilePath centerCliExePath,
    string centerCliPassword,
    string manifestId,
    string buildNumber)
{
    Information(
        "Uploading legacy package from deploy directory '"
            + deployDirectory + "', manifest '" + manifestId + "'"
            + " and build number '" + buildNumber + "'. Using CLI path: '" + centerCliExePath + "'");
    EnsureDirectoryExists(deployDirectory);
    CleanDirectory(deployDirectory);

    var absoluteDeployDirectory = MakeAbsolute(deployDirectory);

    var settings = new ProcessSettings
        {
            Arguments = @"uploadLegacyPackage -e local --mid=" + manifestId +" --path=\"" + absoluteDeployDirectory +
                "\" --password=" + centerCliPassword + " --version=" + buildNumber
        }.UseWorkingDirectory(centerCliExePath.GetDirectory());
    using(var process = StartAndReturnProcess(centerCliExePath, settings))
    {
        process.WaitForExit();
        // This should output 0 as valid arguments supplied
        Information("Exit code: {0}", process.GetExitCode());
    }
}

void Publish(DirectoryPath sourceDirectory, DirectoryPath publishDirectory, PublishOptions publishOptions)
{
    if (publishOptions.Includes == null)
    {
        Error("PublishOptions specified without any include");
        throw new ArgumentException("PublishOptions specified without any include", "publishOptions");
    }

    Information("Publishing from '" + sourceDirectory + "' to '" + publishDirectory + "'");

    Debug("Copying using " + publishOptions.Includes.Length + " include directive(s)");
    foreach (var include in publishOptions.Includes)
    {
        var fullInclude = sourceDirectory + (include.StartsWith("/") ? include : "/" + include);
        Debug("Copying include directive '" + fullInclude + "'");
        CopyFiles(fullInclude, publishDirectory, true);
    }

    if (publishOptions.Excludes != null)
    {
        foreach (var exclude in publishOptions.Excludes)
        {
            var fullExclude = publishDirectory + (exclude.StartsWith("/") ? exclude : "/" + exclude);
            Debug("Removing exclude directive '" + fullExclude + "'");
            DeleteFiles(fullExclude);
        }
    }

    if (publishOptions.RenameItems == null)
    {
        return;
    }

    foreach (var renameItem in publishOptions.RenameItems)
    {
        Warning("Renaming '" + renameItem.Source + "' to '" + renameItem.Destination + "'");
        var targetFile = publishDirectory.CombineWithFilePath(renameItem.Destination);
        if (FileExists(targetFile))
        {
            Warning("Target file '" + targetFile + "' already exists. Deleting it");
            DeleteFile(targetFile);
        }

        var sourceFile = publishDirectory.CombineWithFilePath(renameItem.Source);
        if (!FileExists(sourceFile))
        {
            Error("Source file '" + sourceFile + "' not found. Renaming not possible.");
            return;
        }

        Debug("Moving file '" + sourceFile + "' to '" + targetFile + "'");
        MoveFile(sourceFile, targetFile);
    }
}

void UseCenterCliEnvironment(DirectoryPath pathToCenterCliDirectory, Uri tcpServices)
{
    Debug(
        "Setting CenterCli environment for path '" + pathToCenterCliDirectory + "' and services '" + tcpServices + "'");
    var environments = new []{
        new {
            name = "local",
            wcfServicesHost = tcpServices.ToString()
        }
    };

    var path = pathToCenterCliDirectory.CombineWithFilePath(File("./Environments.json"));
    SerializeJsonToFile(path, environments);
}