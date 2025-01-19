using Clothers.Constants;
using Clothers.Data;
using Clothers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Clothers.Controllers
{
    //autoryzacja akcji tylko dla admina
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> AdminPanel()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.User)
                    .ToListAsync();

                var users = await _userManager.Users.ToListAsync();

                var model = new AdminPanelViewModel
                {
                    Products = products,
                    Users = users
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas ładowania panelu administratora.");
                return StatusCode(500, "Wystąpił problem podczas ładowania strony.");
            }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Nie znaleziono użytkownika.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AdminPanel));
            }

            return BadRequest("Nie udało się usunąć użytkownika.");
        }

        public IActionResult CreateUser()
        {
            return PartialView("_CreateUserPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {

            //nowa walidacja sprawdzajaca przed dodaniem czy taki email juz istnieje
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Użytkownik o podanym adresie e-mail już istnieje.");
                return PartialView("_CreateUserPartial", model);
            }

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }
                    return RedirectToAction(nameof(AdminPanel));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return PartialView("_CreateUserPartial", model);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Nie znaleziono użytkownika.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles
            };

            return PartialView("_EditUserPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound("Nie znaleziono użytkownika.");
                }

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != model.Id)
                {
                    ModelState.AddModelError(string.Empty, "Podany adres e-mail jest już używany przez innego użytkownika.");
                    return PartialView("_EditUserPartial", model);
                }

                user.Email = model.Email;
                user.UserName = model.Email;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                    if (model.Roles != null && model.Roles.Any())
                    {
                        await _userManager.AddToRolesAsync(user, model.Roles);
                    }

                    return RedirectToAction(nameof(AdminPanel));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return PartialView("_EditUserPartial", model);
        }
    }

    public class AdminPanelViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<IdentityUser> Users { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Rola")]
        public string Role { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //lista rol przypisanych do uzytkownika
        [Display(Name = "Role")]
        public IList<string> Roles { get; set; }

        //lista dostępnych ról do wyboru
        public List<SelectListItem> AvailableRoles { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "Admin", Value = "Admin" },
            new SelectListItem { Text = "UżytkownikStrony", Value = "SiteUser" },
            new SelectListItem { Text = "Firma", Value = "Company" }
        };
    }
}
