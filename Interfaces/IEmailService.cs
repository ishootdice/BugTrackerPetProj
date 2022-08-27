using System;
namespace BugTrackerPetProj.Interfaces
{
    public interface IEmailService
    {
        void SendInviteEmail(string recieverEmail, string encryptedUserData, bool isAdmin);
    }
}

