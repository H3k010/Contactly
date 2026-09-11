using System.Globalization;
using ContactsManager.Application;
using ContactsManager.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace ContactsManager.Presentation;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
            .AddViewLocalization()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (_, factory) =>
                    factory.Create(typeof(ValidationResource));
            });

        builder.Services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });

        builder.Services.AddInfrastructureServices(builder.Configuration);

        var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ar") };
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("en");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        var webRoot = app.Environment.WebRootPath; 
        var relativePath = !app.Environment.IsProduction() ? "lib/Rotativa" : "lib/hosting/Rotativa";
        RotativaConfiguration.Setup(webRoot, wkhtmltopdfRelativePath: relativePath);
        
        app.UseHttpsRedirection();
        app.UseRequestLocalization();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSession();

        app.MapControllers();

        app.Run();
    }
}