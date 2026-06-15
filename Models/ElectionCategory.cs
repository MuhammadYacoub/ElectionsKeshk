using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdvancedVotingSystem.Models.Enums;

namespace AdvancedVotingSystem.Models
{
    public class ElectionCategory
    {
        [Key]
        public int Id { get; set; }

        public int ElectionId { get; set; }
        [ForeignKey(nameof(ElectionId))]
        public virtual Election? Election { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public CategoryType CategoryType { get; set; }

        public int? RequiredSeats { get; set; }

        // Navigation properties
        public virtual ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
    }
}
