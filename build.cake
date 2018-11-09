#tool nuget:?package=MSBuild.SonarQube.Runner.Tool
#addin nuget:?package=Cake.Sonar

var target = Argument("Target", "Default");
var configuration = Argument("configuration", "Release");
var login = Argument<String>("login", null);

var artifactsDirectory = Directory("./artifacts");
var testResultsDirectory = Directory("./.test-results");

var settings = new SonarBeginSettings{
  Url = "http://sonar.navigatorglass.com:9000",
  Key = "9c944632fe7a37d24b533680dac1e45b5b34fea7",
  Name = "AzureDevOpsKats"
};


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
           Information("Testing project " + project);  
           DotNetCoreTest(project.FullPath, new DotNetCoreTestSettings
           {
               Configuration = configuration,
               NoBuild = true,
               Logger = "trx",
               ResultsDirectory = testResultsDirectory,
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
