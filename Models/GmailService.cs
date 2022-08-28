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

        public MimeMessage GeneratePasswordResetMail(string recieverEmail, string userId, string userName)
        {
            MimeMessage message = new MimeMessage();
            
            string appUrl = "http://localhost:53741/Account/ChangePassword?encryptedUserId=";
            message.From.Add(new MailboxAddress("Моя компанія", "voitenkodevpost@gmail.com"));
            message.To.Add(new MailboxAddress(userName, recieverEmail));
            message.Subject = "Зміна паролю";
            message.Body = new BodyBuilder()
            {
                HtmlBody = "<div style=\"color: green;\"> Повідомлення від MailKit" +
                    $"</hr><a href=\"{appUrl}{userId}\">Congirm</a></div>"
            }.ToMessageBody();

            return message;
        }

        public void Send(MimeMessage message)
        {
            using (SmtpClient client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("voitenkodevpost@gmail.com", "glfpmjnxfjxzhgxh");
                client.Send(message);

                client.Disconnect(true);
                _logger.LogInformation("Повідомлення відправлено успішно");
            }
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
                    appUrl = "http://localhost:53741/Administration/CompanyInviteConfirmation?userData=";
                    message.Subject = "Запрошення до компанії";
                }
                else
                {
                    appUrl = "http://localhost:53741/Administration/InviteConfirmation?userData=";
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

