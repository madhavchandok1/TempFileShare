using TempFileShare.Contracts.Models;

namespace TempFileShare.Contracts.Interfaces.Utilities
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);
    }
}
