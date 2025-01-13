using Clothers.Data;
using Clothers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothers.Controllers
{
    public class ClothesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ClothesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClothesController
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: ClothesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ClothesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClothesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // GET: ClothesController/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Nie znaleziono takiego produktu!");
            }

            return View(product);
        }

        // POST: ClothesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product updatedProduct)
        {
            

            if (id != updatedProduct.Id)
            {
                return BadRequest("Nieprawidłowy identyfikator obiektu.");
            }

            var product = await _context.Products.FindAsync(id);

            if(product == null)
            {
                return NotFound("Nie znaleziono takiego produktu!");
            }

            if (ModelState.IsValid)
            {
                product.Name = updatedProduct.Name;
                product.Description = updatedProduct.Description;
                product.Price = updatedProduct.Price;
                product.Quantity = updatedProduct.Quantity;

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Wystąpił błąd podczasz zapisywania zmian. Spróbuj ponownie";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(updatedProduct);
        }

        // GET: ClothesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ClothesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
