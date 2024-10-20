using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TempFileShare.Contracts.DataTransferObjects.Sessions;
using TempFileShare.Contracts.Interfaces.Repositories;
using TempFileShare.Utilities.Token;

namespace TempFileShare.Controllers
{
    [ApiController]
    [Route("/session")]
    [Authorize]
    public class SessionController(ISessionRepository sessionRepository) : ControllerBase
    {
        private readonly ISessionRepository _sessionRepository = sessionRepository;


        //GET: /session/createSession
        [HttpGet]
        [Route("/createSession")]
        public async Task<IActionResult> CreateSessionId()
        {
            string? username = User.GetUsername();
            string? sessionId = null;

            if (username != null)
            {
                sessionId = await _sessionRepository.CreateSessionAsync(username);
            }
            if (sessionId == "Status409")
            {
                return StatusCode(409, new { message = "Session already exists. Please use the existing session." });
            }
            else if (sessionId == null)
            {
                return StatusCode(500, new { message = "Internal Server Error. Please use after sometimes." });
            }
            else
            {
                return StatusCode(200, new SessionDetails
                {
                    SessionId = sessionId,
                    ValidTill = DateTime.Now.AddMinutes(30)
                });

            }
        }

        //GET: /session/getSessionDetails
        [HttpGet]
        [Route("/getSessionDetails")]
        public async Task<IActionResult> GetSessionDetails()
        {
            string? username = User.GetUsername();
            object? sessionId = null;

            if (username != null)
            {
                sessionId = await _sessionRepository.GetSessionAsync(username);
            }

            if (sessionId == null)
            {
                return StatusCode(404, new { message = "No session exists. Please create a session first" });
            }

            return StatusCode(200, sessionId);
        }

        //GET: /session/getAccessToken
        [HttpGet]
        [Route("/getAccessToken")]
        public async Task<IActionResult> GetAccessToken()
        {
            string? username = User.GetUsername();
            object? accessToken = null;

            if (username != null)
            {
                accessToken = await _sessionRepository.GetAccessToken(username);
            }
            if (accessToken == null)
            {
                return StatusCode(404, new { message = "No session exists. Please create a session first." });
            }
            return StatusCode(200, accessToken);
        }

        //POST: /session/uploadFiles/{sessionId}
        [RequestFormLimits(MultipartBodyLengthLimit = 50 * 1024 * 1024)]
        [DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        [HttpPost]
        [Route("/uploadFiles/{sessionId}")]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files, [FromRoute] string sessionId)
        {
            if (files == null || files.Count == 0)
            {
                return StatusCode(400, new { message = "No files were uploaded" });
            }
            else if (sessionId == null)
            {
                return StatusCode(400, new { message = "No Session ID was found " });
            }

            string response = await _sessionRepository.UploadFilesAsync(files, sessionId);

            if (response == "404_SessionNotFound")
            {
                return StatusCode(400, new { message = "No session exists with the specified session ID. Please create one and try again later!!!" });
            }
            else if (response == "500_InternalServerError")
            {
                return StatusCode(500, new { message = "Internal Server Error. Please try again later!!!" });
            }
            else
            {
                return StatusCode(200, new { message = "All files uploaded successfully" });
            }
        }

        //GET: /session/listFiles/{sessionId}
        [HttpGet]
        [Route("/listFiles/{sessionId}")]
        public async Task<IActionResult> ListAllFiles([FromRoute] string sessionId)
        {
            if (sessionId == null)
            {
                return StatusCode(400, new { message = "No Session ID was found." });
            }

            string response = await _sessionRepository.ListAllFilesAsync(sessionId);

            if (response == "404_NoSessionFound")
            {
                return StatusCode(404, new { message = "No session exists with the specified session ID. Please create one and try again later!!!" });
            }

            if (response == "404_NoFilesFound")
            {
                return StatusCode(404, new { message = "No files found during this session" });
            }

            List<DBFilesDetails>? files = JsonConvert.DeserializeObject<List<DBFilesDetails>>(response);

            return StatusCode(200, new { Files = files });
        }

        [HttpDelete]
        [Route("/deleteFile/{sessionId}")]
        public async Task<IActionResult> DeleteFiles([FromBody] List<string> files, [FromRoute] string sessionId)
        {
            if (sessionId == null)
            {
                return StatusCode(400, new { message = "No Session ID was found " });
            }

            if (files.Count == 0)
            {
                return StatusCode(400, new { message = "No files info was found in request body. Please try again!!!" });
            }

            string response = await _sessionRepository.DeleteFilesAsync(files, sessionId);

            if (response == "500_Internal_Server_Error")
            {
                return StatusCode(400, new { message = "Internal Server Error. Please try again later!!!" });
            }

            return StatusCode(200, new { message = "Selected Files Deleted Successfully." });
        }

        [HttpDelete]
        [Route("/deleteAll/{sessionId}")]
        public async Task<IActionResult> DeleteAllFiles([FromRoute] string sessionId)
        {
            if (sessionId == null)
            {
                return StatusCode(400, new { message = "No Session ID was found " });
            }

            string response = await _sessionRepository.DeleteAllFilesAsync(sessionId);

            if (response == "500_Internal_Server_Error")
            {
                return StatusCode(400, new { message = "Internal Server Error. Please try again later!!!" });
            }

            return StatusCode(200, new { message = "Selected Files Deleted Successfully." });
        }

        [HttpDelete]
        [Route("/disbandSession/{sessionId}")]
        public async Task<IActionResult> DisbandSession([FromRoute] string sessionId)
        {
            if (sessionId == null)
            {
                return StatusCode(400, new { message = "No Session ID was found " });
            }

            string response = await _sessionRepository.DisbandSession(sessionId);

            if (response == "404_Session_Not_Found")
            {
                return StatusCode(404, new { message = "No session exists with the specified session ID. Please create one and try again later!!!" });
            }

            return StatusCode(200, new { message = "Session Successfully Disbanded" });
        }
    }
}
