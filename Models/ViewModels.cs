using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class CreateTodoListViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Color { get; set; } = "#6C63FF";
        public DateTime? DueDate { get; set; }
    }

    public class CreateTaskViewModel
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public string? Notes { get; set; }
        public int TodoListId { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalLists { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public List<UserSummary> Users { get; set; } = new();
    }

    public class UserSummary
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int ListCount { get; set; }
        public int TaskCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class CalendarViewModel
    {
        public List<TodoTask> Tasks { get; set; } = new();
        public List<TodoList> Lists { get; set; } = new();
    }
}
