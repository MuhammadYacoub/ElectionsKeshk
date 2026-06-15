using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedVotingSystem.Models
{
    public class VoteRecord
    {
        [Key]
        public int Id { get; set; }

        public int VoterId { get; set; }
        [ForeignKey(nameof(VoterId))]
        public virtual Voter? Voter { get; set; }

        public int CandidateId { get; set; }
        [ForeignKey(nameof(CandidateId))]
        public virtual Candidate? Candidate { get; set; }

        public bool Selection { get; set; } // true for Yes, false for No, used in Regulation/IndividualOpinion
    }
}
