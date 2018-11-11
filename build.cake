//build.cake
//  .\build.ps1 -t Coverage

#load build/settings.cake

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

#tool nuget:?package=MSBuild.SonarQube.Runner.Tool
#tool nuget:?package=xunit.runner.console&version=2.2.0
#tool nuget:?package=xunit.runner.visualstudio&version=2.2.0

//////////////////////////////////////////////////////////////////////
// ADDINS
//////////////////////////////////////////////////////////////////////

#addin nuget:?package=Cake.MiniCover&version=0.29.0-next20180721071547&prerelease
// #addin nuget:?package=Cake.NSwag.Console
#addin nuget:?package=Cake.Sonar

SetMiniCoverToolsProject("./build/tools.csproj");

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("Target", "Default");
var configuration = Argument("configuration", "Release");
var login = Argument<String>("login", null);

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var projectName = Settings.ProjectName;
var projectDirectory =  Directory(".") +  Directory("src") +  Directory(projectName);

var artifactsDirectory = Directory("./artifacts");
var sonarDirectory = Directory("./.sonarqube");
var testResultsDirectory = Directory("./.test-results");
var coverageResultsDirectory = Directory("./coverage-html");
var publishirectory = Directory(".") + Directory("publish") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDirectory);
        CleanDirectory(publishirectory);
        CleanDirectory(testResultsDirectory);
        CleanDirectory(coverageResultsDirectory);
        //DeleteDirectory(sonarDirectory);
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


Task("Publish")
    .IsDependentOn("Test")
    .Does(() =>
{
    DotNetCorePublish(
        projectDirectory,
        new DotNetCorePublishSettings()
        {
            Configuration = configuration,
            OutputDirectory = publishirectory
        });
});

Task("Sonar")
  .IsDependentOn("SonarBegin")
  .IsDependentOn("Build")
  .IsDependentOn("SonarEnd");

Task("SonarBegin")
    .Does(() => { SonarBegin(new SonarBeginSettings {
        Url = Settings.SonarUrl,
        Key = Settings.SonarKey,
        Name = Settings.SonarName
        });
    });
  
Task("SonarEnd")
    .Does(() => { 
        SonarEnd(new SonarEndSettings{});
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
