using Microsoft.AspNetCore.Http;

namespace TempFileShare.Contracts.Interfaces.Repositories
{
    public interface ISessionRepository
    {
        public Task<string> CreateSessionAsync(string username);

        public Task<object?> GetSessionAsync(string username);

        public Task<object?> GetAccessToken(string username);

        public Task<string> UploadFilesAsync(List<IFormFile> files, string sessionId);

        public Task<string> ListAllFilesAsync(string sessionId);

        public Task<string> DeleteFilesAsync(List<string> files, string sessionId);

        public Task<string> DeleteAllFilesAsync(string sessionId);

        public Task<string> DisbandSession(string sessionId);
    }
}
