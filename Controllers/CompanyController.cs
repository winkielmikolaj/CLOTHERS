using Clothers.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clothers.Controllers
{
    public class CompanyController : Controller
    {

        [Authorize(Roles = Roles.Company)]
        public IActionResult CompanyPanel()
        {
            return View();
        }
    }
}
