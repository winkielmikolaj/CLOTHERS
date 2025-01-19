using Clothers.Constants;
using Clothers.Data;
using Clothers.Models;
using Clothers.Services;
using Clothers.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Clothers.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<OrdersController> _logger;
        private readonly OrderPdfGenerator _pdfGenerator;

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<OrdersController> logger, OrderPdfGenerator pdfGenerator)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _pdfGenerator = pdfGenerator;
        }

        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Wywołano akcję GET Create w OrdersController.");

            var userId = _userManager.GetUserId(User);
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Twój koszyk jest pusty.";
                return RedirectToAction("Cart", "Clothes");
            }

            var viewModel = new OrderViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            _logger.LogInformation("Wywołano akcję POST Create w OrdersController.");

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.Items.Any())
                {
                    TempData["ErrorMessage"] = "Twój koszyk jest pusty.";
                    return RedirectToAction("Cart", "Clothes");
                }

                var order = new Order
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DeliveryAddress = model.DeliveryAddress,
                    PaymentMethod = model.PaymentMethod,
                    DeliveryMethod = model.DeliveryMethod,
                    OrderDate = DateTime.Now,
                    UserId = userId
                };

                foreach (var item in cart.Items)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price
                    });
                }

                _context.Orders.Add(order);

                // Aktualizacja ilości produktów
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity;
                        if (product.Quantity < 0)
                        {
                            TempData["ErrorMessage"] = $"Brak wystarczającej ilości produktu: {product.Name}.";
                            return RedirectToAction("Cart", "Clothes");
                        }
                    }
                }

                // Usunięcie przedmiotów z koszyka
                _context.CartItems.RemoveRange(cart.Items);

                try
                {
                    await _context.SaveChangesAsync();

                    // Przekierowanie do akcji Confirmation z przekazaniem orderId
                    return RedirectToAction(nameof(Confirmation), new { orderId = order.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Błąd podczas zapisywania zamówienia.");
                    TempData["ErrorMessage"] = "Wystąpił błąd podczas przetwarzania Twojego zamówienia. Spróbuj ponownie później.";
                }
            }

            // Jeśli ModelState nie jest valid, wróć do widoku z błędami
            return View(model);
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == _userManager.GetUserId(User));

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize(Roles = Roles.Company)]
        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderItems.Any(oi => oi.Product.UserId == userId))
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> DownloadPdf(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == _userManager.GetUserId(User));

            if (order == null)
            {
                return NotFound();
            }

            var pdfBytes = _pdfGenerator.GeneratePdf(order);
            return File(pdfBytes, "application/pdf", $"Order_{order.Id}.pdf");
        }

    }
}
