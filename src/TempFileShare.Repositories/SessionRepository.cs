using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TempFileShare.Contracts.DataTransferObjects.Sessions;
using TempFileShare.Contracts.Interfaces.Repositories;
using TempFileShare.Contracts.Interfaces.Utilities;
using TempFileShare.Contracts.Models;
using TempFileShare.DataAccessLayer;
using TempFileShare.Utilities.Helper;
using TempFileShare.Utilities.Mapper;
using TempFileShare.Utilities.Token;

namespace TempFileShare.Repositories
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0037:Use inferred member name", Justification = "<Pending>")]
    public class SessionRepository(ApplicationDBContext context,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IBlobStorage blobStorage) : ISessionRepository
    {
        private readonly ApplicationDBContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly IBlobStorage _blobStorage = blobStorage;

        public async Task<string> CreateSessionAsync(string username)
        {
            try
            {
                string sessionId = SessionIDGenerator.GenerateSessionId(username);

                ApplicationUser? appUser = await _userManager.FindByNameAsync(username);

                Session? existingSession = await _context.Sessions.FirstOrDefaultAsync(row => row.UserId == appUser.Id);

                if (existingSession != null)
                {
                    return "Status409";
                }
                else
                {
                    string? key = _configuration["AESEncryption:Key"] ?? null;
                    string? iv = _configuration["AESEncryption:IV"] ?? null;

                    ArgumentException.ThrowIfNullOrEmpty(key);
                    ArgumentException.ThrowIfNullOrEmpty(iv);

                    Session session = new()
                    {
                        SessionId = sessionId,
                        SessionStartTime = DateTime.Now,
                        SessionEndTime = DateTime.Now.AddMinutes(30),
                        AccessToken = AccessTokenGenerator.GenerateAccessToken(sessionId, username, DateTime.Now.AddMinutes(30).ToString(), key, iv),
                        UserId = appUser.Id
                    };

                    _ = await _context.Sessions.AddAsync(session);
                    _ = await _context.SaveChangesAsync();

                    return sessionId;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<object?> GetSessionAsync(string username)
        {
            ApplicationUser? appUser = await _userManager.FindByNameAsync(username);
            Session? session = await _context.Sessions.FirstOrDefaultAsync(row => row.UserId == appUser.Id);
            if (session == null)
            {
                return session;
            }

            return new { Session = session.SessionId, ValidTill = session.SessionEndTime };
        }

        public async Task<object?> GetAccessToken(string username)
        {
            ApplicationUser? appUser = await _userManager.FindByNameAsync(username);
            Session? session = await _context.Sessions.FirstOrDefaultAsync(row => row.UserId == appUser.Id);
            if (session == null)
            {
                return null;
            }

            return new { AccessToken = session.AccessToken, ValidTill = session.SessionEndTime };
        }

        public async Task<string> UploadFilesAsync(List<IFormFile> files, string sessionId)
        {
            Session? session = await _context.Sessions.FirstOrDefaultAsync(row => row.SessionId == sessionId);

            if (session == null)
            {
                return "404_SessionNotFound";
            }

            try
            {
                foreach (IFormFile file in files)
                {
                    if (file != null)
                    {
                        BlobFileDetails blobFileDetails = await _blobStorage.UploadFileAsync(file, sessionId);
                        Files fileDetails = new()
                        {
                            FileName = blobFileDetails.FileName ?? "",
                            FilePath = blobFileDetails.BlobURI?.ToString() ?? "",
                            SessionId = sessionId
                        };

                        _ = await _context.Files.AddAsync(fileDetails);
                        _ = await _context.SaveChangesAsync();
                    }
                }
                return "Files Uploaded Successfully";
            }
            catch (Exception)
            {
                return "500_InternalServerError";
            }

        }

        public async Task<string> ListAllFilesAsync(string sessionId)
        {
            Session? session = await _context.Sessions.FirstOrDefaultAsync(session => session.SessionId == sessionId);
            if (session != null)
            {
                List<Files> files = await _context.Files.Where(row => row.SessionId == sessionId).ToListAsync();
                List<DBFilesDetails> dbFiles = files.Select(AutoMap.MapToDBFilesDetails).ToList();

                if (files.Count <= 0 || files == null)
                {
                    return "404_NoFilesFound";
                }
                return JsonConvert.SerializeObject(dbFiles);
            }
            return "404_NoSessionFound";
        }

        public async Task<string> DeleteFilesAsync(List<string> files, string sessionId)
        {
            string response = await _blobStorage.DeleteFileAsync(files, sessionId);

            if (response == "200_Files_Successfully_Delete")
            {
                List<Files> filesInDB = await _context.Files.Where(row => files.Contains(row.FileName) && row.SessionId == sessionId).ToListAsync();
                if (filesInDB.Any())
                {
                    _context.Files.RemoveRange(filesInDB);
                    _ = await _context.SaveChangesAsync();
                    return "200_Selected_Files_Deleted_Successfully";
                }
            }
            return "500_Internal_Server_Error";
        }

        public async Task<string> DeleteAllFilesAsync(string sessionId)
        {
            List<Files> files = await _context.Files.Where(row => row.SessionId == sessionId).ToListAsync();
            if (!(files.Count <= 0 && files == null))
            {
                _context.Files.RemoveRange(files);
                _ = await _context.SaveChangesAsync();

                string response = await _blobStorage.DeleteAllFilesAsync(sessionId);
                if (response == "200_All_Files_Deleted_Successfully")
                {
                    return "200_All_Files_Deleted_Successfully";
                }
            }
            return "500_Internal_Server_Error";
        }

        public async Task<string> DisbandSession(string sessionId)
        {
            Session? session = await _context.Sessions.FirstOrDefaultAsync(row => row.SessionId == sessionId);

            if (session != null)
            {
                _ = _context.Sessions.Remove(session);
                _ = await _context.SaveChangesAsync();
                string response = await _blobStorage.DeleteAllFilesAsync(sessionId);
                if (response == "200_All_Files_Deleted_Successfully")
                {
                    return "200_Session_Successfully_Disbanded";
                }
            }
            return "404_Session_Not_Found";
        }
    }
}
