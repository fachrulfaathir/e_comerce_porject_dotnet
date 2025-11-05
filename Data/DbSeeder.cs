using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectEcomerceFinal.Constants;

namespace ProjectEcomerceFinal.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {


            var context = service.GetService<ApplicationDbContext>();

            // this block will check if there are any pending migrations and apply them
            if ((await context.Database.GetPendingMigrationsAsync()).Count() > 0)
            {
                await context.Database.MigrateAsync();
            }

            var userMgr = service.GetService<UserManager<IdentityUser>>();
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();

            // create admin role if not exists
            var adminRoleExists = await roleMgr.RoleExistsAsync(Roles.Admin.ToString());

            if (!adminRoleExists)
            {
                await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            }

            // create user role if not exists
            var userRoleExists = await roleMgr.RoleExistsAsync(Roles.User.ToString());

            if (!userRoleExists)
            {
                await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));
            }

            // 1. Pastikan role ada
            if (!await roleMgr.RoleExistsAsync(Roles.Admin.ToString()))
                await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));

            if (!await roleMgr.RoleExistsAsync(Roles.User.ToString()))
                await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));

            // 2. Fix typo email
            const string email = "fachrulfai3@gmail.com"; // Bukan gamil.com

            var admin = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };

            // 3. Cek apakah user sudah ada
            var userInDb = await userMgr.FindByEmailAsync(email);
            if (userInDb != null)
                return; // sudah ada, skip

            // 4. Buat user + CEK HASIL
            var createResult = await userMgr.CreateAsync(admin, "Admin123@");
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Gagal buat user: {errors}");
            }

            // 5. Add role + pastikan user sudah punya Id
            var roleResult = await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Gagal assign role: {errors}");
            }
        }
    }
}
