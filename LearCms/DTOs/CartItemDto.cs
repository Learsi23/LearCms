using System;

namespace LearCms.Models
{
    public class CartItemDto
    {
        public Guid CartItemId { get; set; }

        public string? SessionId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        // Puedes incluir información del producto si la necesitas en la vista
        public ProductDto? Product { get; set; }
    }
}
