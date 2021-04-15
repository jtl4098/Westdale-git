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

        [Display(Name = "Available Refill")]
        public int RefillAvailable { get; set; }

        [Display(Name = "Refill Times")]
        public int TimesRefill { get; set; }

        [Display(Name = "Prescription")]
        public byte[] ImageFile { get; set; }

        public string Status { get; set; }

        [Display(Name = "Creation Time")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy hh:mm tt}")]
        public DateTimeOffset CreationTime { get; set; }

        [Display(Name = "Update Time")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy hh:mm tt}")]
        public DateTimeOffset? UpdatedTime { get; set; }

        [Display(Name = "Special Instruction")]
        public string SpecialInstruction { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }


        public Prescription()
        {
            Status = "Not Set";
            UpdatedTime = null;
            RefillAvailable = 0;
            TimesRefill = 0;
        }
    }
}
