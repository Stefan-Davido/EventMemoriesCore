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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SharedItems;
using SharedItems.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

namespace EventMemories
{
    public class Program
    {
        private const string FrontendCorsPolicy = "FrontendCorsPolicy";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext
            builder.Services.AddDbContext<EventMemoriesDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<TenantProvider>();

            // Add Identity
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<EventMemoriesDbContext>()
            .AddUserManager<ApplicationUserManager>()
            .AddDefaultTokenProviders();

            // Add services to the container.
            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            var jwtKey = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero // removes default 5min expiry tolerance
                };
            });

            builder.Services.AddAuthorization();


            // Register repositories
            builder.Services.AddScoped<ITenantRepository, TenantRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<IInfoRepository, InfoRepository>();
            builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Register services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITenantService, TenantService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IInfoService, InfoService>();
            builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(FrontendCorsPolicy, policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerFileOperationFilter>();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
            }

            app.UseHttpsRedirection();

            // Middlewares
            app.UseMiddleware<SharedItems.Middleware.GlobalExceptionMiddleware>();

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

            app.UseCors(FrontendCorsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }

    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formParams = context.MethodInfo.GetParameters()
                .Where(p => p.GetCustomAttribute<Microsoft.AspNetCore.Mvc.FromFormAttribute>() != null
                         || p.ParameterType.GetProperties().Any(prop => IsFileType(prop.PropertyType)))
                .ToList();

            if (!formParams.Any()) return;

            var properties = new Dictionary<string, IOpenApiSchema>();

            foreach (var param in formParams)
            {
                foreach (var prop in param.ParameterType.GetProperties())
                {
                    properties[prop.Name] = IsFileType(prop.PropertyType)
                        ? new OpenApiSchema
                        {
                            Type = JsonSchemaType.Array,
                            Items = new OpenApiSchema { Type = JsonSchemaType.String, Format = "binary" }
                        }
                        : new OpenApiSchema { Type = MapSimpleType(prop.PropertyType) };
                }
            }

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Object,
                            Properties = properties
                        }
                    }
                }
            };

            operation.Parameters?.Clear();
        }

        private static bool IsFileType(Type type)
        {
            return type == typeof(IFormFile)
                || typeof(IEnumerable<IFormFile>).IsAssignableFrom(type);
        }

        private static JsonSchemaType MapSimpleType(Type type)
        {
            if (type == typeof(int) || type == typeof(int?)) return JsonSchemaType.Integer;
            if (type == typeof(bool) || type == typeof(bool?)) return JsonSchemaType.Boolean;
            // Guid, DateTime, string, etc. all serialize as string in OpenAPI
            return JsonSchemaType.String;
        }
    }
}
