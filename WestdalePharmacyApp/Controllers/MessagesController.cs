using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WestdalePharmacyApp.Data;
using WestdalePharmacyApp.Models;

namespace WestdalePharmacyApp.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        

        public MessagesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Messages.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,Title,body,Timestamp,From_UserEmail,To_UserId")] Message message)
        {
            //var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {

                message.MessageId = Guid.NewGuid();
                message.Timestamp = DateTimeOffset.Now;



                var roleUser = (from role in _context.UserRoles
                                join u in _context.Users on role.UserId equals u.Id
                                join a in _context.Roles on role.RoleId equals a.Id
                                where (a.NormalizedName.Equals("ADMIN"))
                                select new UserViewModel
                                {
                                    UserId = u.Id,
                                    RoleId = a.Id,
                                    NormalizedName = a.NormalizedName
                                }).FirstOrDefault();

                message.To_UserId = roleUser.UserId;
                message.To_User = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(roleUser.UserId));
                //Send Notification via Email to admin and user
                //await _emailSender.SendEmailAsync(message.From_UserEmail, "Email Request", "Successfully get it");
                //await _emailSender.SendEmailAsync(message.To_User.Email, "Email Request", "Successfully get it");





                _context.Add(message);
                await _context.SaveChangesAsync();

             
                
                return RedirectToAction("Index", "Home");
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", message.To_User);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MessageId,Title,body,Timestamp,From_UserEmail,To_UserId")] Message message)
        {
            if (id != message.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var message = await _context.Messages.FindAsync(id);
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(Guid id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
    }
}
