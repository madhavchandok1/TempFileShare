namespace TempFileShare.Contracts.Models
{
    public class AccessToken
    {
        public string UniqueID { get; set; } = Guid.NewGuid().ToString();

        public string? SessionId { get; set; }

        public string? Username { get; set; }

        public string? ValidTill { get; set; }
    }
}
