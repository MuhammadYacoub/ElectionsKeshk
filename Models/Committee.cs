using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdvancedVotingSystem.Models.Enums;

namespace AdvancedVotingSystem.Models
{
    public class Committee
    {
        [Key]
        public int Id { get; set; }

        public int ElectionId { get; set; }
        [ForeignKey(nameof(ElectionId))]
        public virtual Election? Election { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? HeadId { get; set; }

        [ForeignKey(nameof(HeadId))]
        public virtual ApplicationUser? Head { get; set; }

        public CommitteeStatus Status { get; set; }

        public int TotalVotesCast { get; set; } = 0;

        public int RegisteredVotersCount { get; set; } = 0;

        public int CooldownThreshold { get; set; } = 3;

        // Navigation properties
        public virtual ICollection<Voter> Voters { get; set; } = new List<Voter>();
        
        [InverseProperty("Committee")]
        public virtual ICollection<ApplicationUser> Supervisors { get; set; } = new List<ApplicationUser>();
    }
}
