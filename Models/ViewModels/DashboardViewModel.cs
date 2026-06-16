using System.Collections.Generic;

namespace AdvancedVotingSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalVoters { get; set; }
        public int TotalCommittees { get; set; }
        public int ActiveElections { get; set; }
        public int TotalElections { get; set; }

        public List<VoterDashboardDto> RecentVoters { get; set; } = new();
        public List<CommitteeDashboardDto> Committees { get; set; } = new();
    }

    public class VoterDashboardDto
    {
        public string VoterName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string CommitteeName { get; set; } = string.Empty;
    }

    public class CommitteeDashboardDto
    {
        public string Name { get; set; } = string.Empty;
        public string ElectionName { get; set; } = string.Empty;
        public int TotalVoters { get; set; }
        public int VotesCast { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
