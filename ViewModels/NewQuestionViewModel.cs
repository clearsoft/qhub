using QHub.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace QHub.ViewModels
{
    public class NewQuestionViewModel
{
        public Subject? Subject { get; set; }

        public Question? Question { get; set; }

    }
}
