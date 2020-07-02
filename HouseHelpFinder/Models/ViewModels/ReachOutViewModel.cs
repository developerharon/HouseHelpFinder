using System.ComponentModel.DataAnnotations;

namespace HouseHelpFinder.Models.ViewModels
{
    public class ReachOutViewModel
    {
        [Required]
        public string HouseHelpId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
