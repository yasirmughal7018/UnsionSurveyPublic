using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace UnionSurvey.Models
{
    public class LoginViewModel
    {
        [Display(Name ="User Name")]
        public string UserName { get; set; } = null!;
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; } = true;
    }
}
