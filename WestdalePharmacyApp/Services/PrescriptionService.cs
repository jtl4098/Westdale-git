using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestdalePharmacyApp.Data;
using WestdalePharmacyApp.IServices;
using WestdalePharmacyApp.Models;

namespace WestdalePharmacyApp.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        Prescription IPrescriptionService.GetSavedPrescription()
        {
            return _context.Prescriptions.SingleOrDefault();
        }

        Prescription IPrescriptionService.Save(Prescription prescription)
        {
            _context.Prescriptions.Add(prescription);
            _context.SaveChanges();
            return prescription;
        }
    }
}
