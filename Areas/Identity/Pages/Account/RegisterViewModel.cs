using QHub.Models;

namespace QHub.Areas.Identity.Pages.Account
{
    public class RegisterViewModel
    {
        public RegisterModel RegisterModel { get; set; }
        public List<Subject> Subjects { get; set; }

    }
}
