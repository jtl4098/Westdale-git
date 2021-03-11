using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WestdalePharmacyApp.Models;

namespace WestdalePharmacyApp.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
           

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "City")]
            public string City { get; set; }

            [Required]
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }

            [Required]
            [Display(Name = "Address Line1")]
            public string AddressLine1 { get; set; }

            
            [Display(Name = "Address Line2")]
            public string AddressLine2 { get; set; }

            [Required]
            [Display(Name = "Province")]
            public string Province { get; set; }

           
            [Display(Name = "Health Card No")]
            public string HealthCard { get; set; }

           
            [Display(Name = "Insurance No")]
            public string InsuranceNumber { get; set; }

            [Required]
            [Display(Name = "Birth Date")]
            [DataType(DataType.Date)]
            public DateTime BirthDate { get; set; }


            [Required]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            
            [Display(Name = "Allergies")]
            public string Allergies { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }


        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var firstName = user.FirstName;
            var lastName = user.LastName;
            var city = user.City;
            var postalCode = user.PostalCode;
            var addressLine1 = user.AddressLine1;
            var addressLine2 = user.AddressLine2;
            var province = user.Province;
            var healthCard = user.HealthCard;
            var insuranceNumber = user.InsuranceNumber;
            var birthDate = user.BirthDate;
            var gender = user.Gender;
            var allergies = user.Allergies;




            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = firstName,
                LastName = lastName,
                City = city,
                PostalCode = postalCode,
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                Province = province,
                HealthCard = healthCard,
                InsuranceNumber = insuranceNumber,
                BirthDate = birthDate,
                Gender = gender,
                Allergies = allergies
        };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            //Update custom properties
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.City = Input.City;
            user.PostalCode = Input.PostalCode;
            user.AddressLine1 = Input.AddressLine1;
            user.AddressLine2 = Input.AddressLine2;
            user.Province = Input.Province;
            user.HealthCard = Input.HealthCard;
            user.InsuranceNumber = Input.InsuranceNumber;
            user.BirthDate = Input.BirthDate;
            user.Gender = Input.Gender;
            user.Allergies = Input.Allergies;
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
