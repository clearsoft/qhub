using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QHub.Models
{
    public class Response
    {
        public int Id { get; set; }

        [Display(Name="Your Response")]
        public string? ResponseText { get; set; }

        [NotMapped]
        public string? ShortResponseText => ResponseText == null || ResponseText.Length < 300 ? ResponseText : ResponseText.Substring(0, 300) + "...";

        public ApplicationUser? Responder { get; set; }

        //[Required(ErrorMessage = "This field is required")]
        public string? Comments { get; set; }

        [NotMapped]
        public string? ShortComments => Comments == null || Comments.Length < 300 ? Comments : Comments.Substring(0, 300) + "...";

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        [Range(0, 10, ErrorMessage = "Score must be between 0 and 10")]
        public double? Mark { get; set; }
    }
}
