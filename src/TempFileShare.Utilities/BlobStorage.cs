using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TempFileShare.DataAccessLayer;
using TempFileShare.Contracts.Interfaces.Utilities;
using TempFileShare.Contracts.DataTransferObjects.Sessions;

namespace TempFileShare.Utilities
{
    public class BlobStorage : IBlobStorage
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;

        public BlobStorage(IConfiguration configuration, ApplicationDBContext context)
        {
            _configuration = configuration;
            _context = context;
            _blobServiceClient = new BlobServiceClient(_configuration["BlobStorageCredentials:blobConnectionString"]);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_configuration["BlobStorageCredentials:containerName"]);
            _ = _blobContainerClient.CreateIfNotExists();
        }

        public async Task<BlobFileDetails> UploadFileAsync(IFormFile file, string sessionId)
        {
            try
            {
                string fileName = Path.GetFileName(file.FileName);
                string blobName = $"{sessionId}/{fileName}";

                BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

                using (Stream stream = file.OpenReadStream())
                {
                    _ = await blobClient.UploadAsync(stream, overwrite: true);
                }

                return new BlobFileDetails { FileName = fileName, BlobURI = blobClient.Uri.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<string> DeleteFileAsync(List<string> files, string sessionId)
        {
            try
            {
                foreach (string file in files)
                {
                    string blobName = $"{sessionId}/{file}";
                    BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);
                    _ = await blobClient.DeleteIfExistsAsync();
                }

                return "200_Files_Successfully_Delete";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<string> DeleteAllFilesAsync(string sessionId)
        {
            try
            {
                await foreach (BlobItem blobItem in _blobContainerClient.GetBlobsAsync(prefix: sessionId))
                {
                    BlobClient blobClient = _blobContainerClient.GetBlobClient(blobItem.Name);
                    _ = await blobClient.DeleteIfExistsAsync();
                }
                return "200_All_Files_Deleted_Successfully";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
