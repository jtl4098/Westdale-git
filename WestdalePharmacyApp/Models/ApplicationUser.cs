using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WestdalePharmacyApp.Models
{
    public class ApplicationUser :IdentityUser
    {
        public virtual ICollection<Prescription> Prescriptions { get; set; }

        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }

        [PersonalData]
        public string City { get; set; }

        [PersonalData]
        public string PostalCode { get; set; }

        [PersonalData]
        public string Province { get; set; }

        [PersonalData]
        public string AddressLine1 { get; set; }

        [PersonalData]
        public string AddressLine2 { get; set; }

        [PersonalData]
        public string HealthCard { get; set; }

        [PersonalData]
        public string InsuranceNumber { get; set; }

        [PersonalData]
        public DateTime BirthDate { get; set; }


        [PersonalData]
        public string Gender { get; set; }

        [PersonalData]
        public string Allergies { get; set; }

        
     




    }
}
