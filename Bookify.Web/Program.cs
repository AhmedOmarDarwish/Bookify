namespace Bookify.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            builder.Services.Configure<SecurityStampValidatorOptions>(
                options => options.ValidationInterval = TimeSpan.Zero);

            builder.Services.AddDataProtection().SetApplicationName(nameof(Bookify));
            builder.Services.AddSingleton<IHashids>(_ => new Hashids("f1nd1ngn3m0", minHashLength: 11));

            builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
            builder.Services.AddTransient<IImageService, ImageService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddTransient<IEmailBodyBuilder, EmailBodyBuilder>();


            builder.Services.AddControllersWithViews();

            builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
            builder.Services.AddWhatsAppApiClient(builder.Configuration);

            builder.Services.AddExpressiveAnnotations();

            builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            builder.Services.AddHangfireServer();

            builder.Services.Configure<AuthorizationOptions>(
                options => options.AddPolicy("AdminsOnly", policy => 
                { 
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(AppRoles.Admin);
                }
                ));
            builder.Services.AddViewToHTML();

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

            //app.UseStatusCodePages(async statusCodeContext =>
            //{
            //	// using static System.Net.Mime.MediaTypeNames;
            //	statusCodeContext.HttpContext.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;

            //	await statusCodeContext.HttpContext.Response.WriteAsync(
            //		$"Status Code Page: {statusCodeContext.HttpContext.Response.StatusCode}");
            //});

            //app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");
            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

            app.UseHttpsRedirection();
            app.UseRouting();

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
