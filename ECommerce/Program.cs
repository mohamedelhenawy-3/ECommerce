using DataBaseAccess;
using ECommerce.Repository.CategoryRepo;
using ECommerce.Utilty;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using Stripe;
using NuGet.Protocol.Core.Types;

namespace ECommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var ConnectionStrings = builder.Configuration.GetConnectionString("defult");

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDBContext>(options =>
            {
                options.UseSqlServer(ConnectionStrings);
            });
            // Register Identity services (UserManager, SignInManager)
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true; // Enforce unique email
            })
             .AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddScoped<ICategoryRepository, CategoryRepositorycs>();
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

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
            StripeConfiguration.ApiKey ="sk_test_51QD6R7007c2glpwVgdpUpzHdF1daBhjnw1uDw1XNQb6YQAb6MISrvBGmEcO9jpG83FN26SrLTz0L2LleaQ3Qy4aq00waLCfnrV";

            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
