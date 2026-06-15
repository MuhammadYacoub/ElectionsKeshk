namespace AdvancedVotingSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalVoters { get; set; }
        public int TotalCommittees { get; set; }
        public int ActiveElections { get; set; }
        public int TotalElections { get; set; }
    }
}
