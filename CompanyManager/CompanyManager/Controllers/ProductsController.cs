﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace CompanyManager.Controllers
{
    [Authorize(Roles = "ADMIN,SELLER")]
    public class ProductsController : Controller
    {
        private readonly CMContext _context;

        public ProductsController(CMContext context) {
            _context = context;
        }

        private bool ProductExists(int id) {
            return _context.Product.Any(e => e.Id == id);
        }

        // Vista de listado de productos.
        public async Task<IActionResult> Index()
        {
              return View(await _context.Product.ToListAsync());
        }

        // Vista detalle de producto.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Vista de creación de producto vacía.
        public IActionResult Create()
        {
            return View();
        }

        // Al crear producto, luego de apretar en el botón crear.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid) {
                _context.Add(product);
                await _context.SaveChangesAsync();

                var log = StocksController.CreateStockLog(product, product.Stock, "Nuevo Producto");
                _context.Stock.Add(log);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // Vista de edición de producto.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Al editar producto, luego de apretar en el botón editar.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }


            var lastStockLog = _context.Stock.OrderBy(o => o.UpdatedAt).Where(s => s.ProductId == id).Last();
            int updatedQuantity = product.Stock - lastStockLog.CurrentStock;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    var log = StocksController.CreateStockLog(product, updatedQuantity, "Producto Editado");
                    _context.Stock.Add(log);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // Vista de eliminar de producto.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null) {
                return NotFound();
            }

            var product = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) {
                return NotFound();
            }

            return View(product);
        }

        // Al eliminar producto, luego de apretar en el botón eliminar.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null) {
                return Problem("Entity set 'CMContext.Product'  is null.");
            }

            var product = await _context.Product.FindAsync(id);
            
            if (product != null) {
                IList<Stock> productStockLogs = await _context.Stock.Where(s => s.ProductId == product.Id).ToListAsync();
                _context.Stock.RemoveRange(productStockLogs);
                _context.Product.Remove(product);
                _context.SaveChanges();
            }
            
            return RedirectToAction("Index", "Store");
        }
    }
}
