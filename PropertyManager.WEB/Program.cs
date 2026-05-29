using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using PropertyManager.Data;
using PropertyManager.Data.Identity;
using PropertyManager.Data.Infrastructure.Persistence;
using PropertyManager.WEB.ApiClients;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.Infrastructure;

namespace PropertyManager.WEB
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<PropertyManagerDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;
                })
                .AddEntityFrameworkStores<PropertyManagerDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            builder.Services.AddAuthorization();

            builder.Services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<JwtBearerHandler>();

            var apiBaseUrl = builder.Configuration["Api:BaseUrl"]!;

            builder.Services.AddHttpClient<IPropertyApiClient, PropertyApiClient>(client =>
                    client.BaseAddress = new Uri(apiBaseUrl))
                .AddHttpMessageHandler<JwtBearerHandler>();

            builder.Services.AddHttpClient<IUnitsApiClient, UnitsApiClient>(client =>
                    client.BaseAddress = new Uri(apiBaseUrl))
                .AddHttpMessageHandler<JwtBearerHandler>();

            builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
                    client.BaseAddress = new Uri(apiBaseUrl))
                .AddHttpMessageHandler<JwtBearerHandler>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PropertyManagerDbContext>();
                await db.Database.MigrateAsync();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await IdentityDataSeeder.SeedAsync(userManager, roleManager);

                await DbInitializer.SeedAsync(db);
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Properties}/{action=Index}/{id?}");

            await app.RunAsync();
        }
    }
}
