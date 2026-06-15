using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AdvancedVotingSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [Display(Name = "الاسم")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "اللقب مطلوب")]
        [Display(Name = "اللقب")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "النوع مطلوب")]
        [Display(Name = "النوع")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "الرقم القومي مطلوب")]
        [Display(Name = "الرقم القومي أو المعرف")]
        public string NationalId { get; set; } = string.Empty;

        [Display(Name = "الانتخابات")]
        public int? ElectionId { get; set; }

        public virtual Election? Election { get; set; }

        [Display(Name = "اللجنة")]
        public int? CommitteeId { get; set; }

        public virtual Committee? Committee { get; set; }
    }
}
