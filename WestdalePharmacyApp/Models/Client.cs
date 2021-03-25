using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WestdalePharmacyApp.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string AddressLine1 { get; set; }


        public string AddressLine2 { get; set; }


        public string Province { get; set; }

        public string HealthCard { get; set; }


        public string InsuranceNumber { get; set; }

        //[DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }


        public string Gender { get; set; }

        public string Allergies { get; set; }

        public string PhoneNumber { get; set; }
    }
}
