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
    public class MessagesForAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public string senderEmail;

        public MessagesForAdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
        }

        // GET: MessagesForAdmin
        public async Task<IActionResult> Index()
        {
            //var user = await _userManager.GetUserAsync(User);
            //return View(await _context.Messages.Where( m=> m.To_UserId.Equals(user.Id)).ToListAsync());
            return View(await _context.Messages.OrderByDescending(m => m.Timestamp).ToListAsync());
        }

        // GET: MessagesForAdmin/Details/5
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
            var toUser = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(message.To_UserId));
            if (toUser != null)
            {
                ViewBag.ToUser = toUser.Email;
            }
            else
            {
                ViewBag.ToUser = "";
            }
            

            return View(message);
        }

        // GET: MessagesForAdmin/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: MessagesForAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,Title,body,Timestamp,From_UserEmail,To_UserId")] Message message)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var toUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(message.From_UserEmail));
                if (toUser != null)
                {
                    message.To_UserId = toUser.Id;
                    message.To_User = toUser;
                }
                message.From_UserEmail = user.Email;
                message.MessageId = Guid.NewGuid();
                message.Timestamp = DateTimeOffset.Now;
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        // GET: MessagesForAdmin/Edit/5
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

        // POST: MessagesForAdmin/Edit/5
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

        // GET: MessagesForAdmin/Delete/5
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

        // POST: MessagesForAdmin/Delete/5
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


        // GET: MessagesForAdmin/Reply
        public async Task<IActionResult> Reply(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if(message == null)
            {
                return NotFound();
            }
            ViewData["OriginMessage"] = message.From_UserEmail;
            senderEmail = message.From_UserEmail;
            TempData["MsgEmail"] = message.From_UserEmail;
            return View();
        }

        // POST: MessagesForAdmin/Reply
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply([Bind("MessageId,Title,body,Timestamp,From_UserEmail,To_UserId")] Message message)
        {
            if (ModelState.IsValid)
            {
                
                var toUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(TempData["MsgEmail"].ToString()));
                if(toUser != null)
                {
                    message.To_UserId = toUser.Id;
                    message.To_User = toUser;
                }
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
                var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(roleUser.UserId));
                message.From_UserEmail = adminUser.Email;


                //Send Notification via Email to admin and user
                //await _emailSender.SendEmailAsync(message.From_UserEmail, "Email Request", "Successfully get it");
                //await _emailSender.SendEmailAsync(message.To_User.Email, "Email Request", "Successfully get it");
                message.Timestamp = DateTimeOffset.Now;
                message.MessageId = Guid.NewGuid();
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }




    }
}
