using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(255)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Enrolled subjects")]
        public List<Subject> Subjects { get; set; } = new List<Subject>();

        [NotMapped]
        public string DisplayName =>
            String.IsNullOrEmpty(LastName)
            ? Email
            : FirstName + " " + LastName;

        [NotMapped]
        public bool IsTeacher { get; set; }
    }
}
