namespace JaCaptei.Model{

    public class SiteSettings {

        public string       apiName                 { get; set; } = "";
        public string       apiVersion              { get; set; } = "";
        public string       environment             { get; set; } = "";
        public string       connectionString        { get; set; } = "";
        public string       connectionStringMSSQL   { get; set; } = "";
        public string       endpoint                { get; set; } = "";
        public bool         enableLog               { get; set; } = true;
        public AwsCognito   awsCognito              { get; set; } = new AwsCognito();
        public MailSettings mail                    { get; set; } = new MailSettings();

        public void CopyToStaticSettings(string _environment) {
            Settings.apiName                = apiName   ;
            Settings.apiVersion             = apiVersion;
            Settings.environment            = _environment;
            Settings.connectionString       = connectionString;
            Settings.connectionStringMSSQL  = connectionStringMSSQL;
            Settings.endpoint               = endpoint;
            Settings.enableLog              = enableLog;
            Settings.awsCognito             = awsCognito;
            Settings.mail                   = mail;
        }
    }

    public static class Settings {
        public static string        apiName                 { get; set; }
        public static string        apiVersion              { get; set; }
        public static string        environment             { get; set; }
        public static string        connectionString        { get; set; }
        public static string        connectionStringMSSQL   { get; set; }
        public static string        endpoint                { get; set; }
        public static bool          enableLog               { get; set; }
        public static string        key                     { get; set; } = "T7547759889876JuH<aGSqd$ML.dB!Rt^SocnP4WPa19GoW3xa&J%h;u9y^WXtD9jwDwSZ2mww";
        public static MailSettings  mail                    { get; set; } = new MailSettings();
        public static AwsCognito    awsCognito              { get; set; } = new AwsCognito();
    }


    public class AwsCognito { 
        public string region                            { get; set; } = "";
        public string userPoolId                        { get; set; } = ""; 
        public string appClientId                       { get; set; } = ""; 
        public string appClientDomain                   { get; set; } = ""; 
        public string accessSecretKey                   { get; set; } = ""; 
    }

    public class MailSettings { 
        public string   login                           { get; set; } = "";
        public string   senha                           { get; set; } = "";
        public int      smtpPort                        { get; set; } = 0;
        public string   smtpHost                        { get; set; } = "";
        public bool     enableSsl                       { get; set; } = false;
        public string   smtpSecureType                  { get; set; } = "";
        public string   remetenteNome                   { get; set; } = "";
        public string   remetenteEmail                  { get; set; } = "";
    }



}


