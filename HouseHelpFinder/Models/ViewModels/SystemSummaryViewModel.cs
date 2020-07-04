namespace HouseHelpFinder.Models.ViewModels
{
    /// <summary>
    /// The view model used to display the system summary in the dashboard
    /// </summary>
    public class SystemSummaryViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalHouseHelps { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalReachOutsSent { get; set; }
    }
}
