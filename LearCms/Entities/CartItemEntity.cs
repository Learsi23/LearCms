using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearCms.Entities
{
    public class CartItemEntity
    {
        [Key]
        public Guid CartItemId { get; set; } = Guid.NewGuid();

        public string? SessionId { get; set; }

        public Guid ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductEntity? Product { get; set; }

        public int Quantity { get; set; }
    }
}
