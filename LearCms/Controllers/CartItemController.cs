using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LearCms.Contexts;
using LearCms.Entities;

namespace LearCms.Controllers
{
    public class CartItemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CartItem
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CartItems.Include(c => c.Product);
            return View(await applicationDbContext.ToListAsync());
        }

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
