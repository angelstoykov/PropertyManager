using Microsoft.EntityFrameworkCore;
using PropertyManager.Application.Services;
using PropertyManager.Application.Services.Contracts;
using PropertyManager.Data;
using PropertyManager.Data.Infrastructure.Persistence;
using PropertyManager.WEB.ApiClients.Contracts;
using System;

namespace PropertyManager.WEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<PropertyManagerDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpClient<IPropertyApiClient, PropertyApiClient>(PropertyApiClient =>
            {
                PropertyApiClient.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"]!);
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PropertyManagerDbContext>();
                DbInitializer.SeedAsync(db);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Units}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
