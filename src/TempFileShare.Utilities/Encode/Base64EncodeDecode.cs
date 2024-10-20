using System.Text;

namespace TempFileShare.Utilities.Encode
{
    public static class Base64EncodeDecode
    {
        public static string Base64Encoder(string input)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            string base64encoded = Convert.ToBase64String(byteArray);
            return base64encoded;
        }

        public static string Base64Decoder(string input)
        {
            byte[] byteArray = Convert.FromBase64String(input);
            string base64decoded = Encoding.UTF8.GetString(byteArray);
            return base64decoded;
        }
    }
}
