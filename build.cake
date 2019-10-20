#tool nuget:?package=GitVersion.CommandLine&version=5.0.1

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

DirectoryPath msbuildInstallationPath;
FilePath msbuildExePath;
GitVersion version;

ConvertableDirectoryPath sourceDir;
ConvertableDirectoryPath stageDir;
ConvertableDirectoryPath packageDir;
ConvertableDirectoryPath publishDir;
FilePath slnPath;

Setup(ctx =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");

    msbuildInstallationPath = VSWhereLatest(new VSWhereLatestSettings { Requires = "Microsoft.Component.MSBuild" });
    msbuildExePath = msbuildInstallationPath.CombineWithFilePath("MSBuild/current/Bin/MSBuild.exe");

    sourceDir = Directory("./source");
    stageDir = Directory("./stage");
    packageDir = Directory("./stage/package");
    publishDir = Directory("./stage/publish");
    slnPath = File("./DanielsWpfCoaster.sln");

    version = GitVersion(new GitVersionSettings
    {
        UpdateAssemblyInfo = false
    });
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean").Does(() =>
{
    CleanDirectories("**/bin/" + configuration);
    CleanDirectories("**/obj/" + configuration);
	CleanDirectory(stageDir);
    EnsureDirectoryExists(packageDir);
    EnsureDirectoryExists(publishDir);
});

Task("PrepareBuild").IsDependentOn("Clean").Does(() =>
{
    CreateAssemblyInfo(stageDir + File("AssemblyVersion.generated.cs"), new AssemblyInfoSettings
    {
        Version = version.MajorMinorPatch,
        FileVersion = version.MajorMinorPatch,
        InformationalVersion = version.InformationalVersion,
    });
});

Task("Build").IsDependentOn("PrepareBuild").Does(() =>
{
     DotNetBuild(slnPath, settings => settings.SetConfiguration(configuration));
});

Task("Test").IsDependentOn("Build").Does(() =>
{
    NUnit3($"./**/bin/{configuration}/*.Tests.dll", new NUnit3Settings
    {
        //X86 = true,
        Results = new[]
        {
            new NUnit3Result
            {
                FileName = stageDir + File("TestResult.xml")
            }
        },
    });
});

Task("Package").IsDependentOn("Build").Does(() =>
{
    var libDir = packageDir + Directory("lib");

    EnsureDirectoryExists(libDir);
    CopyFileToDirectory(File("./nuspecs/coaster.nuspec"), packageDir);
    CopyFiles($"source/DanielsWpfCoaster/bin/{configuration}/**/DanielsWpfCoaster.dll", libDir, true);

    var settings = new NuGetPackSettings
    {
        Version = version.NuGetVersion,
        //ProjectUrl = new Uri("https://github.com/dansav/coroutines-for-wpf"),
        //License = new NuSpecLicense() { Type = "expression", Value = "MIT" },
        BasePath = packageDir,
        OutputDirectory = publishDir
    };
    NuGetPack(packageDir + File("coaster.nuspec"), settings);
});

Task("Publish")
    .IsDependentOn("Package")
    .WithCriteria(() => (version.BranchName == "master" || version.BranchName.StartsWith("release/")) && !BuildSystem.IsLocalBuild && !BuildSystem.IsPullRequest)
    .Does(() =>
{
    if (string.IsNullOrWhiteSpace(EnvironmentVariable("NUGET_API_KEY")))
    {
        Warning($"Not Pushing the nuget package. Api key is missing!");
        return;
    }

    var feedUrl = "https://api.nuget.org/v3/index.json";
    var package = GetFiles($"{publishDir}/*{version.NuGetVersion}.nupkg").First();

    Information($"Pushing {package} to {feedUrl}");

    var settings = new NuGetPushSettings
    {
        Source = feedUrl,
        ApiKey = EnvironmentVariable("NUGET_API_KEY")
    };

    NuGetPush(package, settings);
});

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);