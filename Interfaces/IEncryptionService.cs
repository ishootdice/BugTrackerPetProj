using System;
namespace BugTrackerPetProj.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string dataToEncrypt);

        string Decrypt(string dataToDecrypt);
    }
}

