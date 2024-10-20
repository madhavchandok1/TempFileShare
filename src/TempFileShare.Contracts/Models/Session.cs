using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TempFileShare.Contracts.Models
{
    [Table("Sessions")]
    public class Session
    {
        [Key]
        [Required]
        public required string SessionId { get; set; }

        [Required]
        public DateTime SessionStartTime { get; set; }

        [Required]
        public DateTime SessionEndTime { get; set; }

        [Required]
        public required string AccessToken { get; set; }

        [Required]
        public required string UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public List<Files> Files { get; set; } = [];
    }
}
