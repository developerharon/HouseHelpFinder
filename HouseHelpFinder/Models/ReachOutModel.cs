using Microsoft.EntityFrameworkCore;

namespace HouseHelpFinder.Models
{
    [Owned]
    public class ReachOutModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
    }
}
