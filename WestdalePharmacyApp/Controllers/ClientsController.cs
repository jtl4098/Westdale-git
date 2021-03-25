using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestdalePharmacyApp.Models;

namespace WestdalePharmacyApp.Controllers
{
    public class ClientsController : Controller
    {
        private UserManager<ApplicationUser> userManager;

        public ClientsController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string value)
        {
            var user = from n in userManager.Users select n;


            ViewBag.SearchedUser = value;

            if (!string.IsNullOrEmpty(value))
            {
                user = user.Where(a => a.LastName.Contains(value) || a.PhoneNumber.Contains(value));
            }

            return View(await user.AsNoTracking().ToListAsync());

        }



        // GET: ClientController/Details/5
        public async Task<ActionResult> Details(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
