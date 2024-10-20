using System.ComponentModel.DataAnnotations;

namespace TempFileShare.Contracts.DataTransferObjects.Accounts
{
    public class Login
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
