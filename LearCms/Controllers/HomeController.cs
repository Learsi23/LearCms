using LearCms.Contexts;
using LearCms.DTOs;    // 🚨 CLAVE: Necesitas el DTO para el mapeo
using LearCms.Entities; // Necesitas la entidad para consultar la DB
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq; // Necesario para el método Select

namespace LearCms.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Store (Listado de productos para la tienda)
        public async Task<IActionResult> Index()
        {
            // 1. Recupera la lista de Entidades (ProductEntity)
            var products = await _context.Products.ToListAsync();

            // 2. Mapea la lista de Entidades a DTOs
            var productDtos = products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl // Incluye la URL de la imagen
            }).ToList();

            // 3. Pasa la lista de DTOs a la vista (por defecto, será Views/Home/Index.cshtml)
            // NOTA: Si esta acción es para la vista 'Tienda.cshtml', debes especificarla: return View("Tienda", productDtos);
            return View(productDtos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}