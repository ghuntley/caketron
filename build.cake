#tool nuget:?package=KuduSync.NET
#addin nuget:?package=Cake.Kudu

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
    DotNetCoreRestore("./src/CakeTron.sln", new DotNetCoreRestoreSettings
    {
        Verbose = false,
        Sources = new [] { "https://api.nuget.org/v3/index.json" }
    });
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    // Build
    DotNetCoreBuild("./src/CakeTron.sln", new DotNetCoreBuildSettings 
    {
        Configuration = config
    });

    // Publish
    DotNetCorePublish("./src/CakeTron.sln", new DotNetCorePublishSettings
    {
        Configuration = config,
        OutputDirectory = "./.artifacts/bin",
        Verbose = false
    });
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() =>
{
    // Copy files.
    CopyFileToDirectory("./res/run.cmd", "./.artifacts/bin");
    CopyFileToDirectory("./settings.job", "./.artifacts/bin");

    // Zip the bin folder for manual upload?
    if(!Kudu.IsRunningOnKudu)
    {
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

    // Sync folders.
    Information("Deploying from {0} to {1}.", source, destination);
    Kudu.Sync(source, destination);
});

Task("Default")
    .IsDependentOn("Package");

Task("Kudu")
    .IsDependentOn("Deploy");

RunTarget(target);