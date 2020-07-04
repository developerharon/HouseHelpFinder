using System.ComponentModel.DataAnnotations;

namespace HouseHelpFinder.Models.ViewModels
{
    /// <summary>
    /// The view model used to get the data to send a reach out request
    /// </summary>
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
