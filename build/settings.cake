public static class Settings
{
    public static string ProjectName => "AzureDevOpsKats.Web";

    public static string SonarUrl => "http://sonar.navigatorglass.com:9000";

    public static string SonarKey => "9c944632fe7a37d24b533680dac1e45b5b34fea7";

    public static string SonarName => "AzureDevOpsKats";

    public static string SonarExclude => "/d:sonar.exclusions=**/Extensions/Swagger/**,**/Helpers/**,**/wwwroot/js/**,Program.cs,**/ServiceCollectionExtensions.cs";

    public static string SonarExcludeDuplications => "/d:sonar.cpd.exclusions=**/Extensions.cs";

    public static string MyGetSource => "https://www.myget.org/F/azuredevopskats/api/v2/package";
}
