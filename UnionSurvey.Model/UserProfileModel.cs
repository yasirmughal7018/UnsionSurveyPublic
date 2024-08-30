using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class UserProfileModel
    {
        [Display(Name = "Id")]
        public string? Id { get; set; }

        [Display(Name = "User Name")]
        public string? UserName { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "User Code")]
        public string? UserCode { get; set; } = null!;

        [Display(Name = "Referal Code")]
        public string? ReferalCode { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? BrithDate { get; set; }

        public string? Avatar { get; set; }

        [Display(Name = "Country")]
        public string? Country { get; set; }

        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        public bool IsAgent { get; set; }

    }
}
