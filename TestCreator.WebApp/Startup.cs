using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using TestCreator.Data.Context;
using TestCreator.Data.Models;
using TestCreator.Data.Repositories;
using TestCreator.Data.Repositories.Interfaces;
using TestCreator.WebApp.Broadcast;
using TestCreator.WebApp.Converters;
using TestCreator.WebApp.Converters.Interfaces;
using TestCreator.WebApp.Services;
using TestCreator.WebApp.Services.Interfaces;

namespace TestCreator.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(config =>
                {
                    config.RequireHttpsMetadata = false;
                    config.SaveToken = true;
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["Auth:Jwt:Issuer"],
                        ValidAudience = Configuration["Auth:Jwt:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero,

                        //security
                        RequireExpirationTime = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = true
                    };
                });

            services.AddSignalR();

            //register dependencies
            services.Add(new ServiceDescriptor(typeof(ITestAttemptViewModelConverter), typeof(TestAttemptViewModelConverter), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(ITokenService), typeof(TokenService), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(ITestCalculationService), typeof(TestCalculationService), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IUserAndRoleRepository), typeof(UserAndRoleRepository), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(ITestRepository), typeof(TestRepository), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IResultRepository), typeof(ResultRepository), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IQuestionRepository), typeof(QuestionRepository), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IAnswerRepository), typeof(AnswerRepository), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(ITokenRepository), typeof(TokenRepository), ServiceLifetime.Scoped));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                //#if DEBUG
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                //{
                //    HotModuleReplacement = true
                //});
                //#endif
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            //disable cache for static files
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (context =>
                    {
                        context.Context.Response.Headers["Cache-Control"] = Configuration["StaticFiles:Headers:Cache-Control"];
                        context.Context.Response.Headers["Pragma"] = Configuration["StaticFiles:Headers:Pragma"];
                        context.Context.Response.Headers["Expires"] = Configuration["StaticFiles:Headers:Expires"];
                    })
            });
            app.UseSpaStaticFiles();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}");

                //endpoints.MapFallbackToController("Index", "Home");
                endpoints.MapHub<TestsHub>("/testsHub");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                context?.Database.EnsureCreated();
                context?.Database.Migrate();

                DbSeeder.Seed(context, roleManager, userManager);
            }
        }
    }
}
