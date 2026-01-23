#load build/settings.cake
#load build/helpers.cake

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

#tool nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.8.0
#tool nuget:?package=xunit.runner.console&version=2.9.2
#tool nuget:?package=xunit.runner.visualstudio&version=2.8.2
#tool nuget:?package=DocFx.Console&version=2.58.9
#tool nuget:?package=OpenCoverToCoberturaConverter&version=0.3.4

//////////////////////////////////////////////////////////////////////
// ADDINS
//////////////////////////////////////////////////////////////////////

#addin nuget:?package=Cake.MiniCover&version=0.29.0-next20180721071547&prerelease
#addin nuget:?package=Cake.Sonar&version=1.1.29
#addin nuget:?package=Cake.DocFx&version=1.0.0
#addin "nuget:?package=Cake.OpenCoverToCoberturaConverter&version=0.1.10.11"

SetMiniCoverToolsProject("./build/tools.csproj");

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("Target", "Default");
var configuration = Argument("configuration", "Release");
var login = Argument<String>("login", null);
var mygetApiKey = EnvironmentVariable("mygetApiKey");


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var projectName = Settings.ProjectName;
var projectDirectory =  Directory(".") +  Directory("src") +  Directory(projectName);

var artifactsDirectory = Directory("./artifacts");
var sonarDirectory = Directory("./.sonarqube");
var testResultsDirectory = Directory("./.test-results");
var coverageResultsDirectory = Directory("./coverage-html");
var publishDirectory = Directory(".") + Directory("publish") + Directory(configuration);


///////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
	//versionInfo = GitVersion();
	//Information("Building for version {0}", versionInfo.FullSemVer);
});


Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDirectory);
        CleanDirectory(publishDirectory);
        CleanDirectory(testResultsDirectory);
        CleanDirectory(coverageResultsDirectory);

        DeleteFiles("./*coverage*.*");
    });


Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetRestore();
    });


 Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var projects = GetFiles("./**/*.csproj");
        foreach(var project in projects)
        {
            DotNetBuild(
                project.GetDirectory().FullPath,
                new DotNetBuildSettings()
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
           DotNetTest(project.FullPath, new DotNetTestSettings
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
   .IsDependentOn("Test")
   .Does(() =>
   {
        Information("Code Coverage");
        MiniCover(tool =>
        {
            foreach(var project in GetFiles("./test/**/*.csproj"))
            {
                DotNetTest(project.FullPath, new DotNetTestSettings
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
            .GenerateReport(ReportType.OPENCOVER |  ReportType.CONSOLE | ReportType.XML | ReportType.HTML)
        );

        if (!BuildSystem.TravisCI.IsRunningOnTravisCI)
        {
            OpenCoverToCoberturaConverter("./opencovercoverage.xml", "./cobertura-coverage.xml");
        }

   });

Task("Publish")
    .IsDependentOn("Test")
    .Does(() =>
{
    DotNetPublish(
        projectDirectory,
        new DotNetPublishSettings()
        {
            Configuration = configuration,
            OutputDirectory = publishDirectory
        });

    Zip("./publish/Release", "./publish/webapplication.zip");

});

Task("Generate-Docs")
    .IsDependentOn("Clean")
    .Does(() =>
    {
       DocFxBuild("./docfx/docfx.json");
       Zip("./docfx/_site/", "./artifacts/docfx.zip");
    });

Task("Clean-Sonarqube")
  .WithCriteria(BuildSystem.IsLocalBuild)
  .Does(()=>{
    CleanDirectory(sonarDirectory);
});


Task("SonarBegin")
    .Does(() => { SonarBegin(new SonarBeginSettings {
        Url = Settings.SonarUrl,
        Key = Settings.SonarKey,
        Name = Settings.SonarName,
        ArgumentCustomization = args=>args
        .Append("/d:sonar.cs.opencover.reportsPaths=opencovercoverage.xml")
        .Append(Settings.SonarExclude)
        .Append(Settings.SonarExcludeDuplications)
    });
});

Task("SonarEnd")
    .Does(() => {
        SonarEnd(new SonarEndSettings{});
    });

Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
    {
        foreach (var project in GetFiles("./src/**/*.csproj"))
        {
            DotNetPack(
                project.GetDirectory().FullPath,
                new DotNetPackSettings()
                {
                    Configuration = configuration,
                    OutputDirectory = artifactsDirectory,
                    NoBuild = true
                });
        }
    });

Task("Push-Myget")
    .IsDependentOn("Pack")
    .Does(() => {
        var pushSettings = new DotNetNuGetPushSettings
        {
            Source = Settings.MyGetSource,
            ApiKey = mygetApiKey
        };

        Information($"artifactsDirectory \"{artifactsDirectory}\".");

        var packages = GetFiles("./artifacts/*.nupkg");
        foreach(var package in packages)
        {
            if(!IsNuGetPublished(package))
            {
                Information($"Publishing \"{package}\".");
                DotNetNuGetPush(package.FullPath, pushSettings);
            }
            else {
                Information($"Bypassing publishing \"{package}\" as it is already published.");
            }
        }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Pack");

Task("Sonar")
  .IsDependentOn("Clean-Sonarqube")
  .IsDependentOn("SonarBegin")
  .IsDependentOn("Coverage")
  .IsDependentOn("SonarEnd");


Task("CI-Build")
  .IsDependentOn("Test")
  .IsDependentOn("Publish");
  //.IsDependentOn("Coverage")
  //.IsDependentOn("Generate-Docs");


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);
