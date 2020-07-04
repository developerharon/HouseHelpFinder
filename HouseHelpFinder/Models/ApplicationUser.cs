using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HouseHelpFinder.Models
{
    /// <summary>
    /// The class that represents the user of the system in the database
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool isAvailable { get; set; }
        public byte[] ProfilePicture { get; set; }
        public List<ReachOutModel> ReachOuts { get; set; }
    }
}
