using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using QHub.Data;
using QHub.Models;
using System.Security.Claims;

namespace QHub.Classes
{
    public class Helpers
    {
        internal static List<Question> GetQuestionsForUser(ApplicationDbContext context, ClaimsPrincipal u)
        {
            if (context == null)
                return null;

            var userId = u.Identity.GetUserId();
            var user = context.Users.Find(userId);
            var subjects = context.Subjects.Where(s => s.Users.Contains(user));
            var list = context.Questions.Include(q => q.Responses).Where(q => subjects.Contains(q.Subject)).ToList();
            return list;
        }
    }
}
