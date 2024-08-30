using System.ComponentModel.DataAnnotations;
using UnionSurvey.Common;

namespace UnionSurvey.Models
{
    public class UserViewModel
    {
        [Display(Name = "User Name")]
        public string UserName { get; set; } = null!;

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Last Name")]
        public string? LastName { get; set; } = null;

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [StringLength(100)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;

        [Display(Name = "Referal Code")]
        public string ReferalCode { get; set; } = Constant.US_ADMIN_REFERAL_CODE;
    }
}
