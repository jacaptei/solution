namespace JaCaptei.Model {

    public static class Config {
        public static AppSettingsRecord settings    { get; set; }
    }
    public class AppSettingsRecord {

        public string apiName                       { get; set; }
        public string apiVersion                    { get; set; }
        public string apiEndpoint                   { get; set; }
        public string environment                   { get; set; }
        public string release                       { get; set; }
        public string key                           { get; set; }
        public string token                         { get; set; }

        public string title                         { get; set; }
        public string description                   { get; set; }
        public string keywords                      { get; set; }
        public string baseColor                     { get; set; }

        public string DBconnectionString            { get; set; }
        public string DBconnectionStringMSSQL       { get; set; }

        public string mailServerFromName            { get; set; }
        public string mailServerFromEmail           { get; set; }
        public string mailServerFromPassword        { get; set; }
        public string mailServerSmtpHost            { get; set; }
        public string mailServerSmtpPort            { get; set; }
        public string mailServerEnableSsl           { get; set; }
        public string mailServerDefaultToName       { get; set; }
        public string mailServerDefaultToEmail      { get; set; }

        public string crmEndpoint                   { get; set; }
        public string crmGlobalUsername             { get; set; }
        public string crmGlobalPassword             { get; set; }

        public string server = System.Environment.MachineName.Trim().ToUpper();

        public string baseURL                       { get; set; }
        public string host                          { get; set; }

        public string obs                           { get; set; }

        public string AzureMQ                       { get; set; }

        public void CopyToStaticSettings() {
            Config.settings = this;
            Config.settings.environment = this.environment.ToUpper();
        }




    }
}




