using System;
using MimeKit;

namespace BugTrackerPetProj.Interfaces
{
    public interface IEmailService
    {
        void SendInviteEmail(string recieverEmail, string encryptedUserData, bool isAdmin);

        public MimeMessage GeneratePasswordResetMail(string recieverEmail, string userId, string userName);

        public void Send(MimeMessage message);
    }
}

