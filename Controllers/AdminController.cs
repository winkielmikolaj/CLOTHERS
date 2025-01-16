using Clothers.Constants;
using Clothers.Data;
using Clothers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clothers.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> AdminPanel()
        {
            var products = await _context.Products
                .Include(p => p.User)
                .ToListAsync();
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Nie znaleziono takiej oferty.");
            }

            product.IsApproved = true;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AdminPanel));
        }
    }
}
