using QHub.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace QHub.ViewModels
{
    public class NewTopicViewModel
    {
        public string? TopicName { get; set; }

        public Subject? Subject { get; set; }
    }
}
