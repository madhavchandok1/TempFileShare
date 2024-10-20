using Microsoft.AspNetCore.Http;
using TempFileShare.Contracts.DataTransferObjects.Sessions;

namespace TempFileShare.Contracts.Interfaces.Utilities
{
    public interface IBlobStorage
    {
        public Task<BlobFileDetails> UploadFileAsync(IFormFile file, string sessionId);
        public Task<string> DeleteFileAsync(List<string> files, string sessionId);
        public Task<string> DeleteAllFilesAsync(string sessionId);
    }
}
