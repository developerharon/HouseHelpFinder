using Microsoft.AspNetCore.Identity;

namespace HouseHelpFinder.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool isAvailable { get; set; }
        public byte[] ProfilePicture { get; set; }
    }
}
