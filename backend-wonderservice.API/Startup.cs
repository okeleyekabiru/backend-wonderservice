using System;
using System.Text;
using AutoMapper;
using backend_wonderservice.API.SignalR;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Infrastructure;
using backend_wonderservice.DATA.Infrastructure.Cloudinary;
using backend_wonderservice.DATA.Models;
using backend_wonderservice.DATA.Repo;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Plugins.JwtHandler;
using WonderService.Data.Repo;
using WonderService.Data.Services;
using System.Reflection;
using System.IO;
using Microsoft.OpenApi.Models;

namespace backend_wonderservice.API
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
            

            services.AddTransient<IMailService, EmailService>();
            services.AddScoped<IUser, UserRepo>();
            services.AddScoped<ICustomerOrder, CustomerOrderRepo>();
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddSignalR();
            services.AddScoped<IStateRepo, StateRepo>();
            services.AddCors(opt =>
            {
                opt.AddPolicy("corsPolicy", builder =>
                {
                    builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200").Build();
                });
            });
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetSection("Wonder").Value, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
            services.AddHangfireServer();
            services.AddMvc();
            services.AddScoped<IServiceType, ServiceTypeRepo>();
            services.AddScoped<IServiceUpload, ServicesUploadRepo>();
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            services.AddScoped<IPhoto, PhotoRepo>();

          
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetSection("Wonder").Value);

            });
            services.AddAutoMapper(typeof(CreateProfile));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("jwtHandler").Value));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(opt =>
                {
                    opt.SaveToken = true;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateAudience = false,
                        ValidateIssuer = false
                    };


                });
            services.AddScoped<IJwtSecurity, JwtGenrator>();
            services.AddScoped<INotification, Notification>();
            services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = true;


            }).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
           
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            try
            {
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
                    context.Database.Migrate();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            // app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "wonder service v1");
                c.InjectStylesheet("/swagger-ui/custom.css");

            });
            app.UseStaticFiles();
            app.UseHangfireDashboard();
            app.UseRouting();
            app.UseCors("corsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            
            Hangfire.BackgroundJob.Enqueue(() => Console.Write("running"));
            new BackgroundJob.BackgroundJob();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapHub<NotificationHUb>("/notification");
            });
        }
    }
}
