using System;
using System.Security.Cryptography;
using System.Text;
using QBR.Infrastructure.Extensions;
using QBR.Infrastructure.Interfaces;

namespace QBR.RecordManagerModule.Services
{
    class SecurityCodeService : ISecurityCodeService
    {
        public string CalculateSecurityCode(string fileContent)
        {
            var decoder = (HashAlgorithm)Activator.CreateInstance(Type.GetType(Properties.Settings.Default.CryptoProvider));
            var securitycode = decoder.ComputeHash(fileContent.GetBytes());

            var sBuilder = new StringBuilder();
            for (var i = 0; i < securitycode.Length; i++)
            {
                sBuilder.Append(securitycode[i].ToString("X2"));
            }

            return sBuilder.ToString();
        }
    }
}
