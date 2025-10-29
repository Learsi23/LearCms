using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LearCms.ViewModels
{
    public class ProductEditViewModel
    {
        // CLAVE: ID del producto que se está editando (campo oculto)
        [HiddenInput]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        // CLAVE: URL de la imagen actual (para mostrarla y mantenerla si no se sube una nueva)
        [HiddenInput]
        public string? ExistingImageUrl { get; set; }

        // Propiedad para la subida de un NUEVO archivo (es opcional, por eso es nullable '?' )
        [Display(Name = "Reemplazar Imagen")]
        public IFormFile? NewImageFile { get; set; }
    }
}