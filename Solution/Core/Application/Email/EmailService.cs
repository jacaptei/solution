using HandlebarsDotNet;
using HandlebarsDotNet.IO;

using JaCaptei.Model;
using JaCaptei.Model.DTO;

using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Logging;

using MimeKit;
using MimeKit.Text;

namespace JaCaptei.Application.Email
{
    public class EmailService
    {
        const string TEMPLATE_IMOVIEW_INATIVOS = @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">

<head>
  <title>JáCaptei Integração Imóveis - Imoview</title>
  <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
</head>

<body style=""font-family: 'segoe-ui','roboto','calibri','trebuchet-ms','verdana"",'helvetica','sans-serif';color:#64656e"">
  <style>
    @media only screen {
      html {
        min-height: 100%;
        background: #f3f3f3;
      }
    }
    
    @media only screen and (max-width: 596px) {
      .small-float-center {
        margin: 0 auto !important;
        float: none !important;
        text-align: center !important;
      }
      .small-text-center {
        text-align: center !important;
      }
      .small-text-left {
        text-align: left !important;
      }
      .small-text-right {
        text-align: right !important;
      }
    }
    
    @media only screen and (max-width: 596px) {
      .hide-for-large {
        display: block !important;
        width: auto !important;
        overflow: visible !important;
        max-height: none !important;
        font-size: inherit !important;
        line-height: inherit !important;
      }
    }
    
    @media only screen and (max-width: 596px) {
      table.body table.container .hide-for-large,
      table.body table.container .row.hide-for-large {
        display: table !important;
        width: 100% !important;
      }
    }
    
    @media only screen and (max-width: 596px) {
      table.body table.container .callout-inner.hide-for-large {
        display: table-cell !important;
        width: 100% !important;
      }
    }
    
    @media only screen and (max-width: 596px) {
      table.body table.container .show-for-large {
        display: none !important;
        width: 0;
        mso-hide: all;
        overflow: hidden;
      }
    }
    
    @media only screen {
      a[x-apple-data-detectors] {
        color: inherit !important;
        text-decoration: none !important;
        font-size: inherit !important;
        font-family: inherit !important;
        font-weight: inherit !important;
        line-height: inherit !important;
      }
    }
    
    @media only screen and (max-width: 596px) {
      .menu.small-vertical .menu-item {
        padding-left: 0 !important;
        padding-right: 0 !important;
      }
    }
    
    @media only screen and (max-width: 596px) {
      table.body img {
        width: auto;
        height: auto;
      }
      table.body center {
        min-width: 0 !important;
      }
      table.body .container {
        width: 95% !important;
      }
      table.body .columns,
      table.body .column {
        height: auto !important;
        -moz-box-sizing: border-box;
        -webkit-box-sizing: border-box;
        box-sizing: border-box;
        padding-left: 16px !important;
        padding-right: 16px !important;
      }
      table.body .collapse > tbody > tr > .columns,
      table.body .collapse > tbody > tr > .column {
        padding-left: 0 !important;
        padding-right: 0 !important;
      }
      td.small-1,
      th.small-1 {
        display: inline-block !important;
        width: 8.333333% !important;
      }
      td.small-2,
      th.small-2 {
        display: inline-block !important;
        width: 16.666666% !important;
      }
      td.small-3,
      th.small-3 {
        display: inline-block !important;
        width: 25% !important;
      }
      td.small-4,
      th.small-4 {
        display: inline-block !important;
        width: 33.333333% !important;
      }
      td.small-5,
      th.small-5 {
        display: inline-block !important;
        width: 41.666666% !important;
      }
      td.small-6,
      th.small-6 {
        display: inline-block !important;
        width: 50% !important;
      }
      td.small-7,
      th.small-7 {
        display: inline-block !important;
        width: 58.333333% !important;
      }
      td.small-8,
      th.small-8 {
        display: inline-block !important;
        width: 66.666666% !important;
      }
      td.small-9,
      th.small-9 {
        display: inline-block !important;
        width: 75% !important;
      }
      td.small-10,
      th.small-10 {
        display: inline-block !important;
        width: 83.333333% !important;
      }
      td.small-11,
      th.small-11 {
        display: inline-block !important;
        width: 91.666666% !important;
      }
      td.small-12,
      th.small-12 {
        display: inline-block !important;
        width: 100% !important;
      }
      .columns td.small-12,
      .column td.small-12,
      .columns th.small-12,
      .column th.small-12 {
        display: block !important;
        width: 100% !important;
      }
      table.body td.small-offset-1,
      table.body th.small-offset-1 {
        margin-left: 8.333333% !important;
        Margin-left: 8.333333% !important;
      }
      table.body td.small-offset-2,
      table.body th.small-offset-2 {
        margin-left: 16.666666% !important;
        Margin-left: 16.666666% !important;
      }
      table.body td.small-offset-3,
      table.body th.small-offset-3 {
        margin-left: 25% !important;
        Margin-left: 25% !important;
      }
      table.body td.small-offset-4,
      table.body th.small-offset-4 {
        margin-left: 33.333333% !important;
        Margin-left: 33.333333% !important;
      }
      table.body td.small-offset-5,
      table.body th.small-offset-5 {
        margin-left: 41.666666% !important;
        Margin-left: 41.666666% !important;
      }
      table.body td.small-offset-6,
      table.body th.small-offset-6 {
        margin-left: 50% !important;
        Margin-left: 50% !important;
      }
      table.body td.small-offset-7,
      table.body th.small-offset-7 {
        margin-left: 58.333333% !important;
        Margin-left: 58.333333% !important;
      }
      table.body td.small-offset-8,
      table.body th.small-offset-8 {
        margin-left: 66.666666% !important;
        Margin-left: 66.666666% !important;
      }
      table.body td.small-offset-9,
      table.body th.small-offset-9 {
        margin-left: 75% !important;
        Margin-left: 75% !important;
      }
      table.body td.small-offset-10,
      table.body th.small-offset-10 {
        margin-left: 83.333333% !important;
        Margin-left: 83.333333% !important;
      }
      table.body td.small-offset-11,
      table.body th.small-offset-11 {
        margin-left: 91.666666% !important;
        Margin-left: 91.666666% !important;
      }
      table.body table.columns td.expander,
      table.body table.columns th.expander {
        display: none !important;
      }
      table.body .right-text-pad,
      table.body .text-pad-right {
        padding-left: 10px !important;
      }
      table.body .left-text-pad,
      table.body .text-pad-left {
        padding-right: 10px !important;
      }
      table.menu {
        width: 100% !important;
      }
      table.menu td,
      table.menu th {
        width: auto !important;
        display: inline-block !important;
      }
      table.menu.vertical td,
      table.menu.vertical th,
      table.menu.small-vertical td,
      table.menu.small-vertical th {
        display: block !important;
      }
      table.menu[align=center] {
        width: auto !important;
      }
      table.button.small-expand,
      table.button.small-expanded {
        width: 100% !important;
      }
      table.button.small-expand table,
      table.button.small-expanded table {
        width: 100%;
      }
      table.button.small-expand table a,
      table.button.small-expanded table a {
        text-align: center !important;
        width: 100% !important;
        padding-left: 0 !important;
        padding-right: 0 !important;
      }
      table.button.small-expand center,
      table.button.small-expanded center {
        min-width: 0;
      }
      th.callout-inner {
        padding: 10px !important;
      }
    }
  </style>
  <img src=""https://admin.jacaptei.com.br/resources/images/header_mail.png"" width=""240"" style=""-ms-interpolation-mode: bicubic; clear: both; display: block; max-width: 100%; outline: none; text-decoration: none; width: 240px;""><br>
  <div style=""margin-left:10px"">
    <b>Atualização dos Imóveis Integração Imoview - {{Cliente.Nome}}</b><br>
    <b>Data da atualização: {{DataEnvio}}<br><br>
        <b>Segue a lista de imovéis que se encontram indisponíveis para venda:</b><br><br> {{#each Imoveis}}
    <table class=""callout"" style=""Margin-bottom: 16px; border-collapse: collapse; border-spacing: 0; margin-bottom: 16px; padding: 0; text-align: left; vertical-align: top;"">
      <tbody>
        <tr style=""padding: 0; text-align: left; vertical-align: top;"">
          <th class=""callout-inner secondary"" style=""-moz-box-sizing: border-box; -moz-hyphens: auto; -webkit-box-sizing: border-box; -webkit-hyphens: auto; Margin: 0; background: #ebebeb; border: 1px solid #444444; border-collapse: collapse !important; box-sizing: border-box; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; hyphens: auto; line-height: 130%; margin: 0; padding: 10px; text-align: left; vertical-align: top; width: 100%; word-wrap: break-word;"">
            <table class=""row"" style=""border-collapse: collapse; border-spacing: 0; padding: 0; position: relative; text-align: left; vertical-align: top; width: 100%;"">
              <tbody>
                <tr style=""padding: 0; text-align: left; vertical-align: top;"">
                  <th class=""small-12 large-8 columns first"" style=""-moz-box-sizing: border-box; -moz-hyphens: auto; -webkit-box-sizing: border-box; -webkit-hyphens: auto; Margin: 0 auto; border-collapse: collapse !important; box-sizing: border-box; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; hyphens: auto; line-height: 130%; margin: 0 auto; padding: 0; padding-bottom: 16px; padding-left: 16px; padding-right: 8px; text-align: left; vertical-align: top; width: 274px; word-wrap: break-word;"">
                    <table style=""border-collapse: collapse; border-spacing: 0; padding: 0; text-align: left; vertical-align: top; width: 100%;"">
                      <tbody>
                        <tr style=""padding: 0; text-align: left; vertical-align: top;"">
                          <th style=""-moz-box-sizing: border-box; -moz-hyphens: auto; -webkit-box-sizing: border-box; -webkit-hyphens: auto; Margin: 0; border-collapse: collapse !important; box-sizing: border-box; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; hyphens: auto; line-height: 130%; margin: 0; padding: 0; text-align: left; vertical-align: top; word-wrap: break-word;"">
                            <p style=""Margin: 0; Margin-bottom: 10px; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; line-height: 130%; margin: 0; margin-bottom: 10px; padding: 0; text-align: left;"">
                              <strong>Código Imoview</strong><br> {{CodImoview}}
                            </p>
                            <p style=""Margin: 0; Margin-bottom: 10px; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; line-height: 130%; margin: 0; margin-bottom: 10px; padding: 0; text-align: left;"">
                              <strong>Descrição</strong><br> {{Descricao}}
                            </p>
                            <p style=""Margin: 0; Margin-bottom: 10px; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; line-height: 130%; margin: 0; margin-bottom: 10px; padding: 0; text-align: left;"">
                              <strong>Código Jacaptei</strong><br> {{CodJacaptei}}
                            </p>
                          </th>
                        </tr>
                      </tbody>
                    </table>
                  </th>
                  <th class=""small-12 large-4 columns last"" style=""-moz-box-sizing: border-box; -moz-hyphens: auto; -webkit-box-sizing: border-box; -webkit-hyphens: auto; Margin: 0 auto; border-collapse: collapse !important; box-sizing: border-box; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; hyphens: auto; line-height: 130%; margin: 0 auto; padding: 0; padding-bottom: 16px; padding-left: 8px; padding-right: 16px; text-align: left; vertical-align: top; width: 274px; word-wrap: break-word;"">
                    <table style=""border-collapse: collapse; border-spacing: 0; padding: 0; text-align: left; vertical-align: top; width: 100%;"">
                      <tbody>
                        <tr style=""padding: 0; text-align: left; vertical-align: top;"">
                          <th style=""-moz-box-sizing: border-box; -moz-hyphens: auto; -webkit-box-sizing: border-box; -webkit-hyphens: auto; Margin: 0; border-collapse: collapse !important; box-sizing: border-box; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; hyphens: auto; line-height: 130%; margin: 0; padding: 0; text-align: left; vertical-align: top; word-wrap: break-word;"">
                            <p style=""Margin: 0; Margin-bottom: 10px; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; line-height: 130%; margin: 0; margin-bottom: 10px; padding: 0; text-align: left;"">
                              <strong>Endereco</strong><br> {{Endereco.rua}}, {{Endereco.numero}}<br> {{Endereco.complemento}}, {{Endereco.bairro}}<br> {{Endereco.cidade}} {{Endereco.uf}}<br>
                            </p>
                          </th>
                        </tr>
                      </tbody>
                    </table>
                  </th>
                </tr>
              </tbody>
            </table>
          </th>
          <th class=""expander"" style=""-moz-box-sizing: border-box; -moz-hyphens: auto; -webkit-box-sizing: border-box; -webkit-hyphens: auto; Margin: 0; border-collapse: collapse !important; box-sizing: border-box; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; hyphens: auto; line-height: 130%; margin: 0; padding: 0 !important; text-align: left; vertical-align: top; visibility: hidden; width: 0; word-wrap: break-word;""></th>
        </tr>
      </tbody>
    </table>
    {{/each}}
    <br><br>
    </b>
  </div><br><br>
  <div style=""color:#131a2e;margin-left:10px"">JáCaptei 2024.<br>
    <a href=""https://jacaptei.com.br"" target=""_blank"" style=""color: #ef5924; font-family: Helvetica, Arial, sans-serif; font-weight: normal; line-height: 130%; padding: 0; text-align: left; text-decoration: underline;"">https://jacaptei.com.br</a><br><br>
  </div>

</body>

</html>
";
        private readonly string _nameSender;
        private readonly string _emailSender;
        private readonly string _userFrom;
        private readonly string _passwordFrom;
        private readonly int _smtpPort;
        private readonly string _smtpHost;
        private readonly bool _enableSsl;
        private readonly ILogger? _logger;

        public EmailService(ILogger? logger = null)
        {
            _nameSender = Config.settings.mailServerFromName;
            _emailSender = Config.settings.mailServerFromName + " <" + Config.settings.mailServerFromEmail + ">";
            _userFrom = Config.settings.mailServerFromEmail;
            _passwordFrom = Config.settings.mailServerFromPassword;
            _smtpPort = int.Parse(Config.settings.mailServerSmtpPort);
            _smtpHost = Config.settings.mailServerSmtpHost;
            _enableSsl = bool.Parse(Config.settings.mailServerEnableSsl);
            _logger = logger;
        }
        public async Task<(bool, string)> EnviarImoviewInativos(EmailImoveisInativadosImoview emailInativos)
        {
            var format = "dd/MM/yyyy";
            var formatter = new CustomDateTimeFormatter(format);
            Handlebars.Configuration.FormatterProviders.Add(formatter);
            emailInativos.Imoveis.ForEach(i => i.Descricao = i?.Descricao?.Length > 150 ? i.Descricao.Substring(0, 150) + "..." : i.Descricao);
            var template = Handlebars.Compile(TEMPLATE_IMOVIEW_INATIVOS);
            var emailContent = template(emailInativos);
            var emailTo = emailInativos.Cliente.Nome + " <" + emailInativos.Cliente.Email + ">";
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(_emailSender));
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = "Atualização dos Imóveis Integração Imoview - " + emailInativos.Cliente.Nome;
            email.Body = new TextPart(TextFormat.Html) { Text = emailContent };

            using var emailClient = new SmtpClient();
            try
            {
                await emailClient.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                await emailClient.AuthenticateAsync(_userFrom, _passwordFrom);
                await emailClient.SendAsync(email);
                await emailClient.DisconnectAsync(true);
                return (true, emailContent);
            }
            catch (Exception e)
            {
                _logger?.LogError("Erro ao enviar email para {to}. Erro {e}", emailTo, e);
                return (false, emailContent);
            }
        }
    }

    public sealed class CustomDateTimeFormatter : IFormatter, IFormatterProvider
    {
        private readonly string _format;

        public CustomDateTimeFormatter(string format) => _format = format;

        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            if (!(value is DateTime dateTime))
                throw new ArgumentException("supposed to be DateTime");

            writer.Write($"{dateTime.ToString(_format)}");
        }

        public bool TryCreateFormatter(Type type, out IFormatter formatter)
        {
            if (type != typeof(DateTime))
            {
                formatter = null;
                return false;
            }

            formatter = this;
            return true;
        }
    }
}
