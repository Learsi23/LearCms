using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LearCms.Contexts;
using LearCms.Entities;
using Microsoft.AspNetCore.Http; // Necesario para usar extensiones de Session

namespace LearCms.Controllers
{
    public class CartItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        // 🚨 CAMBIO: Se añade la constante para la clave de sesión, necesaria para GetSessionId.
        private const string CartSessionId = "CartSessionId";

        public CartItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CartItem
        public async Task<IActionResult> Index()
        {
            var sessionId = GetSessionId();
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.SessionId == sessionId)
                .ToListAsync();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity)
        {
            // La lógica de adición/actualización ya estaba correcta, se mantiene.

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Asegura que la cantidad no sea negativa
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "La cantidad debe ser mayor que cero." });
            }

            var sessionId = GetSessionId();
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.ProductId == productId);

            if (cartItem == null)
            {
                // Producto nuevo en el carrito
                _context.CartItems.Add(new CartItemEntity
                {
                    CartItemId = Guid.NewGuid(),
                    SessionId = sessionId,
                    ProductId = productId,
                    Quantity = quantity
                });
            }
            else
            {
                // Producto ya existente, se actualiza la cantidad
                cartItem.Quantity += quantity;
                _context.Update(cartItem);
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Producto '{product.Name}' añadido al carrito." });
        }

        // 🚨 CAMBIO: Nueva acción para devolver el componente de vista del carrito actualizado.
        // Utiliza ViewComponentResult para renderizar directamente el componente.
        [HttpGet]
        public IActionResult CartCount()
        {
            // Invoca y renderiza el CartViewComponent.
            return ViewComponent("Cart");
        }

        private string GetSessionId()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(CartSessionId)))
            {
                HttpContext.Session.SetString(CartSessionId, Guid.NewGuid().ToString());
            }
            return HttpContext.Session.GetString(CartSessionId);
        }

        // GET: CartItem/Details/5
        // ... (resto de acciones sin cambios)

        // GET: CartItem/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItemEntity = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CartItemId == id);
            if (cartItemEntity == null)
            {
                return NotFound();
            }

            return View(cartItemEntity);
        }

        // GET: CartItem/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }

        // POST: CartItem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartItemId,SessionId,ProductId,Quantity")] CartItemEntity cartItemEntity)
        {
            if (ModelState.IsValid)
            {
                cartItemEntity.CartItemId = Guid.NewGuid();
                _context.Add(cartItemEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", cartItemEntity.ProductId);
            return View(cartItemEntity);
        }

        // GET: CartItem/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItemEntity = await _context.CartItems.FindAsync(id);
            if (cartItemEntity == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", cartItemEntity.ProductId);
            return View(cartItemEntity);
        }

        // POST: CartItem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CartItemId,SessionId,ProductId,Quantity")] CartItemEntity cartItemEntity)
        {
            if (id != cartItemEntity.CartItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartItemEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemEntityExists(cartItemEntity.CartItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", cartItemEntity.ProductId);
            return View(cartItemEntity);
        }

        // GET: CartItem/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItemEntity = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CartItemId == id);
            if (cartItemEntity == null)
            {
                return NotFound();
            }

            return View(cartItemEntity);
        }

        // POST: CartItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cartItemEntity = await _context.CartItems.FindAsync(id);
            if (cartItemEntity != null)
            {
                _context.CartItems.Remove(cartItemEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartItemEntityExists(Guid id)
        {
            return _context.CartItems.Any(e => e.CartItemId == id);
        }
    }
}