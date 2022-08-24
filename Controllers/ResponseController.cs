using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QHub.Data;
using QHub.Models;
using QHub.ViewModels;

namespace QHub.Controllers
{
    public class ResponseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResponseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Response
        public async Task<IActionResult> Index(int? id)
        {
            // If question passed in, show all responses for that question
            if (id != null)
            {
                var question = await _context.Questions.Include(q => q.Responses).ThenInclude(r => r.Responder).SingleOrDefaultAsync(q => q.Id == id);
                if (question == null)
                    return NotFound();

                // Make into a list with a single entry so I can double up the view
                var questions = new List<Question>();
                questions.Add(question);

                return View(questions);
            }
            else
            // else return all responses given by the current user with edit rights if eligible
            {
                var userId = User.Identity.GetUserId();
                var user = _context.Users.Find(userId);
                var enrolledSubjects = _context.Subjects.Where(s => s.Users.Contains(user));
                var questions = enrolledSubjects.SelectMany(s => s.Questions).Include(q => q.Responses).ToList();
                return View(questions);
            }
        }

        // GET: Response/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Responses == null)
            {
                return NotFound();
            }

            var response = await _context.Responses.FindAsync(id);
            if (response == null)
            {
                return NotFound();
            }

            return View(response);
        }

        // GET: Response/Create
        //public IActionResult Respond()
        //{
        //    var userId = User.Identity.GetUserId();

        //    var user = _context.Users.Find(userId);
        //    var enrolledSubjects = _context.Subjects.Where(s => s.Users.Contains(user));
        //    var questions = enrolledSubjects.SelectMany(s => s.Questions);
        //    if (!questions.Any()) return NotFound();

        //    var viewModel = new NewResponseViewModel()
        //    {
        //        Question = questions.First(),
        //        StudentResponse = new Response()
        //        {
        //            Comments = "",
        //            Mark = 0,
        //            ResponseText = ""
        //        }
        //    };
        //    return View(viewModel);
        //}

        // POST: Response/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(List<Question> model)
        {
            if (ModelState.IsValid)
            {
                //var userId = User.Identity.GetUserId();
                //var user = _context.Users.Find(userId);
                //viewModel.StudentResponse.Responder = user;
                //var question = await _context.Questions.FindAsync(id);
                //question.Responses.Add(viewModel.StudentResponse);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index), new { id });
            }
            return View(model);
        }

        // GET: Response/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Responses == null)
            {
                return NotFound();
            }

            try
            {
                var response = await _context.Responses.Include(r => r.Responder).FirstOrDefaultAsync(r => r.Id == id);
                // Backtrack to the source question
                var question = _context.Questions.Where(q => q.Responses.Contains(response)).FirstOrDefault();
                var vm = new NewResponseViewModel
                {
                    Question = question,
                    Response = response
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Response/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Response Response)
        {
            if (ModelState.IsValid)
            {
                // Retrieve full record and add newf fields
                var r = _context.Responses.Find(id);
                r.Comments = Response.Comments; // Refill the fields
                r.Mark = Convert.ToDouble(Response.Mark);
                r.ResponseText = Response.ResponseText;

                _context.Update(r);
                await _context.SaveChangesAsync();
                Question question = _context.Questions.Where(q => q.Responses.Contains(r)).FirstOrDefault();
                return RedirectToAction(nameof(Index), new { id = question.Id });
            }
            var response2 = await _context.Responses.Include(r => r.Responder).SingleOrDefaultAsync(r => r.Id == id);
            if (response2 == null)
            {
                return NotFound();
            }
            // Backtrack to the source question
            var question2 = _context.Questions.Where(q => q.Responses.Contains(response2)).FirstOrDefault();
            var vm2 = new NewResponseViewModel
            {
                Question = question2,
                Response = response2
            };
            return View(vm2);
        }

        // GET: Response/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Responses == null)
            {
                return NotFound();
            }

            var response = await _context.Responses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (response == null)
            {
                return NotFound();
            }

            return View(response);
        }

        // POST: Response/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Responses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Responses'  is null.");
            }
            var response = await _context.Responses.FindAsync(id);
            if (response != null)
            {
                _context.Responses.Remove(response);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResponseExists(int id)
        {
            return (_context.Responses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
