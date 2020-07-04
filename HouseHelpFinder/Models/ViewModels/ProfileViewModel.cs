using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace HouseHelpFinder.Models.ViewModels
{
    /// <summary>
    /// The view model that represents the data to be displayed in the profile page
    /// </summary>
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

        public List<ReachOutModel> ReachOuts { get; set; }
    }
}
