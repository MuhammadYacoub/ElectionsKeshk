using System.ComponentModel.DataAnnotations;
using AdvancedVotingSystem.Models.Enums;

namespace AdvancedVotingSystem.Models
{
    public class Election
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public InvalidationType InvalidationType { get; set; }

        [StringLength(500)]
        public string? LogoPath { get; set; }

        public string? BylawPdfPath { get; set; }

        public bool PrintCardAfterVote { get; set; }

        public bool ReviewInPages { get; set; }

        public EntryMethod EntryMethod { get; set; }

        public ElectionType Type { get; set; } = ElectionType.Positions;

        public ElectionStatus Status { get; set; }

        // Navigation properties
        public virtual ICollection<ElectionCategory> Categories { get; set; } = new List<ElectionCategory>();
        public virtual ICollection<Committee> Committees { get; set; } = new List<Committee>();
        public virtual ICollection<Position> Positions { get; set; } = new List<Position>();
    }
}
