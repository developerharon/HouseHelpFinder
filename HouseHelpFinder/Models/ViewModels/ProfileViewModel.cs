using Microsoft.AspNetCore.Http;

namespace HouseHelpFinder.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string UserId { get; set; }
        public string ProfilePictureUrl { get; set; }
        public IFormFile ProfilePicture { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public bool isAvailable { get; set; }
    }
}
