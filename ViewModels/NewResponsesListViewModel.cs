using QHub.Models;

namespace QHub.ViewModels
{
    public class NewResponsesListViewModel
    {
        public Question? Question { get; set; }
        public IEnumerable<Response> Responses { get; set; }
        public NewResponsesListViewModel(Question? question, IEnumerable<Response> responses)
        {
            Question = question;
            Responses = responses;
        }
    }
}
