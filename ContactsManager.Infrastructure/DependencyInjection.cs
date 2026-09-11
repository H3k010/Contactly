using ContactsManager.Application.Interfaces;
using ContactsManager.Domain.Entities;
using ContactsManager.Infrastructure.Data;
using ContactsManager.Infrastructure.Repositories;
using ContactsManager.Infrastructure.Services;
using ContactsManager.Infrastructure.Services.Account;
using ContactsManager.Infrastructure.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySQL(configuration.GetConnectionString("DefaultConnection")!));
        
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

        services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<LocalizedIdentityErrorDescriber>();
        
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.LoginPath = "/Account/Login"; 
        });
        
        services.AddHttpContextAccessor();
        services.AddSession();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAccountVerificationService, AccountVerificationService>();
        services.AddScoped<IStepUpAuthentication, StepUpAuthentication>();
        services.AddScoped<IContactsRepository, ContactsRepository>();
        services.AddScoped<IContactsService, ContactsService>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));

        return services;
    }
}