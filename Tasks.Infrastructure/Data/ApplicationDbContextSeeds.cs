using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Taks.Core.Entities.General;
using Task = System.Threading.Tasks.Task;

namespace Tasks.Infrastructure.Data
{
    public class ApplicationDbContextSeeds
    {
        public static async Task SeedsAsync(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryFor = retry ?? 0;
            ApplicationDbContext appContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            UserManager<User> UserManager = serviceProvider.GetRequiredService<UserManager<User>>();

            try
            {
                // Adding Roles
                if (!appContext.Roles.Any())
                {
                    using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = appContext.Database.BeginTransaction();
                    appContext.Roles.AddRange(Roles());
                    _ = await appContext.SaveChangesAsync();
                    transaction.Commit();
                }

                // Adding users 
                if (!appContext.Users.Any())
                {
                    User defaultUser = new()
                    {
                        FullName = "Rogger G. Díaz",
                        UserName = "rogger",
                        RoleId = 1,
                        Email = "roggergarciadiaz@gmail.com",
                        EntryDate = DateTime.Now,
                        IsActive = true,

                    };
                    IdentityResult identityResult = await UserManager.CreateAsync(defaultUser, "Passw0rd#");

                    if (identityResult.Succeeded)
                    {
                        // Assing the new role
                        _ = await UserManager.AddToRoleAsync(defaultUser, "ADMIN");
                    }

                }
                // Adding Categories
                if (!appContext.TaskCategories.Any())
                {
                    using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = appContext.Database.BeginTransaction();
                    appContext.TaskCategories.AddRange(TaskCategories());
                    _ = await appContext.SaveChangesAsync();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {

                if (retryFor < 10)
                {
                    retryFor++;
                    ILogger<ApplicationDbContextSeeds> log = loggerFactory.CreateLogger<ApplicationDbContextSeeds>();
                    log.LogError(ex.Message);
                    await SeedsAsync(serviceProvider, loggerFactory, retryFor);
                }
            }
        }

        private static IEnumerable<Role> Roles()
        {
            return new List<Role>
            {
                new() {Code="ADMIN", Name = "Admin", NormalizedName="ADMIN", IsActive = true, EntryDate= DateTime.Now },
                new() {Code="USER", Name = "User", NormalizedName= "USER", IsActive = true, EntryDate= DateTime.Now },
            };
        }

        private static IEnumerable<TaskCategory> TaskCategories()
        {
            Faker<TaskCategory> faker = new Faker<TaskCategory>()
               .RuleFor(c => c.Name, f => f.Commerce.Product())
               .RuleFor(c => c.Status, true);

            return faker.Generate(50);

        }
    }
}


