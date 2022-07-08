using QHub.Models;

namespace QHub.Classes
{
    public static class ExtensionMethods
    {
        public static Question Clone(this Question q)
        {
            var clone = new Question()
            {
                Id = q.Id,
                Subject = q.Subject,
                QuestionText = q.QuestionText,
                TopicName = q.TopicName,
                OpenDate = q.OpenDate,
                CloseDate = q.CloseDate,
                IdealResponse = q.IdealResponse,
                IsCompleted = q.IsCompleted,
                IsVisible = q.IsVisible,
                Responses = q.Responses
            };
            return clone;
        }
    }
}
