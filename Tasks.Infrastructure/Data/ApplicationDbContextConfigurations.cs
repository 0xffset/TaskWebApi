using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Taks.Core.Entities.General;

namespace Tasks.Infrastructure.Data
{
    public class ApplicationDbContextConfigurations
    {
        public static void Configure(ModelBuilder builder)
        {
            // Configure custom entities
            _ = builder.Entity<User>().ToTable("Users");
            _ = builder.Entity<Role>().ToTable("Roles");

            // Configure Identity entities
            _ = builder.Entity<IdentityUserRole<int>>()
                        .ToTable("UserRoles")
                        .HasKey(p => new { p.UserId, p.RoleId });

           

            _ = builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            _ = builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins").HasKey(p => new { p.LoginProvider, p.ProviderKey });
            _ = builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens").HasKey(p => new { p.UserId, p.LoginProvider, p.Name });
            _ = builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");


        }
    }
}
