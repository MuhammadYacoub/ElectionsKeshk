using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedVotingSystem.Models
{
    public class Voter
    {
        [Key]
        public int Id { get; set; }

        public int CommitteeId { get; set; }
        [ForeignKey(nameof(CommitteeId))]
        public virtual Committee? Committee { get; set; }

        [Required]
        [StringLength(100)]
        public string LoginIdentifier { get; set; } = string.Empty; // NationalId or Code

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? NationalId { get; set; }

        public bool HasVoted { get; set; }

        public int LastUsedSequence { get; set; } = -10;

        // Navigation properties
        public virtual ICollection<VoteRecord> VoteRecords { get; set; } = new List<VoteRecord>();
    }
}
