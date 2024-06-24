using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PiHire.API.Common;
using PiHire.API.Common.Hubs;
using PiHire.API.Common.Swagger;
using PiHire.API.Security;
using System.IO;

namespace PiHire.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder.WithOrigins(
                    "https://devpihireapp1o.paraminfo.online", "https://devpihirecp1o.paraminfo.online", "https://pihireapi1o.paraminfo.online",
                    "https://docs.google.com", "https://qapihireapp1o.paraminfo.online", "https://qapihirecp1o.paraminfo.online",
                    "http://localhost:4200", "http://localhost:4400", "http://localhost:53971", "http://localhost:4401", "http://localhost:55577",
                    "https://mgpihireapp1o.paraminfo.online", "https://hirecp.paraminfo.com", "https://hire.paraminfo.com", "https://picrm.paraminfo.online")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
            });
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            services.AddSession();
            services.AddMvc(c => c.Conventions.Add(new ApiExplorerIgnores())).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllers().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddSignalR().AddMessagePackProtocol();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            var AppSettingConfig = Configuration.GetSection("AppSettings");
            var JwtSecret = AppSettingConfig.GetValue<string>("JwtSecret");
            var JwtIssuer = AppSettingConfig.GetValue<string>("JwtIssuer");
            var scrt = System.Text.Encoding.ASCII.GetBytes(JwtSecret);

            services.AddAuthentication(da =>
            {
                da.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                da.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(da =>
            {
                da.RequireHttpsMetadata = false;
                da.SaveToken = true;
                da.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = JwtIssuer,
                    ValidateIssuer = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(scrt),
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateActor = false,
                    ValidateTokenReplay = false,
                    RequireSignedTokens = false
                };
            });

            //Fetching Connection string from APPSETTINGS.JSON  
            var ConnectionString = Configuration.GetConnectionString("dbConnection");

            //Entity Framework  
            services.AddDbContext<DAL.PiHIRE2Context>(options => options.UseSqlServer(ConnectionString));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "piHire", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
         {
             {
                   new OpenApiSecurityScheme
                     {
                         Reference = new OpenApiReference
                         {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                         }
                     },
                     new string[] {}

             }
         });
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ILoggedUserDetails, LoggedUserDetails>();
            services.AddScoped<BAL.Common.Extensions.AppSettings>();

            services.AddScoped<BAL.IRepositories.IBaseRepository, BAL.Repositories.BaseRepository>();
            services.AddScoped<BAL.IRepositories.ICompanyRepository, BAL.Repositories.CompanyRepository>();
            services.AddScoped<BAL.IRepositories.IAutomationRepository, BAL.Repositories.AutomationRepository>();
            services.AddScoped<BAL.IRepositories.IEmployeeRepository, BAL.Repositories.EmployeeRepository>();
            services.AddScoped<BAL.IRepositories.ICommonRepository, BAL.Repositories.CommonRepository>();
            services.AddScoped<BAL.IRepositories.IUserRespository, BAL.Repositories.UserRespository>();
            services.AddScoped<BAL.IRepositories.IWorkflowRepository, BAL.Repositories.WorkflowRepository>();
            services.AddScoped<BAL.IRepositories.ITemplateRepository, BAL.Repositories.TemplateRepository>();
            services.AddScoped<BAL.IRepositories.IOpeningRepository, BAL.Repositories.OpeningRepository>();
            services.AddScoped<BAL.IRepositories.ITechnologyRepository, BAL.Repositories.TechnologyRepository>();
            services.AddScoped<BAL.IRepositories.ICandidateRepository, BAL.Repositories.CandidateRepository>();
            services.AddScoped<BAL.IRepositories.IBGVRepository, BAL.Repositories.BGVRepository>();
            services.AddScoped<BAL.IRepositories.IAccountRepository, BAL.Repositories.AccountRepository>();
            services.AddScoped<BAL.IRepositories.INotificationRepository, BAL.Repositories.NotificationRepository>();
            services.AddScoped<BAL.IRepositories.IChatComunicationRepository, BAL.Repositories.ChatComunicationRepository>();
            services.AddScoped<BAL.IRepositories.IBlogsRepository, BAL.Repositories.BlogsRepository>();
            services.AddScoped<BAL.IRepositories.ITestimonialRepository, BAL.Repositories.TestimonialRepository>();
            services.AddScoped<BAL.IRepositories.IRefRepository, BAL.Repositories.RefRepository>();
            services.AddScoped<BAL.IRepositories.IOffersRepository, BAL.Repositories.OffersRepository>();
            services.AddScoped<BAL.IRepositories.ICustomSchedulerRepository, BAL.Repositories.CustomSchedulerRepository>();
            services.AddScoped<BAL.IRepositories.ICandidateInterviewRepository, BAL.Repositories.CandidateInterviewRepository>();
            services.AddScoped<BAL.IRepositories.IWorkSheduleRepository, BAL.Repositories.WorkSheduleRepository>();
            services.AddScoped<BAL.IRepositories.IReportRepository, BAL.Repositories.ReportRepository>();
            services.AddScoped<BAL.IRepositories.IMailSupportRepository, BAL.Repositories.MailSupportRepository>();

            services.AddScoped<JWTAutorization, JWTAutorization>();

            {
                var tmpPth = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "piHireLogger.txt");
                System.IO.File.WriteAllText(tmpPth, "LogPath:" + System.AppContext.BaseDirectory + "Logs");
            }

            if (System.IO.Directory.Exists(System.AppContext.BaseDirectory + "Logs") == false)
                System.IO.Directory.CreateDirectory(System.AppContext.BaseDirectory + "Logs");

            services.AddLogging(lb =>
            {
                lb.AddConfiguration(Configuration.GetSection("Logging"));
                lb.AddFile(o => o.RootPath = System.AppContext.BaseDirectory);
                //lb.AddFile<CustomFileLoggerProvider>(configure: o => o.RootPath = System.AppContext.BaseDirectory);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ICorsService corsService, ICorsPolicyProvider corsPolicyProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-local-development");
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notification");
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                ServeUnknownFileTypes = true,
                OnPrepareResponse = (ctx) =>
                {
                    var policy = corsPolicyProvider.GetPolicyAsync(ctx.Context, "CorsPolicy")
                        .ConfigureAwait(false)
                        .GetAwaiter().GetResult();

                    var corsResult = corsService.EvaluatePolicy(ctx.Context, policy);

                    corsService.ApplyResult(corsResult, ctx.Context.Response);
                }
            });


            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
                      "Origin, X-Requested-With, Content-Type, Accept");
                }
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "TemplateGallery")),
                RequestPath = "/TemplateGallery"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Candidate")),
                RequestPath = "/Candidate"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Employee")),
                RequestPath = "/Employee"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Testimonals")),
                RequestPath = "/Testimonals"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Blogs")),
                RequestPath = "/Blogs"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates")),
                RequestPath = "/EmailTemplates"
            });

            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("v1/swagger.json", "piHire_v1");
            //});


            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "piHire_v1");
                });
            }

        }
    }
}
