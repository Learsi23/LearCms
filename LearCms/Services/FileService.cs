using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace LearCms.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) return string.Empty;

            // Define la ruta donde se guardará el archivo (e.g., wwwroot/images/productos)
            var uploadPath = Path.Combine(_env.WebRootPath, folderName);

            // Crea el directorio si no existe
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Genera un nombre de archivo único para evitar colisiones
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Devuelve la URL relativa que se guardará en la base de datos
            return $"/{folderName.Replace('\\', '/')}/{fileName}";
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            // Convierte la ruta relativa a la ruta física
            var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}