using LearCms.Contexts;
using LearCms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LearCms.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Store
        public async Task<IActionResult> Index()
        {
            // Recupera todos los productos de la base de datos
            // Si no hay productos, ToListAsync() devuelve una lista vacía, no null.
            var products = await _context.Products.ToListAsync();

            // Pasa la lista de productos a la vista
            return View(products);
        }

    }
}
