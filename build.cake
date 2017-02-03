#tool "nuget:?package=KuduSync.NET"
#addin "nuget:?package=Cake.Kudu"

var target = Argument<string>("target", "Default");
var config = Argument<string>("configuration", "Release");

Task("Clean")
    .Does(() =>
{
    CleanDirectory("./.artifacts");
    CleanDirectory("./.artifacts/bin");
    CleanDirectory($"./src/CakeTron/bin/{config}");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore("./", new DotNetCoreRestoreSettings
    {
        Verbose = false,
        Verbosity = DotNetCoreRestoreVerbosity.Warning,
        Sources = new [] { "https://api.nuget.org/v3/index.json" }
    });
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild("./src/CakeTron/project.json", new DotNetCoreBuildSettings {
        Configuration = config
    });

    DotNetCorePublish("./src/CakeTron/project.json", new DotNetCorePublishSettings
    {
        Framework = "netcoreapp1.1",
        Configuration = config,
        VersionSuffix = "alpha",
        OutputDirectory = "./.artifacts/bin",
        NoBuild = true,
        Verbose = false
    });
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() =>
{
    // Copy files.
    CopyFileToDirectory("./res/run.cmd", "./.artifacts/bin");

    if(!Kudu.IsRunningOnKudu)
    {
        // Zip the bin folder for manual upload.
        Zip("./.artifacts/bin", "./.artifacts/CakeTron.zip");
    }
});

Task("Deploy")
    .IsDependentOn("Package")
    .WithCriteria(Kudu.IsRunningOnKudu)
    .Does(() =>
{
    var source = MakeAbsolute(Directory("./.artifacts/bin"));
    var destination = MakeAbsolute(Kudu.Deployment.Target.Combine("./app_data/jobs/continuous/CakeTron"));

    Information("Deploying from {0} to {1}.", source, destination);
    Kudu.Sync(source, destination);
});

Task("Default")
    .IsDependentOn("Package");

Task("Kudu")
    .IsDependentOn("Deploy");

RunTarget(target);