using System.ComponentModel.DataAnnotations;

namespace HouseHelpFinder.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string HouseHelpId { get; set; }
        [Required]
        [UIHint("password")]
        public string CurrentPassword { get; set; }
        [Required]
        [UIHint("password")]
        public string NewPassword { get; set; }
        [Required]
        [UIHint("password")]
        public string ConfirmNewPassword { get; set; }
    }
}
