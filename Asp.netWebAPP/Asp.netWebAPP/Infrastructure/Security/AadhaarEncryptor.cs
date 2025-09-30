using System.Security.Cryptography;
using System.Text.Json;
using System.Text;

namespace Asp.netWebAPP.Infrastructure.Security
{
    public class Encryptor
    {
        public static string EncryptWithPublicKeyString(string plainText, string publicKeyPemOrJson)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                throw new ArgumentException("Input text cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(publicKeyPemOrJson))
                throw new ArgumentException("Public key cannot be null or empty.");

            //  Extract clean PEM content
            string cleanPem = ExtractPem(publicKeyPemOrJson);

            //  Convert to byte[] and import into RSA
            byte[] keyBytes = Convert.FromBase64String(cleanPem);

            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);

            //  Encrypt using OAEP SHA-1 (as required by ABDM)
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA1);

            return Convert.ToBase64String(encryptedBytes);
        }

        private static string ExtractPem(string input)
        {
            string cleanPem = input;

            //  JSON format { "publicKey": "...." }
            if (input.TrimStart().StartsWith("{"))
            {
                var json = JsonDocument.Parse(input);
                cleanPem = json.RootElement.GetProperty("publicKey").GetString() ?? "";
            }

            //  PEM with headers
            cleanPem = cleanPem
                .Replace("-----BEGIN PUBLIC KEY-----", "", StringComparison.OrdinalIgnoreCase)
                .Replace("-----END PUBLIC KEY-----", "", StringComparison.OrdinalIgnoreCase)
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace(" ", "")
                .Trim();

            return cleanPem;
        }
    }

}
