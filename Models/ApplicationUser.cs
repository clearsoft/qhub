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
        [Display(Name = "Username")]
        public string DisplayName =>
            String.IsNullOrEmpty(LastName)
            ? Email
            : FirstName + " " + LastName;

        [NotMapped]
        public bool IsTeacher { get; set; }

        [NotMapped]
        public string ShortSubjectList
        {
            get
            {
                string subjectList = @String.Join(", ", Subjects.Select(s => s.Name));
                return subjectList.Length <= 100 ? subjectList : subjectList.Substring(0, 100) + "...";
            }
        }
    }
}
