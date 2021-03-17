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
    public class UsersController : Controller
    {
        private UserManager<ApplicationUser> userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
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
            //var users = userManager.Users.Where(o => o.Id = id);
            //var users = userManager.Users;
            //return View(users);

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


        // GET: Users1/Edit/5
        public async Task<IActionResult> Edit(string? id)
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

        //POST: Users1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("FirstName,LastName,Email,City,PostalCode,AddressLine1,AddressLine2,Province,HealthCard,InsuranceNumber,BirthDate,Gender,Allergies,PhoneNumber")] User user)
        //{
        //    if (id != user.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await userManager.CreateAsync(user);
        //            object p = await userManager.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!UserExists(user.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(user);
        //    var result = await userManager.ConfirmEmailAsync(user);
        //}

    }
}
