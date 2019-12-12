using Blazored.LocalStorage;
using BlazorAuthTest.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAuthTest
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<AuthenticationStateProvider, SPAAuthticateProvider>();
            services.AddScoped<ILocalStorageService, LocalStorageService>();
            services.AddScoped<IAuthService, DummyAuthService>();

            services.AddAuthorizationCore(config =>
            {
                config.AddPolicy("IsAdmin", policy => policy.RequireRole("Admin", "SuperUser"));
            });
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
