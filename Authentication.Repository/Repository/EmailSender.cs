using Authentication.Models.ViewModels;
using Authentication.Repository.Repository.IRepository;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Repository.Repository
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSetting _emailSettingConfig;
        public EmailSender(IOptions<EmailSetting> emailSettingConfig)
        {
            _emailSettingConfig = emailSettingConfig.Value;

        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }

        public Task SendEmailAsyncWithBody(string to, string subject, string body, bool isHtml = false)
        {
            var mailMessage = new MailMessage { From = new MailAddress(_emailSettingConfig.email) };

            mailMessage.To.Add(new MailAddress(to));

            mailMessage.Subject = subject;

            mailMessage.Body = body;

            mailMessage.IsBodyHtml = isHtml;

            var smtpClient = new SmtpClient
            {
                Host = _emailSettingConfig.HostName,
                Port = _emailSettingConfig.PortNumber,
                EnableSsl = _emailSettingConfig.EnableSsl,//true,
                UseDefaultCredentials = _emailSettingConfig.UseDefaultCredentials,//false,
                Credentials = new NetworkCredential(_emailSettingConfig.email, _emailSettingConfig.password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            try
            {
                smtpClient.Send(mailMessage);
                //  return Task.CompletedTask;
            }
            catch (Exception e)
            {
                // TODO: handle exception              
                throw new InvalidOperationException(e.Message);

            }
            return Task.CompletedTask;
        }
    }
}
