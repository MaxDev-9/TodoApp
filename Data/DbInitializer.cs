using Microsoft.AspNetCore.Identity;
using TodoApp.Models;

namespace TodoApp.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var db = services.GetRequiredService<ApplicationDbContext>();

            await db.Database.EnsureCreatedAsync();

            // Seed roles
            string[] roles = ["Admin", "User"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed admin
            const string adminEmail = "admin@todoapp.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    FullName = "Administrator",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin@123456");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed demo user
            const string userEmail = "demo@todoapp.com";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var user = new ApplicationUser
                {
                    FullName = "Demo User",
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "Demo@123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                    // Add sample data
                    var list = new TodoList
                    {
                        Title = "Getting Started",
                        Description = "My first todo list",
                        Color = "#6C63FF",
                        UserId = user.Id,
                        DueDate = DateTime.UtcNow.AddDays(7),
                        Tasks = new List<TodoTask>
                        {
                            new() { Title = "Explore the dashboard", Priority = TaskPriority.High, DueDate = DateTime.UtcNow.AddDays(1) },
                            new() { Title = "Create your first list", Priority = TaskPriority.Medium, DueDate = DateTime.UtcNow.AddDays(2) },
                            new() { Title = "Add tasks to your list", Priority = TaskPriority.Low, DueDate = DateTime.UtcNow.AddDays(3) },
                        }
                    };
                    db.TodoLists.Add(list);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
