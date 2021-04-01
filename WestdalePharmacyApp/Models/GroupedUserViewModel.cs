using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WestdalePharmacyApp.Models
{
    public class GroupedUserViewModel
    {
        public List<UserViewModel> Users { get; set; }
        public List<UserViewModel> Admins { get; set; }
    }
}
