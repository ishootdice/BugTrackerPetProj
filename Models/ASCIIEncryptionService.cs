using System;
using System.Security.Cryptography;
using System.Text;
using BugTrackerPetProj.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTrackerPetProj.Models
{
    public class ASCIIEncryptionService : IEncryptionService
    {
        private readonly ILogger<ASCIIEncryptionService> _logger;

        public ASCIIEncryptionService(ILogger<ASCIIEncryptionService> logger)
        {
            _logger = logger;
        }
        public string Decrypt(string dataToDecrypt)
        {
            byte[] data;
            string decrypted;
            try
            {
                data = Convert.FromBase64String(dataToDecrypt);
                decrypted = ASCIIEncoding.ASCII.GetString(data);
            }
            catch (FormatException fe)
            {
                _logger.LogError(fe.GetBaseException().ToString());
                decrypted = String.Empty;
            }

            return decrypted;
        }

        public string Encrypt(string dataToEncrypt)
        {
            byte[] dataInBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(dataToEncrypt);
            string result = Convert.ToBase64String(dataInBytes);
            return result;
        }
    }
}

