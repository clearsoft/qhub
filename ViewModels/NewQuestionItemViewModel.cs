using QHub.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace QHub.ViewModels
{
    public class NewQuestionItemViewModel
    {
        public Question? Question { get; set; }

        public Subject? Subject { get; set; }
    }
}
