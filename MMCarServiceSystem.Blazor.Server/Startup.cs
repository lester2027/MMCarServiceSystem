using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.EntityFrameworkCore;
using MMCarServiceSystem.Blazor.Server.Services;

namespace MMCarServiceSystem.Blazor.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(Microsoft.AspNetCore.SignalR.HubConnectionHandler<>), typeof(ProxyHubConnectionHandler<>));

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpContextAccessor();
            services.AddScoped<CircuitHandler, CircuitHandlerProxy>();
            services.AddXaf(Configuration, builder =>
            {
                builder.UseApplication<MMCarServiceSystemBlazorApplication>();
                builder.Modules
                    .AddConditionalAppearance()
                    .AddValidation(options =>
                    {
                        options.AllowValidationDetailsAccess = false;
                    })
                    .Add<MMCarServiceSystem.Module.MMCarServiceSystemModule>()
                    .Add<MMCarServiceSystemBlazorModule>();
                builder.ObjectSpaceProviders
                    .AddSecuredEFCore(options =>
                    {
                        options.PreFetchReferenceProperties();
                    })
                    .WithDbContext<MMCarServiceSystem.Module.BusinessObjects.MMCarServiceSystemEFCoreDbContext>((serviceProvider, options) =>
                    {
                        string connectionString = null;
                        if (Configuration.GetConnectionString("ConnectionString") != null)
                        {
                            connectionString = Configuration.GetConnectionString("ConnectionString");
                        }
#if EASYTEST
                        if(Configuration.GetConnectionString("EasyTestConnectionString") != null) {
                            connectionString = Configuration.GetConnectionString("EasyTestConnectionString");
                        }
#endif
                        ArgumentNullException.ThrowIfNull(connectionString);
                        options.UseConnectionString(connectionString);
                    })
                    .AddNonPersistent();
                builder.Security
                    .UseIntegratedMode(options =>
                    {
                        options.Lockout.Enabled = true;

                        options.RoleType = typeof(PermissionPolicyRole);
                        options.UserType = typeof(MMCarServiceSystem.Module.BusinessObjects.ApplicationUser);
                        options.UserLoginInfoType = typeof(MMCarServiceSystem.Module.BusinessObjects.ApplicationUserLoginInfo);
                        options.Events.OnSecurityStrategyCreated += securityStrategy =>
                        {
                            ((SecurityStrategy)securityStrategy).PermissionsReloadMode = PermissionsReloadMode.NoCache;
                        };
                    })
                    .AddPasswordAuthentication(options =>
                    {
                        options.IsSupportChangePassword = true;
                    });
            });
            var authentication = services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
            authentication.AddCookie(options =>
            {
                options.LoginPath = "/LoginPage";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRequestLocalization();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();
            app.UseXaf();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapXafEndpoints();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllers();
            });
        }
    }
}
