using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WestdalePharmacyApp.Models
{
    public class PrescriptionViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public Prescription Prescription { get; set; }
    }
}
