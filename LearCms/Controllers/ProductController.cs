using LearCms.Contexts;
using LearCms.Entities;
using LearCms.Services;
using LearCms.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearCms.DTOs;

namespace LearCms.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public ProductController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // 🟢 GET: Product (Listado principal, usa DTOs)
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();

            // Mapeo de Entity a DTO
            var productDtos = products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl
            }).ToList();

            return View(productDtos);
        }

        // 🟢 GET: Product/Details/5 (Usa DTO)
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            if (entity == null) return NotFound();

            var dto = new ProductDto
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                Stock = entity.Stock,
                ImageUrl = entity.ImageUrl
            };

            return View(dto);
        }

        // 🟢 GET: Product/Create (Retorna ViewModel vacío)
        public IActionResult Create()
        {
            return View();
        }

        // 🟢 POST: Product/Create (Usa ViewModel)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var productEntity = new ProductEntity
                {
                    ProductId = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    Stock = viewModel.Stock
                };

                if (viewModel.ImageFile != null)
                {
                    productEntity.ImageUrl = await _fileService.SaveFileAsync(viewModel.ImageFile, "images/productos");
                }

                _context.Add(productEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // 🟢 GET: Product/Edit/5 (Faltaba esta acción, crucial para cargar la vista)
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Products.FindAsync(id);
            if (entity == null) return NotFound();

            // Mapeo de Entity a ProductEditViewModel
            var viewModel = new ProductEditViewModel
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Description = entity.Description ?? string.Empty,
                Price = entity.Price,
                Stock = entity.Stock,
                ExistingImageUrl = entity.ImageUrl // Se pasa la URL actual
            };

            return View(viewModel);
        }


        // 🟢 POST: Product/Edit/5 (Usa ViewModel para actualizar y manejar la imagen)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel viewModel)
        {
            if (viewModel.ProductId == Guid.Empty) return NotFound();

            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Products
                    .AsNoTracking() // Se usa AsNoTracking para permitir la actualización de la entidad
                    .FirstOrDefaultAsync(p => p.ProductId == viewModel.ProductId);

                if (existingProduct == null) return NotFound();

                var updatedEntity = new ProductEntity
                {
                    ProductId = viewModel.ProductId,
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    Stock = viewModel.Stock,
                    ImageUrl = viewModel.ExistingImageUrl // Se mantiene la URL existente por defecto
                };

                // Manejo de la nueva imagen
                if (viewModel.NewImageFile != null)
                {
                    // Borrar el archivo anterior
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        _fileService.DeleteFile(existingProduct.ImageUrl);
                    }
                    // Guardar y actualizar con la nueva URL
                    updatedEntity.ImageUrl = await _fileService.SaveFileAsync(viewModel.NewImageFile, "images/productos");
                }

                try
                {
                    _context.Update(updatedEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductEntityExists(updatedEntity.ProductId)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // 🟢 GET: Product/Delete/5 (Usa DTO)
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            if (entity == null) return NotFound();

            var dto = new ProductDto
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                Stock = entity.Stock,
                ImageUrl = entity.ImageUrl
            };

            return View(dto);
        }

        // 🟢 POST: Product/Delete/5 (Manejo de archivo y entidad)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var productEntity = await _context.Products.FindAsync(id);
            if (productEntity != null)
            {
                // Borrar el archivo físico antes de borrar la entidad
                if (!string.IsNullOrEmpty(productEntity.ImageUrl))
                {
                    _fileService.DeleteFile(productEntity.ImageUrl);
                }

                _context.Products.Remove(productEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductEntityExists(Guid id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}