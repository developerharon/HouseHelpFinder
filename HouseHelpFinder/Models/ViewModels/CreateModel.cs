using System.ComponentModel.DataAnnotations;

namespace HouseHelpFinder.Models.ViewModels
{
    /// <summary>
    /// The view model used to get the data required to create a new account with the system
    /// </summary>
    public class CreateModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [UIHint("password")]
        public string Password { get; set; }
        [Required]
        [UIHint("password")]
        public string ConfirmPassword { get; set; }
    }
}
