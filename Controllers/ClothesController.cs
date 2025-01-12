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
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: ClothesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ClothesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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
