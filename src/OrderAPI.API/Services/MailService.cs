

using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace OrderAPI.API.Services
{
    public class MailService
    {
        private readonly IConfiguration _configuration;
        public MailService(IConfiguration config)
        {
            this._configuration = config;
        }

        public void SendRecoverPasswordMail(string email, string name, string code)
        {
            var mailMessage = new MailMessage();
            var smtpClient = new SmtpClient()
            {
                Port = int.Parse(this._configuration["MailSettings:Port"]),
                Host = this._configuration["MailSettings:Host"],
                EnableSsl = bool.Parse(this._configuration["MailSettings:EnableSsl"]),
                Timeout = int.Parse(this._configuration["MailSettings:Timeout"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new System.Net.NetworkCredential(this._configuration["MailSettings:UserName"], this._configuration["MailSettings:Password"]),
            };


            var codes = string.Empty;
            var codeArray = code.ToCharArray();
            for (var i = 0; i < codeArray.Length; i++)
            {
                codes += $"<li> { codeArray[i] } </li>"; 
            }

            var body = "" +
                "<!DOCTYPE html>" +
                "<html lang='pt-br'>" +
                "<head>" +
                    "<meta charset='UTF-8' />" +
                    "<meta http-equiv='X-UA-Compatible' content='IE=edge' />" +
                    "<meta name='viewport' content='width=device-width, initial-scale=1.0' />" +
                    "<title>Document</title>" +
                    "<style>" +
                    ".card {" +
                        "display: block;" +
                        "margin: auto;" +
                        "width: 30em;" +
                        "border: 1px solid rgb(170, 170, 170);" +
                        "box-shadow: 0px 0px 5px rgb(170, 170, 170);" +
                        "border-radius: 5px;" +
                    "}" +
                    ".card-header {" +
                        "padding: 1rem;" +
                        "background-color: #68f76d;" +
                        "color: white;" +
                        "font-size: 1.5em;" +
                        "font-weight: bold;" +
                        "border-radius: 5px 5px 0px 0px;" +
                    "}" +
                    ".card-content {" +
                        "padding: 1rem;" +
                        "background-color: #ffffff;" +
                        "border-radius: 0px 0px 5px 5px;" +
                    "}" +
                    ".nav {" +
                       "padding: 0px;" +
                       "text-align: center;" +
                    "}" +
                    "ul.nav > li {" +
                        "display: inline-block;" +
                        "font-weight: bold;" +
                        "font-size: 2em;" +
                    "}" +
                    ".card-content p {" +
                        "font-size: large;" +
                        "font-family: 'Times New Roman', Times, serif;" +
                        "text-align: inherit;" +
                    "}" +
                    "</style>" +
                "</head>" +
                "<body>" +
                    "<div class='card'>" +
                        "<div class='card-header'>" +
                            "<h2 style='margin: 0px'>Código de Verificação Order API</h2>" +
                        "</div>" +
                        "<div class='card-content'>" +
                            "<p> Order API recebeu uma requisição para usar este email para a alteração de senha. </p>" +
                            "<ul class='nav'>" + codes + "</ul>" +
                            "<p>Informar este código para alterar a senha da sua conta</p>" +
                            "<p>" +
                                "Se você não reconhece esta conta no sistema de gerenciamento de" +
                                "estabelecimentos ou não pediu pela alteração de senha, alguém pode" +
                                "estar se passando por você." +
                            "</p>" +
                        "</div>" +
                    "</div>" +
                "</body>" +
                "</html>";


            mailMessage.From = new MailAddress(this._configuration["MailSettings:From"], this._configuration["MailSettings:DisplayName"]);
            mailMessage.To.Add(new MailAddress(email, name));
            mailMessage.Subject = "Recuperação de Senha";
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);
        }

    }
}