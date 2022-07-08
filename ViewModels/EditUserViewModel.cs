using QHub.Models;

namespace QHub.ViewModels
{
    public class EditUserViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Subject> Subjects { get; set; }
        public bool IsTeacher { get; set; }
    }
}
