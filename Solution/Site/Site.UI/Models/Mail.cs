using System;
using System.Collections.Generic;
using System.Text;
using MailKit.Net.Smtp;
using System.Net;
using System.Drawing;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;

namespace JaCaptei.UI.Models {

    public class Mail {

        public int id { get; set; }
        public int idUser { get; set; }

        public string nameSender { get; set; }
        public string emailSender { get; set; }

        public string userFrom { get; set; }
        public string passwordFrom { get; set; }

        public string nameTo { get; set; }
        public string emailTo { get; set; }

        public string about { get; set; }
        public string title { get; set; }
        public string message { get; set; } = "";
        public string body { get; set; }

        public string baseColor { get; set; } = "#8f150c";
        public string baseURL { get; set; } = Config.settings.baseURL;

        public int smtpPort { get; set; }
        public string smtpHost { get; set; }
        public bool enableSsl { get; set; }
        public string SMTPSecure { get; set; }

        public List<string> files { get; set; }

        public DateTime data { get; set; }

        public Mail() {

            nameSender  = Config.settings.serverMailFromName;
            emailSender = Config.settings.serverMailFromName + " <"+ Config.settings.serverMailFromEmail + ">";

            //userFrom = "mailhomolog@gmail.com";
            ////passwordFrom        = "$mh222320"; // site
            //passwordFrom = "xgflswtrejhrubrv"; // apps
            //smtpPort = 587;
            //smtpHost = "smtp.gmail.com";
            ////smtpHost            = "pop.gmail.com";
            //enableSsl = false;


            userFrom        = Config.settings.serverMailFromEmail;
            passwordFrom    = Config.settings.serverMailFromPassword; 
            smtpPort        = int.Parse(Config.settings.serverMailSmtpPort);
            smtpHost        = Config.settings.serverMailSmtpHost;
            enableSsl       = bool.Parse(Config.settings.serverMailEnableSsl);

            nameTo  = Config.settings.serverMailDefaultToName;
            emailTo = Config.settings.serverMailDefaultToEmail;


            files = new List<string>();

        }





        private string buildMailBody() {

            string mailBody = "<html>"
                                    + "<head>"
                                    + "<title>JáCaptei Imóveis</title>"
                                    + "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">"
                                    + "</head>"
                                    + "<body style='font-family: \"Segoe UI\",\"Roboto\",\"Calibri\",\"Trebuchet MS\", \"Verdana\", \"Helvetica\",\"Sans-Serif\";color:#64656e'>"
                                        + "<img src=\"" + baseURL + "/resources/images/header_mail.png\"  width='240' style=\"display: block;width:240px\"  />" + "<br><br> "
                                        + "<div style='margin-left:10px'>"
                                             + message
                                        + "</div>"
                                        + "<br><br>"
                                        + "<div style='color:#131a2e;margin-left:10px'>"
                                             + "© JáCaptei "+(Utils.Date.GetLocalDate().Year.ToString()) +"."
                                             + "<br><a href='https://jacaptei.com.br' target='_blank'  style='color:#ef5924;text-decoration:underline;'>https://jacaptei.com.br</a>"
                                        + "<br><br></div>"
                                    + "</body>"
                             + "</html>";

            return mailBody;

        }


        private string getMailBody() {
            return buildMailBody();
        }











        public async void Send() {

            if (string.IsNullOrWhiteSpace(about))
                about = "mensagem via site";

            if (string.IsNullOrWhiteSpace(title))
                title = about;

            if (string.IsNullOrWhiteSpace(body))
                body = getMailBody();

            if (string.IsNullOrWhiteSpace(emailTo)) {
                nameTo  = Config.settings.serverMailDefaultToName;
                emailTo = Config.settings.serverMailDefaultToEmail;
            }

            if (string.IsNullOrWhiteSpace(emailSender)) {
                nameSender  = Config.settings.serverMailFromName;
                emailSender = Config.settings.serverMailFromName + " <"+ Config.settings.serverMailFromEmail + ">";
            }


            //message = "teste";

            /* ******************
                SEND MAIL
            ********************** */
            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailSender));
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = about;

            email.Body = new TextPart(TextFormat.Html) { Text = buildMailBody() };

            using (var emailClient = new SmtpClient()) {
                try {
                    //The last parameter here is to use SSL (Which you should!)
                    //emailClient.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                    //emailClient.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                    emailClient.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                    //Remove any OAuth functionality as we won't be using it. 
                    //emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                    emailClient.Authenticate(userFrom, passwordFrom);
                    emailClient.Send(email);
                    emailClient.Disconnect(true);
                } catch (Exception e) {
                    throw new Exception(e.Message);
                }
            }

            //mail.SubjectEncoding = Encoding.GetEncoding("ISO-8859-1");
            //mail.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");


        }









    }






}
