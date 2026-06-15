using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvancedVotingSystem.Models
{
    public class Position
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int RequiredCount { get; set; }
        public int ElectionId { get; set; }
        public Election Election { get; set; }
        public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
    }
}
