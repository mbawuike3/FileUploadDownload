using fileUploadDownload.Models;
using Microsoft.EntityFrameworkCore;

namespace fileUploadDownload.DAL
{
    public class FileUploadDbContext : DbContext
    {
        public FileUploadDbContext(DbContextOptions<FileUploadDbContext> options) : base(options) 
        {
            
        }
        public DbSet<FileUpload> FileUploads { get; set; }
    }
}
