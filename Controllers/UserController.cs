using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QHub.Classes;
using QHub.Data;
using QHub.Models;
using QHub.ViewModels;
using System.Security.Claims;

namespace QHub.Controllers
{
    [Authorize(Roles = "Admin, Teacher")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: Subject
        public async Task<IActionResult> Index()
        {
            if (_context.Users != null)
            {
                var users = await _context.Users.Include(u => u.Subjects).ToListAsync();
                // HACK (slow and nasty)
                foreach(var user in users)
                {
                    user.IsTeacher = await _userManager.IsInRoleAsync(user, Enumerations.QHubRoles.Teacher.ToString());
                }
                return View(users);
            }
            else
            {
                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
        }

        // GET: Subject/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Include(u => u.Subjects).SingleOrDefaultAsync(u => u.Id == id);
            var subjects = _context.Subjects.ToList(); // this is needed to be able to set the user into any subject (or likewise remove them)
            foreach (var subject in subjects)
            {
                subject.IsSelected = user.Subjects.Contains(subject); // tick the ones the user is in
            }
            if (user == null)
            {
                return NotFound();
            }
            bool isTeacher = await _userManager.IsInRoleAsync(user, Enumerations.QHubRoles.Teacher.ToString());
            var vm = new EditUserViewModel
            {
                User = user,
                Subjects = subjects,
                IsTeacher = isTeacher
            };
            return View(vm);
        }

        // POST: Subject/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, ApplicationUser user, IEnumerable<Subject> subjects, bool IsTeacher)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user2 = await _context.Users.Include(u => u.Subjects).SingleOrDefaultAsync(u => u.Id == id);
                    //user2.UserName = user.UserName; // Not required and dangerous!
                    user2.Email = user.Email;
                    user2.FirstName = user.FirstName;
                    user2.LastName = user.LastName;

                    bool isCurrentTeacher = await _userManager.IsInRoleAsync(user2, Enumerations.QHubRoles.Teacher.ToString());
                    // promote or demote status of teacher
                    if (IsTeacher && !isCurrentTeacher)
                    {
                        await _userManager.RemoveFromRolesAsync(user2, new string[] { Enumerations.QHubRoles.Student.ToString() });
                        await _userManager.AddToRoleAsync(user2, Enumerations.QHubRoles.Teacher.ToString());
                    }
                    if (!IsTeacher && isCurrentTeacher)
                    {
                        await _userManager.RemoveFromRolesAsync(user2, new string[] { Enumerations.QHubRoles.Teacher.ToString() });
                        await _userManager.AddToRoleAsync(user2, Enumerations.QHubRoles.Student.ToString());
                    }

                    // Deal with any change to enrolments
                    foreach (var subject in subjects)
                    {
                        var subject2 = _context.Subjects.Find(subject.Id); // Need to retrieve the actual object for comparison
                        // The subject was added to the user (provided they didn't already have it
                        if (subject.IsSelected && !user2.Subjects.Contains(subject2))
                        {
                            user2.Subjects.Add(subject2);
                        }
                        // The subject was deselected for the user, so remove it if they do have it
                        if (!subject.IsSelected && user2.Subjects.Contains(subject2))
                        {
                            user2.Subjects.Remove(subject2);
                        }
                    }
                    _context.Update(user2);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            var vm = new EditUserViewModel
            {
                User = user,
                Subjects = _context.Subjects.ToList()
            };
            return View(vm);
        }

        // GET: Subject/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        // POST: Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
            var user = await _context.Users.Include(u => u.Subjects).FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                foreach (var subject in user.Subjects.ToList())
                {
                    user.Subjects.Remove(subject);
                }
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
