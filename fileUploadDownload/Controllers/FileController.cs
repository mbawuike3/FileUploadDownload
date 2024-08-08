using fileUploadDownload.DAL;
using fileUploadDownload.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace fileUploadDownload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly FileUploadDbContext _db;

        public FileController(FileUploadDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("UploadFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationToken)
        {
            var result = await WriteFile(file);
            if(string.IsNullOrEmpty(result))
            {
                return BadRequest("Error uploading file");
            }
            var fileUrl = Url.Content($"~/Upload/Files/{result}");
            var fileRecord = new FileUpload
            {
                FileName = result,
                FileUrl = fileUrl,
            };
            _db.FileUploads.Add(fileRecord);
            await _db.SaveChangesAsync();
            return Ok(result);
        }
        private async Task<string> WriteFile(IFormFile file)
        {
            string fileName = "";
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                fileName = DateTime.Now.Ticks.ToString() + extension;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files");
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                var exactpath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", fileName);
                using (var fileStream = new FileStream(exactpath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return fileName;
        }
        [HttpGet]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", fileName);
            var provider = new FileExtensionContentTypeProvider();
            if(!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filepath);
            return File(bytes, contentType, Path.GetFileName(fileName));
        }
    }
}
