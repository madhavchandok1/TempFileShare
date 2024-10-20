using TempFileShare.Utilities.Encode;

namespace TempFileShare.Utilities.Helper
{
    public class SessionIDGenerator
    {
        public static string GenerateSessionId(string username)
        {
            DateTime dateTime = DateTime.Now;

            string date = dateTime.ToString("dd");
            string month = dateTime.ToString("MM");
            string year = dateTime.ToString("yyyy");
            string timestamp = dateTime.ToString("HHmmss");

            string baseSessionId = $"{username}_{date}{month}{year}{timestamp}";

            return Base64EncodeDecode.Base64Encoder(baseSessionId);
        }
    }
}
