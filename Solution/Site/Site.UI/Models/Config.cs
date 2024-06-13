using System;

namespace JaCaptei.UI.Models {

    public static class Config {
        public static AppSettingsRecord settings { get; set; }
    }
    public class AppSettingsRecord {

        public string apiName { get; set; }
        public string apiVersion { get; set; }
        public string apiEndpoint { get; set; }
        public string environment { get; set; }
        public string key { get; set; }

        public string title { get; set; }
        public string description { get; set; }
        public string keywords { get; set; }
        public string baseColor { get; set; }

        public string DBconnectionString1 { get; set; }
        public string DBconnectionString2 { get; set; } // log

        public string serverMailFromName { get; set; }
        public string serverMailFromEmail { get; set; }
        public string serverMailFromPassword { get; set; }
        public string serverMailSmtpHost { get; set; }
        public string serverMailSmtpPort { get; set; }
        public string serverMailEnableSsl { get; set; }
        public string serverMailDefaultToName { get; set; }
        public string serverMailDefaultToEmail { get; set; }

        public string server = System.Environment.MachineName.Trim().ToUpper();

        public string baseURL { get; set; }
        public string host { get; set; }

        public string obs { get; set; }


        public void CopyToStaticSettings() {
            Config.settings = this;
        }


    }



}


