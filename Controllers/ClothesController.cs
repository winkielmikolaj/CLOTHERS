using Clothers.Constants;
using Clothers.Data;
using Clothers.Models;
using Clothers.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Clothers.Controllers
{
    public class ClothesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly long _maxImageSize = 2 * 1024 * 1024; // 2 MB
        private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public ClothesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(string searchString)
        {
            // Przekazanie searchString do widoku za pomocą ViewBag
            ViewBag.CurrentFilter = searchString;

            var products = _context.Products
                .Where(p => p.IsApproved && p.Quantity > 0);

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => EF.Functions.Like(p.Name, $"%{searchString}%"));
            }

            var approvedProducts = await products.ToListAsync();
            return View(approvedProducts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Roles.Company + "," + Roles.Admin)]
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
                    IsApproved = false
                };

                if (Image != null && Image.Length > 0)
                {

                    //walidacja zdjecia
                    var extension = Path.GetExtension(Image.FileName).ToLower();
                    if (!_permittedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("Image", "Nieobsługiwany format pliku obrazka.");
                        return View(productVm);
                    }

                    if (Image.Length > _maxImageSize)
                    {
                        ModelState.AddModelError("Image", "Rozmiar pliku obrazka przekracza 2 MB.");
                        return View(productVm);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await Image.CopyToAsync(memoryStream);
                        product.Image = memoryStream.ToArray(); //zapisanie obrazu jako bit tabilac
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

        [HttpGet]
        [Authorize(Roles = Roles.Company + "," + Roles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Nie znaleziono takiego produktu!");
            }

            var productVm = new ProductsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Sizes = product.Sizes,
                Image = product.Image
            };

            return View(productVm);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Company + "," + Roles.Admin)]
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

                //logiak dodawania zdjecia
                if (Image != null && Image.Length > 0)
                {
                    //walidacja zdjecia
                    var extension = Path.GetExtension(Image.FileName).ToLower();
                    if (!_permittedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("Image", "Nieobsługiwany format pliku obrazka.");
                        return View(productVm);
                    }

                    if (Image.Length > _maxImageSize)
                    {
                        ModelState.AddModelError("Image", "Rozmiar pliku obrazka przekracza 2 MB.");
                        return View(productVm);
                    }


                    using (var memoryStream = new MemoryStream())
                    {
                        await Image.CopyToAsync(memoryStream);
                        product.Image = memoryStream.ToArray();
                    }
                }

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

        [HttpGet]
        [Authorize(Roles = Roles.Company + "," + Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Nie znaleziono takiego produktu!");
            }

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Company + "," + Roles.Admin)]
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


        [Authorize]
        public async Task<IActionResult> Cart()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var cartViewModel = new CartViewModel();

            if (cart != null)
            {
                cartViewModel.Items = cart.Items.Select(ci => new CartItemViewModel
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity,
                    Total = ci.Product.Price * ci.Quantity,
                    Image = ci.Product.Image
                }).ToList();

                cartViewModel.Total = cartViewModel.Items.Sum(i => i.Total);
            }

            return View(cartViewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsApproved)
            {
                TempData["ErrorMessage"] = "Produkt nie istnieje lub nie jest dostępny.";
                return RedirectToAction(nameof(Index));
            }

            //walidacja ilosci, ktora zostanie dodana do koszyka
            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Ilość musi być większa niż zero.";
                return RedirectToAction(nameof(Index));
            }

            if (product.Quantity < quantity)
            {
                TempData["ErrorMessage"] = "Żądana ilość przekracza dostępny stan magazynowy.";
                return RedirectToAction(nameof(Index));
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == id);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = id,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Produkt został dodany do koszyka.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                TempData["ErrorMessage"] = "Koszyk jest pusty.";
                return RedirectToAction(nameof(Cart));
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Produkt został usunięty z koszyka.";
            }
            else
            {
                TempData["ErrorMessage"] = "Produkt nie został znaleziony w koszyku.";
            }

            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Koszyk jest pusty.";
                return RedirectToAction(nameof(Cart));
            }

            //aktualizacja zamowienia
            foreach (var item in cart.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    if (product.Quantity < 0)
                    {
                        product.Quantity = 0;
                    }
                }
            }

//usuniecie koszyka po zamowieniu
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Zamówienie zostało zrealizowane pomyślnie.";
            return RedirectToAction(nameof(Index));
        }

    }
}
