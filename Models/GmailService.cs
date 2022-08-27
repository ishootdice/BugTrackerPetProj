using System;
using BugTrackerPetProj.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace BugTrackerPetProj.Models
{
    public class GmailService : IEmailService
    {
        private readonly ILogger<GmailService> _logger;

        public GmailService(ILogger<GmailService> logger)
        {
            _logger = logger;
        }

        public void SendInviteEmail(string recieverEmail, string encryptedUserData, bool isAdmin)
        {
     
            try
            {
                string appUrl = null;

                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress("Моя компанія", "voitenkodevpost@gmail.com"));
                message.To.Add(new MailboxAddress("user", recieverEmail));

                if (isAdmin)
                {
                    appUrl = "https://localhost:7109/Administration/CompanyInviteConfirmation?userData=";
                    message.Subject = "Запрошення до компанії";
                }
                else
                {
                    appUrl = "https://localhost:7109/Administration/InviteConfirmation?userData=";
                    message.Subject = "Запрошення на проект";
                }


                message.Body = new BodyBuilder() { HtmlBody = "<div style=\"color: green;\"> Повідомлення від MailKit" +
                    $"</hr><a href=\"{appUrl}{encryptedUserData}\">Congirm</a></div>" }.ToMessageBody();
                using(SmtpClient client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 465, true);
                    client.Authenticate("voitenkodevpost@gmail.com", "glfpmjnxfjxzhgxh");
                    client.Send(message);

                    client.Disconnect(true);
                    _logger.LogInformation("Повідомлення відправлено успішно");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.GetBaseException().Message);
            }
        }


    }
}

