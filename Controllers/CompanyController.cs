using Clothers.Constants;
using Clothers.Data;
using Clothers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clothers.Controllers
{
    [Authorize(Roles = Roles.Company)]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CompanyController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> CompanyPanel()
        {
            var userId = _userManager.GetUserId(User);

            var userOffers = await _context.Products
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return View(userOffers);
        }

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
    }
}
