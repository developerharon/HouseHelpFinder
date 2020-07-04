using System.ComponentModel.DataAnnotations;

namespace HouseHelpFinder.Models.ViewModels
{
    /// <summary>
    /// The view model used to get the data used to log in
    /// </summary>
    public class LoginModel
    {
        [Required]
        [UIHint("email")]
        public string Email { get; set; }
        [Required]
        [UIHint("password")]
        public string Password { get; set; }
    }
}
