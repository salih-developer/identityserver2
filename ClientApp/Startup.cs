using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApplicationBasic
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddAuthentication(options =>
                     {
                         options.DefaultScheme = "Cookies";
                         options.DefaultChallengeScheme = "oidc";
                     })
                    .AddCookie("Cookies")
                    .AddOpenIdConnect("oidc", options =>
                     {
                         options.SignInScheme = "Cookies";
                         options.Authority = "http://localhost:5000";
                         options.RequireHttpsMetadata = false;
                         options.ClientId = "mvc";
                         options.ClientSecret = "secret";
                         options.ResponseType = "code id_token";
                         options.SaveTokens = true;
                         options.GetClaimsFromUserInfoEndpoint = true;
                         options.Scope.Add("apiApp");
                         options.Scope.Add("offline_access");
                     });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                //    HotModuleReplacement = true
                //});
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationScheme = "Cookies"
            //});

            //app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            //{
            //    AuthenticationScheme = "oidc",
            //    SignInScheme = "Cookies",

            //    Authority = "http://localhost:5000",
            //    RequireHttpsMetadata = false,

            //    ClientId = "mvc",
            //    ClientSecret = "secret",

            //    ResponseType = "code id_token",
            //    Scope = { "apiApp", "offline_access" },

            //    GetClaimsFromUserInfoEndpoint = true,
            //    SaveTokens = true
            //});

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
