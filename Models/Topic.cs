using System.ComponentModel.DataAnnotations;

namespace QHub.Models
{
    public class Topic
    {
        public int Id { get; set; }

        [Display(Name="Topic")]
        public string? Name { get; set; }

        public IEnumerable<Question> Questions { get; set; } = new List<Question>();

    }
}
