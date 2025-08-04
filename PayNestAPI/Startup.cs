using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PayNestAPI.Configurations;
using PayNestAPI.Context;
using PayNestAPI.Middlewares;
using PayNestAPI.Models.Security;
using PayNestAPI.Repositories.Implementations;
using PayNestAPI.Repositories.Interfaces;
using PayNestAPI.Services.Implementations;
using PayNestAPI.Services.Interfaces;
using Stripe;
using TourManWebAPI.AutoMapper;

namespace PayNestAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDBContext>(options =>
            options.UseSqlServer(_configuration.GetConnectionString("Default")));

            /*services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<ICardService, Services.Implementations.CardService>();
            services.AddScoped<IAuthService,AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddTransient<ITokenService, Services.Implementations.TokenService>();
            */

            services.Scan(scan 
                => scan.FromAssemblyOf<Startup>()
                       .AddClasses()
                       .AsMatchingInterface()
                       .WithScopedLifetime());

            services.Configure<TokenConfigurations>(_configuration.GetSection("JWT"));
            services.Configure<StripeConfigurations>(_configuration.GetSection("Stripe"));
            //StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"]; <-remove slashes

            services.AddMemoryCache();
            services.AddCustomMapper();
            services.AddControllers();
            services.AddDistributedMemoryCache();
            services.AddEndpointsApiExplorer();
            services.AddHttpContextAccessor();

            var emailConfig=_configuration.GetSection("Email");
            services.AddFluentEmail(emailConfig["SenderEmail"], emailConfig["Sender"])
                .AddSendGridSender(emailConfig["SendGridApiKey"]);

            services.AddIdentityCore<AppUser>(opt =>
                    opt.SignIn.RequireConfirmedAccount = true)
                        .AddRoles<Roles>()
                        .AddEntityFrameworkStores<AppDBContext>()
                        .AddDefaultTokenProviders();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "App.API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter the token after writin 'Bearer'"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                        });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidAudience = _configuration["JWT:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public void Configure(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
