using System.Text.Json;
using TempFileShare.Contracts.Models;
using TempFileShare.Utilities.Encryption;

namespace TempFileShare.Utilities.Token
{
    public class AccessTokenGenerator
    {
        public static string GenerateAccessToken(string sessionId, string username, string validTill, string key, string iv)
        {
            AccessToken accessTokenObject = new()
            {
                SessionId = sessionId,
                Username = username,
                ValidTill = validTill
            };

            string accessTokenJSON = JsonSerializer.Serialize(accessTokenObject);

            return AESEncryption.EncryptString(accessTokenJSON, key, iv);
        }

        //public static string ValidateAccessToken(string accessToken, string key, string iv)
        //{
        //    string accessTokenJSON = AESEncryption.DecryptString(accessToken, key, iv);

        //    AccessToken? accessTokenObject = JsonSerializer.Deserialize<AccessToken>(accessTokenJSON);

        //    if (accessTokenObject != null)
        //    {
        //        return $"{accessTokenObject.SessionId}";
        //    }
        //    return "500_Internal_Server_Error";
        //}
    }
}
