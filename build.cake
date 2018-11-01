var target = Argument("Target", "Default");
var configuration = Argument("configuration", "Release");
var artifactsDirectory = Directory("./artifacts");


// Deletes the contents of the Artifacts folder if it should contain anything from a previous build.
Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDirectory);
    });

// Run dotnet restore to restore all package references.
Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore();
    });

// Find all csproj projects and build them using the build configuration specified as an argument.
 Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var projects = GetFiles("./**/*.csproj");
        foreach(var project in projects)
        {
            DotNetCoreBuild(
                project.GetDirectory().FullPath,
                new DotNetCoreBuildSettings()
                {
                    Configuration = configuration
                });
        }
    });

// Look under a 'Test' folder and run dotnet test against all of those projects.
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var projects = GetFiles("./test/**/*.csproj");
        foreach(var project in projects)
        {
            DotNetCoreTest(
                project.GetDirectory().FullPath,
                
                new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    ArgumentCustomization = args => args.Append($"--no-restore")
                });
        }
    });
	
// Run dotnet pack to produce NuGet packages from our projects. Versions the package
Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
    {
        foreach (var project in GetFiles("./src/**/*.csproj"))
        {
            DotNetCorePack(
                project.GetDirectory().FullPath,
                new DotNetCorePackSettings()
                {
                    Configuration = configuration,
                    OutputDirectory = artifactsDirectory
                });
        }
    });
    
// The default task to run if none is explicitly specified. In this case, we want
// to run everything starting from Clean, all the way up to Pack.
Task("Default")
    .IsDependentOn("Pack");
// Executes the task specified in the target argument.
RunTarget(target);
