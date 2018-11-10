#addin "nuget:?package=Cake.MiniCover&version=0.29.0-next20180721071547&prerelease"

#tool nuget:?package=xunit.runner.console&version=2.2.0
#tool nuget:?package=xunit.runner.visualstudio&version=2.2.0

#tool nuget:?package=MSBuild.SonarQube.Runner.Tool
#addin nuget:?package=Cake.Sonar

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
SetMiniCoverToolsProject("./build/tools.csproj");


var target = Argument("Target", "Default");
var configuration = Argument("configuration", "Release");
var login = Argument<String>("login", null);

var artifactsDirectory = Directory("./artifacts");
var testResultsDirectory = Directory("./.test-results");
var coverageResultsDirectory = Directory("./coverage-html");


var settings = new SonarBeginSettings{
  Url = "http://sonar.navigatorglass.com:9000",
  Key = "9c944632fe7a37d24b533680dac1e45b5b34fea7",
  Name = "AzureDevOpsKats"
};


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDirectory);
        CleanDirectory(testResultsDirectory);
        CleanDirectory(coverageResultsDirectory);
        DeleteFiles("./coverage*.*");
    });


Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore();
    });


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
               Logger = "trx;LogFileName=UnitTestResults.trx",
               ResultsDirectory = testResultsDirectory,
               ArgumentCustomization = args => args.Append($"--no-restore")
           });
        }
    });

Task("Coverage")
   .IsDependentOn("Test").Does(() => 
   {
        Information("Code Coverage");  
        MiniCover(tool => 
        {
            foreach(var project in GetFiles("./test/**/*.csproj"))
            {
                DotNetCoreTest(project.FullPath, new DotNetCoreTestSettings
                {
                    Configuration = configuration,
                    NoRestore = true,
                    NoBuild = true
                });
            }
        },
        new MiniCoverSettings()
            .WithAssembliesMatching("./test/**/*.dll")
            .WithSourcesMatching("./src/**/*.cs")
            .WithNonFatalThreshold()
            .GenerateReport(ReportType.CONSOLE | ReportType.XML | ReportType.HTML)
        );
   });


Task("Pack")
    .IsDependentOn("Coverage")
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

    
//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);
