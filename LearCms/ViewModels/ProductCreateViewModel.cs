using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LearCms.ViewModels
{
    public class ProductCreateViewModel
    {
        // Propiedades de ProductEntity que quieres crear/editar
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // 🆕 NUEVO: Propiedad para el archivo (solo para subirlo)
        [Display(Name = "Imagen del Producto")]
        public IFormFile? ImageFile { get; set; }
    }
}