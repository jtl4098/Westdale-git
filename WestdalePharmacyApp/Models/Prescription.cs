using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WestdalePharmacyApp.Models
{
    public class Prescription
    {
        [Key]
        public Guid PrescriptionId { get; set; }

        public string Refill { get; set; }
        public byte[] ImageFile { get; set; }

        public string Status { get; set; }

        public DateTimeOffset CreationTime { get; set; }

        public DateTimeOffset UpdatedTime { get; set; }

        [Display(Name = "Special Instruction")]
        public string SpecialInstruction { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

    }
}
