using System.ComponentModel.DataAnnotations;

namespace AdvancedVotingSystem.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Title { get; set; }
        public string? Nickname { get; set; }
        public string? ImagePath { get; set; }
        public string? ElectoralSymbol { get; set; }
        public int? CandidateNumber { get; set; }
        public int PositionId { get; set; }
        public Position Position { get; set; }
        public ICollection<VoteRecord> VoteRecords { get; set; } = new List<VoteRecord>();
    }
}
