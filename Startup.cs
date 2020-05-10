using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CusJoWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CusJoWebAPI
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
            services.AddControllers();
            services.AddDbContext<CusJoAPIContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CusCon")));
            services.AddScoped(typeof(CusJoAPIContext), typeof(CusJoAPIContext));
            services.AddScoped(typeof(AppSettings), typeof(AppSettings));
            services.AddScoped(typeof(UserManager<ApplicationUser>), typeof(UserManager<ApplicationUser>));
            services.AddScoped(typeof(RoleManager<IdentityRole>), typeof(RoleManager<IdentityRole>));
            services.AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<CusJoAPIContext>().AddDefaultTokenProviders();
            services.AddEntityFrameworkSqlServer();
            services.AddAutoMapper(c => c.AddProfile<AutoMapperProfile>(), typeof(Startup));
            //var appSettingsSection = Configuration.GetSection("AppSettings");
            //services.Configure<AppSettings>(appSettingsSection);
            //var appSettings = appSettingsSection.Get<AppSettings>();
            //var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //           .AddJwtBearer(x =>
            //           {
            //               x.Audience = "http://localhost:49631";
            //               x.Authority = "http://localhost:49631";
            //               x.RequireHttpsMetadata = false;
            //               x.SaveToken = true;
            //               x.TokenValidationParameters = new TokenValidationParameters
            //               {
            //                   ValidateIssuerSigningKey = true,
            //                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcdefghijklmnopqrstuvwxyz")),
            //                   ValidateIssuer = false,
            //                   ValidateAudience = false
            //               };
            //           });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
              AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyz")),
                      ValidateIssuer = false,
                      ValidateAudience = false



                  };

              });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
         
           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
