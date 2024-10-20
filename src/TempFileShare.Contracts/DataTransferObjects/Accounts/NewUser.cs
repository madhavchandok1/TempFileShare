using System.ComponentModel.DataAnnotations;

namespace TempFileShare.Contracts.DataTransferObjects.Accounts
{
    public class NewUser
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Token { get; set; }
    }
}
