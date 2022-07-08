using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using QHub.Classes;
using QHub.Data;
using QHub.Models;
using QHub.ViewModels;
using System.Data;
using System.Security.Claims;

namespace QHub.Controllers
{
    [Authorize()]
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Question/5
        public async Task<IActionResult> Index()
        {
            if (_context.Questions == null)
            {
                return NotFound();
            }
            else
            {
                // Get user for later
                var userId = User.Identity.GetUserId();
                var user = _context.Users.Find(userId);

                bool isAdmin = User.IsInRole(Enumerations.QHubRoles.Admin.ToString());
                bool isTeacher = User.IsInRole(Enumerations.QHubRoles.Teacher.ToString());

                var subjects = _context.Subjects
                    .Where(s => (s.Users.Contains(user) || isAdmin)); // Admin sees all 

                if (isAdmin || isTeacher)
                {
                    var subjectsAdmin = await subjects
                        .Include(s => s.Questions)
                        .ThenInclude(q => q.Responses)
                        .ToListAsync();

                    return View(subjectsAdmin);
                }

                // Student sees only those of their subject which are open and not stale
                var subjectsStudent = await subjects
                    .Where(s => s.Questions.Any(q => DateTime.Now < q.CloseDate.AddDays(7)))
                    .Include(s => s.Questions.Where(q => DateTime.Now < q.CloseDate.AddDays(7) && DateTime.Now > q.OpenDate))
                    .ThenInclude(q => q.Responses)
                    .ToListAsync();

                return View("StudentIndex", subjectsStudent);
            }
        }

        public async Task<IActionResult> IndexRedirect(int? id)
        {
            if (id != null)
            {
                return RedirectToAction("Index", "Response", new { id = id });
            }
            else
            {
                return View("Index");
            }
        }
        // GET: Subject/Create
        public IActionResult Create(int? id) // Subject Id passed in
        {
            var subject = _context.Subjects.Find(id);
            var startDate = new DateTime(2022, 7, 4);
            int days = ((int)((DateTime.Today - startDate).Days / 14) + 1) * 14;
            var openDate = startDate.AddDays(days);
            var closeDate = openDate.AddDays(12);
            var vm = new NewQuestionViewModel
            {
                Subject = subject,
                Question = new Question
                {
                    OpenDate = openDate,
                    CloseDate = closeDate
                }
            };
            return View(vm);
        }

        // POST: Subject/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewQuestionViewModel vm)
        {
            if (ModelState.IsValid)
            {
                int subjectId = vm.Subject.Id;
                var subject = _context.Subjects.Find(subjectId);
                subject.Questions.Add(vm.Question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vm.Question);
        }

        // GET: Question/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // GET: Question/Respond
        public async Task<IActionResult> Respond(int? id)
        {
            var question = await _context.Questions.Include(q => q.Responses).SingleOrDefaultAsync(q => q.Id == id);
            var userId = User.Identity.GetUserId();
            var user = _context.Users.Find(userId);
            var response = question.Responses.Where(r => r.Responder == user).FirstOrDefault();
            var vm = new NewResponseViewModel
            {
                Question = question,
                Response = response
            };
            return View(vm);
        }

        // Alternative to Respond when the question as closed
        public async Task<IActionResult> Review(int? id)
        {
            var question = await _context.Questions.Include(q => q.Responses).SingleOrDefaultAsync(q => q.Id == id);
            var userId = User.Identity.GetUserId();
            var user = _context.Users.Find(userId);
            var response = question.Responses.Where(r => r.Responder == user).FirstOrDefault();
            var vm = new NewResponseViewModel
            {
                Question = question,
                Response = response
            };
            return View(vm);
        }

        // POST: Question/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Respond(int? id, NewResponseViewModel viewModel)
        {
            var responseText = viewModel.Response.ResponseText;
            var question = await _context.Questions.Include(q => q.Responses).ThenInclude(r => r.Responder).SingleOrDefaultAsync(q => q.Id == id);
            var userId = User.Identity.GetUserId();
            var user = _context.Users.Find(userId);
            var response = question.Responses.Where(r => r.Responder == user).FirstOrDefault();
            if (!string.IsNullOrEmpty(responseText))
            {
                if (ModelState.IsValid)
                {
                    if (response == null)
                    {
                        response = new Response() { ResponseText = responseText, Responder = user }; // new!  need to know who responded
                        question.Responses.Add(response);
                    }
                    else
                    {
                        response.ResponseText = responseText; // user already set in here
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            var vm = new NewResponseViewModel
            {
                Question = question,
                Response = response
            };
            return View(vm);
        }

        // GET: Question/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Questions == null || !_context.Questions.Any())
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);

            return View(question);
        }

        // POST: Question/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Question question)
        {
            if (question == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(question);
        }

        // GET: Question/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Questions == null || !_context.Questions.Any())
            {
                return NotFound();
            }

            var question = await _context.Questions.FirstOrDefaultAsync(m => m.Id == id);

            return View(question);
        }

        // POST: Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Questions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Questions'  is null.");
            }
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
            return (_context.Questions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
