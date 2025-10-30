using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LearCms.Contexts;
using LearCms.Entities;
using Microsoft.AspNetCore.Http;

namespace LearCms.Controllers
{
    public class CartItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionId = "CartSessionId";

        public CartItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================================
        // 🛒 VIEW: Mostrar carrito actual
        // ================================
        public async Task<IActionResult> Index()
        {
            var sessionId = GetSessionId();

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.SessionId == sessionId)
                .ToListAsync();

            return View(cartItems);
        }

        // ===========================================
        // ➕ ADD: Añadir producto al carrito
        // ===========================================
        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity)
        {
            if (quantity <= 0)
                return Json(new { success = false, message = "Quantity must be greater than zero." });

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return Json(new { success = false, message = "Product not found." });

            var sessionId = GetSessionId();
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItemEntity
                {
                    CartItemId = Guid.NewGuid(),
                    SessionId = sessionId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
                _context.Update(cartItem);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = $"Added {quantity} x {product.Name} to cart." });
        }

        // ===========================================
        // 🔄 UPDATE: Cambiar cantidad de un ítem
        // ===========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(Guid cartItemId, int quantity)
        {
            if (quantity <= 0)
                return RedirectToAction(nameof(Index));

            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return NotFound();

            cartItem.Quantity = quantity;
            _context.Update(cartItem);
            await _context.SaveChangesAsync();

            // ✅ Regresar al carrito sin mostrar la vista UpdateQuantity
            return RedirectToAction(nameof(Index));
        }

        // ===========================================
        // ❌ REMOVE: Eliminar producto del carrito
        // ===========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return NotFound();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===========================================
        // 🧹 CLEAR: Vaciar carrito completo
        // ===========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var sessionId = GetSessionId();

            var items = _context.CartItems.Where(c => c.SessionId == sessionId);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===========================================
        // 🧾 CART COUNT: Componente del carrito
        // ===========================================
        [HttpGet]
        public IActionResult CartCount()
        {
            return ViewComponent("Cart");
        }

        // ===========================================
        // ⚙️ MÉTODO PRIVADO: Obtener/crear SessionId
        // ===========================================
        private string GetSessionId()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(CartSessionId)))
            {
                HttpContext.Session.SetString(CartSessionId, Guid.NewGuid().ToString());
            }
            return HttpContext.Session.GetString(CartSessionId)!;
        }
    }
}
