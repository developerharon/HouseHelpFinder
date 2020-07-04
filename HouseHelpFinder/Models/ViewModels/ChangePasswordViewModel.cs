using System.ComponentModel.DataAnnotations;

namespace HouseHelpFinder.Models.ViewModels
{
    /// <summary>
    /// The view model used to get the data for changing the password
    /// </summary>
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
