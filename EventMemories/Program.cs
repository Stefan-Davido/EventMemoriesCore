using Dal;
using Dal.Repositories;
using DalEntities;
using EventMemoriesServices.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using System.Reflection;

namespace EventMemories
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext
            builder.Services.AddDbContext<EventMemoriesDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Identity
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<EventMemoriesDbContext>()
                .AddDefaultTokenProviders();

            // Add services to the container.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            // Register repositories
            builder.Services.AddScoped<ITenantRepository, TenantRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<IInfoRepository, InfoRepository>();
            builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Register services
            builder.Services.AddScoped<ITenantService, TenantService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IInfoService, InfoService>();
            builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddControllers();


            // Add OpenAPI for development
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
            }

            app.UseHttpsRedirection();
            app.MapOpenApi();

            // Enable Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "EventMemories API v1");
                options.RoutePrefix = "swagger";
                options.DefaultModelsExpandDepth(2);
                options.DefaultModelExpandDepth(2);
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                options.DisplayRequestDuration();
                options.EnableDeepLinking();
                options.EnableFilter();
                options.ShowExtensions();
                options.SupportedSubmitMethods(
                    Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Get,
                    Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Post,
                    Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Put,
                    Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Delete,
                    Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Patch
                );
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
