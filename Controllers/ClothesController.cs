using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clothers.Controllers
{
    public class ClothesController : Controller
    {
        // GET: ClothesController
        public ActionResult Index()
        {
            return View();
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
        public ActionResult Create(IFormCollection collection)
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
