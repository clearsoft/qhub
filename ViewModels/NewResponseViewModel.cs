using QHub.Models;
using System.ComponentModel.DataAnnotations;

namespace QHub.ViewModels
{
    public class NewResponseViewModel
    {
        public Question? Question { get; set; }

        [Display(Name ="Student Response")]
        public Response? Response { get; set; }
    }
}
