using Microsoft.AspNetCore.Identity;

namespace TempFileShare.Contracts.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Session? Session { get; set; }
    }
}
