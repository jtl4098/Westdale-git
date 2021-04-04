using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WestdalePharmacyApp.Data;
using WestdalePharmacyApp.IServices;
using WestdalePharmacyApp.Models;

namespace WestdalePharmacyApp.Controllers
{
    [Authorize]
    public class PrescriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;


        private IPrescriptionService _prescriptionService = null;

        public PrescriptionsController(
            ApplicationDbContext context,
            IPrescriptionService prescriptionService,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _context = context;
            _prescriptionService = prescriptionService;
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }

        [Authorize(Roles = "Client")]
        // GET: Prescriptions
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var applicationDbContext = _context.Prescriptions.Where(o => o.UserId == user.Id)
                .Include(p => p.User);

            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Client")]
        // GET: Prescriptions/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PrescriptionId == id);
            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        [Authorize(Roles = "Client")]
        // GET: Prescriptions/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        [Authorize(Roles = "Client")]
        // POST: Prescriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PrescriptionId,Refill,ImageFile,Status,CreationTime,UpdatedTime,SpecialInstruction,UserId")] Prescription prescription)
        {
            if (ModelState.IsValid)
            {

                

                prescription.PrescriptionId = Guid.NewGuid();
                _context.Add(prescription);
                await _context.SaveChangesAsync();

                
                return RedirectToAction(nameof(Index));

            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", prescription.UserId);
            return View(prescription);
        }


        [Authorize(Roles = "Client")]
        // GET: Prescriptions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", prescription.UserId);
            return View(prescription);
        }

        [Authorize(Roles = "Client")]
        // POST: Prescriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PrescriptionId,Refill,ImageFile,Status,CreationTime,UpdatedTime,SpecialInstruction,UserId")] Prescription prescription)
        {

            if (id != prescription.PrescriptionId)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var tempPrescription = await _context.Prescriptions.AsNoTracking().Where(o => o.PrescriptionId == id).FirstOrDefaultAsync();             prescription.ImageFile = tempPrescription.ImageFile;
            prescription.UpdatedTime = DateTimeOffset.Now;
            prescription.ImageFile = tempPrescription.ImageFile;
            prescription.CreationTime = tempPrescription.CreationTime;
            prescription.Status = tempPrescription.Status;
            prescription.RefillAvailable = tempPrescription.RefillAvailable;
            prescription.UserId = user.Id;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prescription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionExists(prescription.PrescriptionId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", prescription.UserId);
            return View(prescription);
        }

        [Authorize(Roles = "Client")]
        // GET: Prescriptions/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PrescriptionId == id);
            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        [Authorize(Roles = "Client")]
        // POST: Prescriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrescriptionExists(Guid id)
        {
            return _context.Prescriptions.Any(e => e.PrescriptionId == id);
        }


        [HttpPost]
        public async Task<string> SaveFile(FileUpload fileObj)
        {
            var user = await _userManager.GetUserAsync(User);

            

            Prescription prescription = JsonConvert.DeserializeObject<Prescription>(fileObj.Prescription);
            if (fileObj.file==null)
            {
                return "Failed";
            }
            if (fileObj.file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    fileObj.file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    prescription.ImageFile = fileBytes;

                    prescription.UserId = user.Id;
                    prescription.CreationTime = DateTimeOffset.Now;
                    prescription = _prescriptionService.Save(prescription);

                    if (prescription.PrescriptionId != null)
                    {
                        var roleUser = await _context.Roles.SingleOrDefaultAsync(m => m.Name == "Admin");
                        var adminUserId = await _context.UserRoles.Where(m => m.RoleId == roleUser.Id).FirstOrDefaultAsync();
                        var adminUser = await _context.Users.Where(o => o.Id == adminUserId.UserId).FirstOrDefaultAsync();
                        await _emailSender.SendEmailAsync(adminUser.Email, "New Prescription Request ", user.FirstName + " " + user.LastName + " requested prescription...");

                        await _emailSender.SendEmailAsync(user.Email, "Prescription Request", "Successfully get it");
                        return "Thank you! Your request has been successfully submitted!";
                    }
                }
            }
            return "Failed";
        }

        [HttpGet]
        public JsonResult GetSavedPrescription()
        {
            var prescription = _prescriptionService.GetSavedPrescription();
            prescription.ImageFile = this.GetImage(Convert.ToBase64String(prescription.ImageFile));
            return Json(prescription);

        }

        private byte[] GetImage(string sBase64String)
        {
            byte[] bytes = null;
            if (!string.IsNullOrEmpty(sBase64String))
            {
                bytes = Convert.FromBase64String(sBase64String);
            }
            return bytes;
        }



        [Authorize(Roles = "Client")]
        // GET: Prescriptions/Refill/5
        public async Task<IActionResult> Refill(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", prescription.UserId);
            return View(prescription);
        }

        [Authorize(Roles = "Client")]
        // POST: Prescriptions/Refill/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refill(Guid id, [Bind("PrescriptionId,Refill,ImageFile,Status,CreationTime,UpdatedTime,SpecialInstruction,UserId")] Prescription prescription)
        {

            if (id != prescription.PrescriptionId)
            {
                return NotFound();
            }
            var tempPrescription = await _context.Prescriptions.AsNoTracking().Where(o => o.PrescriptionId == id).FirstOrDefaultAsync(); prescription.ImageFile = tempPrescription.ImageFile;
            prescription.UpdatedTime = DateTimeOffset.Now;
            prescription.ImageFile = tempPrescription.ImageFile;
            prescription.CreationTime = tempPrescription.CreationTime;
            prescription.Status = "Completed";
            prescription.RefillAvailable = 0;

            prescription.UserId = tempPrescription.UserId;

           
            
            var requestedPrescription = tempPrescription;
            requestedPrescription.CreationTime = DateTimeOffset.Now;
            requestedPrescription.Status = "Refill Requested";
            requestedPrescription.UpdatedTime = null;
            requestedPrescription.SpecialInstruction = prescription.SpecialInstruction;
            //requestedPrescription.RefillAvailable = tempPrescription.RefillAvailable - 1;
            //requestedPrescription.TimesRefill = 
            requestedPrescription.PrescriptionId = new Guid();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prescription);
                    await _context.SaveChangesAsync();

                    _context.Add(requestedPrescription);
                    await _context.SaveChangesAsync();

                    var user = await _userManager.GetUserAsync(User);
                    await _emailSender.SendEmailAsync(user.Email, "Refill Request", "Your refill request successfully got it");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionExists(prescription.PrescriptionId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", prescription.UserId);
            return View(prescription);
        }



        [Authorize(Roles = "Admin")]
        // GET: Prescriptions for admin
        public async Task<IActionResult> IndexA()
        {
            var applicationDbContext = _context.Prescriptions.Include(p => p.User);

            var prescriptionViewModel =
                    from u in _context.Users
                    join p in _context.Prescriptions on u.Id equals p.UserId
                    where (p.Status.Equals("Not Set") || p.Status.Equals("In Process") || p.Status.Equals("Refill Requested"))
                    select new PrescriptionViewModel { ApplicationUser = u, Prescription = p };

            return View(await prescriptionViewModel.ToListAsync());
        }


        [Authorize(Roles = "Admin")]
        // GET: Prescriptions/Edit/5
        public async Task<IActionResult> EditA(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var prescription = await _context.Prescriptions.FindAsync(id);
            var prescription =
                    from u in _context.Users
                    join p in _context.Prescriptions on u.Id equals p.UserId
                    where p.PrescriptionId ==id
                    select new PrescriptionViewModel { ApplicationUser = u, Prescription = p };
            if (prescription == null)
            {
                return NotFound();
            }
            //ViewData["UserId"] = new SelectList(prescription, "Id", "Id", prescription);
            return View( await prescription.FirstOrDefaultAsync());
        }

        [Authorize(Roles = "Admin")]
        // POST: Prescriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditA(Guid id, [Bind("PrescriptionId,Refill,ImageFile,Status,RefillAvailable, CreationTime,UpdatedTime,SpecialInstruction,UserId")] Prescription prescription)
        {

            if (id != prescription.PrescriptionId)
            {
                return NotFound();
            }
            //var user = await _userManager.GetUserAsync(User);
            var user = _context.Prescriptions.AsNoTracking().Where(o => o.PrescriptionId == prescription.PrescriptionId ).ToList();
            
            var tempPrescription = await _context.Prescriptions.AsNoTracking().Where(o => o.PrescriptionId == id).FirstOrDefaultAsync();
            prescription.UserId = user[0].UserId;
            prescription.UpdatedTime = DateTimeOffset.Now;
            prescription.ImageFile = tempPrescription.ImageFile;
            prescription.CreationTime = tempPrescription.CreationTime;
            prescription.TimesRefill = tempPrescription.TimesRefill;
            
            if (prescription.Status.Equals("Completed"))
            {
                prescription.TimesRefill += 1;
                if (prescription.RefillAvailable > 0)
                {
                    prescription.RefillAvailable -= 1;
                }
                   
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prescription);
                    await _context.SaveChangesAsync();
                    if (prescription.Status.Equals("Completed")) {
                        var userEmail =
                           from u in _context.Users
                           join p in _context.Prescriptions on u.Id equals p.UserId
                           where p.PrescriptionId == id
                           select new PrescriptionViewModel { ApplicationUser = u, Prescription = p };
                        await _emailSender.SendEmailAsync(userEmail.FirstOrDefault().ApplicationUser.Email, "Prescription Request", "Your prescription is ready...");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionExists(prescription.PrescriptionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(IndexA));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", prescription.UserId);
            return View(prescription);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailsA(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var prescription = await _context.Prescriptions.FindAsync(id);
            var prescription =
                    from u in _context.Users
                    join p in _context.Prescriptions on u.Id equals p.UserId
                    where p.PrescriptionId == id
                    select new PrescriptionViewModel { ApplicationUser = u, Prescription = p };
            if (prescription == null)
            {
                return NotFound();
            }
            //ViewData["UserId"] = new SelectList(prescription, "Id", "Id", prescription);
            return View(await prescription.FirstOrDefaultAsync());
        }


        [Authorize(Roles = "Admin")]
        // GET: Prescriptions/Delete/5
        public async Task<IActionResult> DeleteA(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PrescriptionId == id);
            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        [Authorize(Roles = "Admin")]
        // POST: Prescriptions/Delete/5
        [HttpPost, ActionName("DeleteA")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedA(Guid id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexA));
        }

        [Authorize(Roles = "Admin")]
        // GET: Prescriptions for admin
        public async Task<IActionResult> IndexAR(string value)
        {
            var applicationDbContext = _context.Prescriptions.Include(p => p.User);
            var prescriptionViewModel =
                    from u in _context.Users
                    join p in _context.Prescriptions on u.Id equals p.UserId
                    where (p.Status.Equals("Completed"))
                    select new PrescriptionViewModel { ApplicationUser = u, Prescription = p };
            if (!string.IsNullOrEmpty(value))
            {
                prescriptionViewModel =
                    from u in _context.Users
                    join p in _context.Prescriptions on u.Id equals p.UserId
                    where (p.Status.Equals("Completed") && (u.LastName.Contains(value) || u.FirstName.Contains(value)))
                    select new PrescriptionViewModel { ApplicationUser = u, Prescription = p };
            }
                

            return View(await prescriptionViewModel.ToListAsync());
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailsAR(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var prescription = await _context.Prescriptions.FindAsync(id);
            var prescription =
                    from u in _context.Users
                    join p in _context.Prescriptions on u.Id equals p.UserId
                    where p.PrescriptionId == id
                    select new PrescriptionViewModel { ApplicationUser = u, Prescription = p };
            if (prescription == null)
            {
                return NotFound();
            }
            //ViewData["UserId"] = new SelectList(prescription, "Id", "Id", prescription);
            return View(await prescription.FirstOrDefaultAsync());
        }


        [Authorize(Roles = "Admin")]
        // GET: Prescriptions/Delete/5
        public async Task<IActionResult> DeleteAR(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PrescriptionId == id);
            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        [Authorize(Roles = "Admin")]
        // POST: Prescriptions/Delete/5
        [HttpPost, ActionName("DeleteAR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedAR(Guid id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexAR));
        }

    }
}
