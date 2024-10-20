using System.Security.Cryptography;

namespace TempFileShare.Utilities.Encryption
{
    public class AESEncryption
    {
        public static string EncryptString(string plainText, string key, string iv)
        {
            using Aes aes = Aes.Create();
            aes.Key = Convert.FromBase64String(key); ;
            aes.IV = Convert.FromBase64String(iv);

            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using StreamWriter streamWriter = new(cryptoStream);
            streamWriter.Write(plainText);

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static string DecryptString(string cipherText, string key, string iv)
        {
            byte[] buffer = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);

            using MemoryStream memoryStream = new(buffer);
            using CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader streamReader = new(cryptoStream);

            return streamReader.ReadToEnd();
        }
    }
}
