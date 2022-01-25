namespace AzureDevOpsKats.Common.Configuration
{
    public class SmtpConfiguration
    {
        public const string SectionName = "Smtp";
        public int Port { get; set; }

        public string Server { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
