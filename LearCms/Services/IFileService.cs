namespace LearCms.Services
{
    public interface IFileService
    {
        // Devuelve la ruta relativa que guardaremos en la DB (e.g., "/images/productos/archivo.jpg")
        Task<string> SaveFileAsync(IFormFile file, string folderName);
        void DeleteFile(string filePath);
    }
}