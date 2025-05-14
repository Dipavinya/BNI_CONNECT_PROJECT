using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BniConnect.Controllers
{
    [Route("logs")]
    public class LogController : Controller
    {
        [HttpGet("latest")]
        public IActionResult GetLatestLog()
        {            
            var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logs");

            try
            {
                var logFiles = Directory.GetFiles(logsDirectory, "log-*.txt");

                var latestLogFile = logFiles
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime) 
                    .FirstOrDefault();

                if (latestLogFile == null)
                {
                    return NotFound("No log files found.");
                }

                using (var fs = new FileStream(latestLogFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs))
                {
                    var logContent = reader.ReadToEnd();
                    return Content(logContent, "text/plain");
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex,"Error while loggin log");
                return StatusCode(503, "Log file is currently in use, please try again later.");
            }

        }
    }
}
