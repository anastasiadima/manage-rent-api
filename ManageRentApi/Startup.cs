using System.Text;
using System.Threading.Tasks;
using ManageRentApi.Helpers;
using ManageRentApi.Interfaces;
using ManageRentApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using System.Security.Claims;
using System;
using System.Net;

namespace ManageRentApi
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
               services.AddCors();
               var connectionString = "Server=DESKTOP-HH3PAF5;Database=ManageRentApp;Trusted_Connection=True;";
               services.AddDbContext<DataContext>(x => x.UseSqlServer(connectionString));
               services.AddAutoMapper(typeof(Startup));
               ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
               services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

               var appSettingsSection = Configuration.GetSection("AppSettings");
               services.Configure<Setting>(appSettingsSection);

               // configure jwt authentication
               var appSettings = appSettingsSection.Get<Setting>();
               var key = Encoding.ASCII.GetBytes(appSettings.Secret);
               services.AddAuthentication(x =>
               {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               })
               .AddJwtBearer(x =>
               {
                    x.Events = new JwtBearerEvents
                    {
                         OnTokenValidated = context =>
                         {
                              var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                              var userId = int.Parse(context.Principal.Identity.Name);
                              var user = userService.GetById(userId);
                              if (user == null)
                              {
                                // return unauthorized if user no longer exists
                                context.Fail("Unauthorized");
                              }
                              var tokenDescriptor = new SecurityTokenDescriptor
                              {
                                   Subject = new ClaimsIdentity(new Claim[]
                                   {
                               new Claim(ClaimTypes.Name, user.Id.ToString())
                                         }),
                                   Expires = DateTime.UtcNow.AddDays(7),
                                   SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                              };
                              return Task.CompletedTask;
                         }
                    };
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(key),
                         ValidateIssuer = false,
                         ValidateAudience = false
                    };
               });

               services.AddScoped<IUserService, UserService>();
               services.AddScoped<IPaymentService, PaymentService>();
               services.AddScoped<IHouseRepository, HouseRepository>();
               services.AddScoped<ITenantRepository, TenantRepository>();
               services.AddScoped<IPlanRepository, PlanRepository>();
               services.AddScoped<ITenantPlanRepository, TenantPlanRepository>();
               services.AddScoped<IMailService, MailService>();
               services.AddScoped<ITenantService, TenantService>();
          }

          // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
          public void Configure(IApplicationBuilder app, IHostingEnvironment env)
          {
               app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

               app.UseAuthentication();

               app.UseMvc();
          }
     }
}
