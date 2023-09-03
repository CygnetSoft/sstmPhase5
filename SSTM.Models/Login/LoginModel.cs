using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.Login
{
    public class LoginModel
    {
        public string TrainingCenterName { get; set; }

        [Required(ErrorMessage = "Please enter email address.")]
        [RegularExpression(@"^((\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)\s*[;,.]{0,1}\s*)+$", ErrorMessage = "Please enter valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter password.")]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public bool isLocationVerified { get; set; }

        public string Macaddress { get; set; }
    }
}