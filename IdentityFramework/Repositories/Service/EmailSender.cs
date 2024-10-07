using IdentityFramework.Repositories.Interface;
using IdentityFramework.ViewModel.Email;
using System.Net;
using System.Net.Mail;

namespace IdentityFramework.Repositories.Service
{
    public class EmailSender : IEmailSender
    {
        // getting datafrom appsettings.json

        private readonly IConfiguration configuration;
        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // Email fuction
        public async Task<bool> EmailSendAsync(string email, string subject, string message)
        {
            bool status = false;
            try
            {
                GetEmailSettings getemailsettings = new GetEmailSettings()
                {
                    SecretKey = configuration.GetValue<string>("AppSettings:SecretKey") ?? string.Empty,
                    From = configuration.GetValue<string>("AppSettings:EmailSettings:From") ?? string.Empty,
                    SmtpServer = configuration.GetValue<string>("AppSettings:EmailSettings:SmtpServer") ?? string.Empty,
                    Port = configuration.GetValue<int>("AppSettings:EmailSettings:Port"),
                    EnableSSL = configuration.GetValue<bool>("AppSettings:EmailSettings:EnableSSL")
                };

                MailMessage mailmessage = new MailMessage()
                {
                    From = new MailAddress(getemailsettings.From),
                    Subject = subject,
                    Body = message,
                    BodyEncoding = System.Text.Encoding.ASCII,
                    IsBodyHtml = true
                };
                mailmessage.To.Add(email);

                SmtpClient smtpclient = new SmtpClient(getemailsettings.SmtpServer)
                {
                    Port = getemailsettings.Port,
                    Credentials = new NetworkCredential(getemailsettings.From, getemailsettings.SecretKey),
                    EnableSsl = getemailsettings.EnableSSL
                };

                await smtpclient.SendMailAsync(mailmessage);
                status = true;

            }catch(Exception)
            {
                status = false;
            }

            return status;
        }
    }
}
