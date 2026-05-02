using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class TodoController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private async Task<string> GetUserIdAsync() =>
            (await _userManager.GetUserAsync(User))!.Id;

        public async Task<IActionResult> Index()
        {
            var userId = await GetUserIdAsync();
            var lists = await _db.TodoLists
                .Include(l => l.Tasks)
                .Where(l => l.UserId == userId && !l.IsArchived)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
            return View(lists);
        }

        public async Task<IActionResult> Calendar()
        {
            var userId = await GetUserIdAsync();
            var tasks = await _db.TodoTasks
                .Include(t => t.TodoList)
                .Where(t => t.TodoList.UserId == userId && t.DueDate != null)
                .ToListAsync();
            var lists = await _db.TodoLists
                .Where(l => l.UserId == userId)
                .ToListAsync();
            return View(new CalendarViewModel { Tasks = tasks, Lists = lists });
        }

        // TodoList CRUD
        [HttpGet]
        public IActionResult CreateList() => View(new CreateTodoListViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateList(CreateTodoListViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var userId = await GetUserIdAsync();
            _db.TodoLists.Add(new TodoList
            {
                Title = model.Title,
                Description = model.Description,
                Color = model.Color,
                DueDate = model.DueDate,
                UserId = userId
            });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditList(int id)
        {
            var userId = await GetUserIdAsync();
            var list = await _db.TodoLists.FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
            if (list == null) return NotFound();
            return View(new CreateTodoListViewModel
            {
                Title = list.Title,
                Description = list.Description,
                Color = list.Color,
                DueDate = list.DueDate
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditList(int id, CreateTodoListViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var userId = await GetUserIdAsync();
            var list = await _db.TodoLists.FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
            if (list == null) return NotFound();
            list.Title = model.Title;
            list.Description = model.Description;
            list.Color = model.Color;
            list.DueDate = model.DueDate;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteList(int id)
        {
            var userId = await GetUserIdAsync();
            var list = await _db.TodoLists.FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
            if (list != null)
            {
                _db.TodoLists.Remove(list);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Task CRUD
        public async Task<IActionResult> ListDetail(int id)
        {
            var userId = await GetUserIdAsync();
            var list = await _db.TodoLists
                .Include(l => l.Tasks)
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
            if (list == null) return NotFound();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateTask(int listId)
        {
            var userId = await GetUserIdAsync();
            var list = await _db.TodoLists.FirstOrDefaultAsync(l => l.Id == listId && l.UserId == userId);
            if (list == null) return NotFound();
            return View(new CreateTaskViewModel { TodoListId = listId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(CreateTaskViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.TodoTasks.Add(new TodoTask
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Priority = model.Priority,
                Notes = model.Notes,
                TodoListId = model.TodoListId
            });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ListDetail), new { id = model.TodoListId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTask(int id)
        {
            var userId = await GetUserIdAsync();
            var task = await _db.TodoTasks
                .Include(t => t.TodoList)
                .FirstOrDefaultAsync(t => t.Id == id && t.TodoList.UserId == userId);
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListDetail), new { id = task?.TodoListId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = await GetUserIdAsync();
            var task = await _db.TodoTasks
                .Include(t => t.TodoList)
                .FirstOrDefaultAsync(t => t.Id == id && t.TodoList.UserId == userId);
            if (task != null)
            {
                int listId = task.TodoListId;
                _db.TodoTasks.Remove(task);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(ListDetail), new { id = listId });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditTask(int id)
        {
            var userId = await GetUserIdAsync();
            var task = await _db.TodoTasks
                .Include(t => t.TodoList)
                .FirstOrDefaultAsync(t => t.Id == id && t.TodoList.UserId == userId);
            if (task == null) return NotFound();
            return View(new CreateTaskViewModel
            {
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Notes = task.Notes,
                TodoListId = task.TodoListId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTask(int id, CreateTaskViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var userId = await GetUserIdAsync();
            var task = await _db.TodoTasks
                .Include(t => t.TodoList)
                .FirstOrDefaultAsync(t => t.Id == id && t.TodoList.UserId == userId);
            if (task == null) return NotFound();
            task.Title = model.Title;
            task.Description = model.Description;
            task.DueDate = model.DueDate;
            task.Priority = model.Priority;
            task.Notes = model.Notes;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ListDetail), new { id = task.TodoListId });
        }
    }
}
