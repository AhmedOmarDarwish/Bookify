namespace Bookify.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddBookifyServices(builder);
            //Add Serilog
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseExceptionHandler("/Home/Error");
            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.Always
            });

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "Deny");

                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            //Add Seed User and Roles
            var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

            using var scope = scopeFactory.CreateScope();

            var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManger = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await DefaultRoles.SeedAsync(roleManger);
            await DefaultUsers.SeedAdminUserAsync(userManger);

            //hangfire
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                DashboardTitle = "Bookify Dashboard",
                //IsReadOnlyFunc = (DashboardContext context) => true,
                Authorization = new IDashboardAuthorizationFilter[]
                {
                     new HangfireAuthorizationFilter("AdminsOnly")
                }
            });

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var webHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var whatsAppClient = scope.ServiceProvider.GetRequiredService<IWhatsAppClient>();
            var emailBodyBuilder = scope.ServiceProvider.GetRequiredService<IEmailBodyBuilder>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var hangfireTasks = new HangfireTasks(dbContext, webHostEnvironment, whatsAppClient,
                emailBodyBuilder, emailSender);

            RecurringJob.AddOrUpdate(
                "prepare-expiration-alert",
                () => hangfireTasks.PrepareExpirationAlert(),
                Cron.Daily(14), // 2 PM daily (cleaner than raw cron)
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local }
            );
            RecurringJob.AddOrUpdate(
                "rentals-expiration-alert",
                () => hangfireTasks.RentalsExpirationAlert(),
                Cron.Daily(14), // 2 PM daily (cleaner than raw cron)
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local }
            );

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
