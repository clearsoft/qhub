using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QHub.Models
{
    public class Question
    {
        public int Id { get; set; }

        public Subject? Subject { get; set; }

        [Display(Name = "Topic")]
        public string TopicName { get; set; }

        [Display(Name = "Research Question")]
        public string QuestionText { get; set; }

        public string? ShortQuestionText => QuestionText == null || QuestionText.Length < 300 ? QuestionText : QuestionText.Substring(0,300) + "...";

        [Display(Name = "Ideal Response")]
        public string IdealResponse { get; set; }

        [Display(Name = "Opening Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime OpenDate { get; set; }

        [Display(Name = "Closing Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime CloseDate { get; set; }

        [Display(Name = "Is Completed")]
        public bool IsCompleted { get; set; }

        public bool IsVisible { get; set; }

        [NotMapped]
        public bool IsBeforeOpen => DateTime.Now < OpenDate;
        [NotMapped]
        public bool IsBeforeClose => DateTime.Now < CloseDate;
        [NotMapped]
        public bool IsAfterClose => DateTime.Now > CloseDate.AddDays(7);

        public List<Response> Responses { get; set; } = new List<Response>();

    }
}
