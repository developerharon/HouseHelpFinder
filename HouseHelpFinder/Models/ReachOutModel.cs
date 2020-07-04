using Microsoft.EntityFrameworkCore;

namespace HouseHelpFinder.Models
{
    /// <summary>
    /// The class that represents the reach out request in the database
    /// </summary>
    [Owned]
    public class ReachOutModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
    }
}
