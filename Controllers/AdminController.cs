using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.Include(u => u.TodoLists).ThenInclude(l => l.Tasks).ToListAsync();
            var summaries = new List<UserSummary>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                summaries.Add(new UserSummary
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    ListCount = user.TodoLists.Count,
                    TaskCount = user.TodoLists.Sum(l => l.Tasks.Count),
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList()
                });
            }

            return View(new AdminDashboardViewModel
            {
                TotalUsers = users.Count,
                TotalLists = await _db.TodoLists.CountAsync(),
                TotalTasks = await _db.TodoTasks.CountAsync(),
                CompletedTasks = await _db.TodoTasks.CountAsync(t => t.IsCompleted),
                Users = summaries
            });
        }

        public async Task<IActionResult> UserDetail(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.TodoLists)
                .ThenInclude(l => l.Tasks)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, role))
                await _userManager.RemoveFromRoleAsync(user, role);
            else
                await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AllLists()
        {
            var lists = await _db.TodoLists.Include(l => l.Tasks).Include(l => l.User).ToListAsync();
            return View(lists);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteList(int id)
        {
            var list = await _db.TodoLists.FindAsync(id);
            if (list != null)
            {
                _db.TodoLists.Remove(list);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(AllLists));
        }
    }
}
