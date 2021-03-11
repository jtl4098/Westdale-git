using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestdalePharmacyApp.Models;

namespace WestdalePharmacyApp.IServices
{
    public interface IPrescriptionService
    {
        Prescription Save(Prescription prescription);
        Prescription GetSavedPrescription();
    }
}
