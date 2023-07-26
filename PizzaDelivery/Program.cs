global using PizzaDelivery.Domain.Models;
global using PizzaDelivery.Persistence;
global using PizzaDelivery.Domain.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Application.Services.Implementation;
using PizzaDelivery.Application.Services.Interfaces;
using GoogleProvider.Interfaces;
using PizzaDelivery.Application.Helpers;
using Microsoft.AspNetCore.Authentication.OAuth;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DockerComposePostgres");
builder.Services.AddDbContext<ApplicationDbContext>(
    options => {
        options.UseNpgsql(connectionString, builder =>
        {
            builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        });
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

#region Own Services
//services.AddSingleton<IWebHostEnvironment>(provider => (IWebHostEnvironment)provider.GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>());

builder.Services.AddTransient<IPizzaService, PizzaService>();
builder.Services.AddTransient<IPromocodeService, PromocodeService>();


builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.Configure<PasswordHasherOptions>(options =>
    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2);

builder.Services.AddTransient<IExternalProvider, GoogleProvider.ExternalProvider>();


#endregion



builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages();


builder.Services.AddIdentity<PizzaDelivery.Domain.Models.User.ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<PizzaDelivery.Persistence.ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
                .AddCookie(options =>
                {
                    options.LoginPath = "/external/google-login";
                })
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration.GetSection("GoogleOauth:ClientId").Value;
                    options.ClientSecret = builder.Configuration.GetSection("GoogleOauth:ClientSecret").Value;
                })
                .AddFacebook(options =>
                {
                    options.AppId = builder.Configuration.GetSection("FacebookOauth:AppId").Value;
                    options.AppSecret = builder.Configuration.GetSection("FacebookOauth:AppSecret").Value;

                })
                .AddOAuth("Vkontakte", options =>
                {
                    options.ClientId = builder.Configuration.GetSection("VkOauth:AppId").Value;
                    options.ClientSecret = builder.Configuration.GetSection("VkOauth:AppKey").Value;
                    options.CallbackPath = new PathString("/signin-vk-token");
                    options.AuthorizationEndpoint = "https://oauth.vk.com/authorize";
                    options.TokenEndpoint = "https://oauth.vk.com/access_token";
                   
                    options.SaveTokens = true;

                    options.Events = new OAuthEvents
                    {
                        OnRedirectToAuthorizationEndpoint = context =>
                        {
                            // Build the authorization URL with additional parameters if needed
                            var authorizationUrl = context.RedirectUri;
                            authorizationUrl += "&scope=photos,email,friends";
                            var code = context.Request.Query["code"];


                            context.Response.Redirect(authorizationUrl);
                            context.Response.CompleteAsync();
                            return Task.CompletedTask;
                        },
                        OnCreatingTicket = context =>
                        {
                            var accessToken = context.AccessToken;
                            var expiresTime = context.ExpiresIn.ToString();
                            
                            
                            context.HttpContext.Session.SetString("AccessToken", accessToken);
                            context.HttpContext.Session.SetString("ExpiresTime", expiresTime);

                            return Task.CompletedTask;
                        }
                        
                    };
                });


builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VKPolicy", policy =>
    {
        policy.AuthenticationSchemes.Add("Vkontakte");
        policy.RequireAuthenticatedUser();
    });
});

builder.Services.AddControllersWithViews();

//builder.Services.AddHostedService<RefreshVkTokenMiddleware>();

builder.Logging.AddConsole();


var app = builder.Build();

//var logger = app.Services.GetService<ILogger<RepeatingService>>();
//var service = new RepeatingService(logger,app.Services.GetService<IServiceScopeFactory>());
//service.StartAsync(CancellationToken.None);

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


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
