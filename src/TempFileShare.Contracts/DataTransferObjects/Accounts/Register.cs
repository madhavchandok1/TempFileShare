using System.ComponentModel.DataAnnotations;

namespace TempFileShare.Contracts.DataTransferObjects.Accounts
{
    public class Register
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
