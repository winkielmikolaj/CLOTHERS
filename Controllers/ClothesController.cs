using Clothers.Data;
using Clothers.Models;
using Clothers.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothers.Controllers
{
    public class ClothesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ClothesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ClothesController
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: ClothesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Nie znaleziono takiego produktu!");
            }
            return View(product);
        }

        // GET: ClothesController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClothesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductsViewModel productVm, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = productVm.Name,
                    Description = productVm.Description,
                    Price = productVm.Price,
                    Quantity = productVm.Quantity,
                    Sizes = productVm.Sizes,
                    UserId = _userManager.GetUserId(User),
                };

                if (Image != null && Image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Image.CopyToAsync(memoryStream);
                        product.Image = memoryStream.ToArray(); // Zapisanie obrazu w bazie jako byte[]
                    }
                }
                else
                {
                    product.Image = productVm.Image;
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productVm);
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

            var productVm = new ProductsViewModel
            {
                Id = product.Id, // Przypisanie Id
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Sizes = product.Sizes,
                Image = product.Image
            };

            return View(productVm);
        }

        // POST: ClothesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductsViewModel productVm, IFormFile Image)
        {
            if (id != productVm.Id)
            {
                return BadRequest("Nieprawidłowy identyfikator obiektu.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Nie znaleziono takiego produktu!");
            }

            if (ModelState.IsValid)
            {
                product.Name = productVm.Name;
                product.Description = productVm.Description;
                product.Price = productVm.Price;
                product.Quantity = productVm.Quantity;
                product.Sizes = productVm.Sizes;

                // Obsługa zdjęcia
                if (Image != null && Image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Image.CopyToAsync(memoryStream);
                        product.Image = memoryStream.ToArray();  // Zapisujemy zdjęcie jako tablicę bajtów
                    }
                }
                // Jeśli nie przesłano nowego obrazu, zachowaj istniejący

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Wystąpił błąd podczas zapisywania zmian. Spróbuj ponownie.";
                }
            }

            return View(productVm);
        }

        // GET: ClothesController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Nie znaleziono takiego produktu!");
            }

            return View(product);
        }

        // POST: ClothesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int quantityToRemove)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Nie znaleziono takiego produktu!");
            }

            if (quantityToRemove <= 0)
            {
                TempData["ErrorMessage"] = "Ilość do usunięcia musi być większa niż zero.";
                return RedirectToAction(nameof(Index));
            }

            if (product.Quantity >= quantityToRemove)
            {
                product.Quantity -= quantityToRemove;

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ilość produktu została zmniejszona.";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Wystąpił błąd podczas zapisywania zmian. Spróbuj ponownie.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Ilość do usunięcia przekracza dostępny stan magazynowy.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
