using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TempFileShare.Contracts.Models
{
    [Table("Files")]
    public class Files
    {
        [Key]
        [Required]
        public string FileId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public required string FileName { get; set; }

        [Required]
        public required string FilePath { get; set; }

        [Required]
        public required string SessionId { get; set; }

        public Session? Session { get; set; }
    }
}
