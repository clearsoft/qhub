using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace QHub.Models
{
public class Subject
{
        public int Id { get; set; }

        [Display(Name = "Subject Name")]
        public string? Name { get; set; }

        public List<Question>? Questions { get; set; } = new List<Question>();

        public List<ApplicationUser>? Users { get; set; } = new List<ApplicationUser>();

        public bool IsSelected { get; set; } // Used for the registration process only
    }
}
