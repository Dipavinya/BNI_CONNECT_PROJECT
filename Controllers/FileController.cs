using BniConnect.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BniConnect.Controllers
{
    public class FileController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsDirectory))
                {
                    return NotFound("No files available in uploads directory");
                }

                var files = Directory.GetFiles(uploadsDirectory);

                var fileUrls = files.Select(file => new FileViewModel
                {
                    FileName = Path.GetFileName(file),
                    FileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{Path.GetFileName(file)}"
                }).ToList();

                return View(fileUrls);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Error while getting files from uploads directory");
                return View(new FileViewModel { });
            }
        }

        [HttpPost("delete")]
        public IActionResult DeleteFile(string fileName)
        {
            try
            {
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                var filePath = Path.Combine(uploadsDirectory, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                System.IO.File.Delete(filePath);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the file", error = ex.Message });
            }
        }

        public IActionResult ViewFile(string fileName)
        {
            try
            {
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                var filePath = Path.Combine(uploadsDirectory, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                var contentType = GetContentType(filePath);
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                return File(fileStream, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the file", error = ex.Message });
            }
        }

        private string GetContentType(string filePath)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream"; 
            }
            return contentType;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                Directory.CreateDirectory(uploadsDirectory);

                var uniqueFileName = file.FileName;
                var uploadPath = Path.Combine(uploadsDirectory, uniqueFileName);

                using (var stream = new FileStream(uploadPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";
                var fileName = uniqueFileName;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Error while uploading file");
                return View("Index");
            }

        }

    }
}
