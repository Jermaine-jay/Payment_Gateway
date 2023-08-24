using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Payment_Gateway.API.Attribute;
using Payment_Gateway.BLL.Implementation;
using Payment_Gateway.BLL.Implementation.Services;
using Payment_Gateway.BLL.Infrastructure;
using Payment_Gateway.BLL.Infrastructure.ApiKeyMiddleware;
using Payment_Gateway.BLL.Infrastructure.CacheServices;
using Payment_Gateway.BLL.Infrastructure.EmailSender;
using Payment_Gateway.BLL.Infrastructure.GenerateEmailPage;
using Payment_Gateway.BLL.Infrastructure.jwt;
using Payment_Gateway.BLL.Infrastructure.Otp;
using Payment_Gateway.BLL.Infrastructure.Paystack;
using Payment_Gateway.BLL.Interfaces;
using Payment_Gateway.BLL.Interfaces.IServices;
using Payment_Gateway.BLL.LoggerService.Implementation;
using Payment_Gateway.BLL.LoggerService.Interface;
using Payment_Gateway.BLL.Paystack.Implementation;
using Payment_Gateway.BLL.Paystack.Interfaces;
using Payment_Gateway.DAL.Context;
using Payment_Gateway.DAL.Implementation;
using Payment_Gateway.DAL.Interfaces;
using Payment_Gateway.Models.Entities;
using Payment_Gateway.Shared.DataTransferObjects;
using StackExchange.Redis;
using System.Security.Authentication;
using System.Text;



namespace Payment_Gateway.API.Extensions
{

    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddScoped<IJWTAuthenticator, JwtAuthenticator>();
            services.AddScoped<IAuthorizationHandler, AuthHandler>();
            services.AddScoped<IUnitOfWork, UnitOfWork<PaymentGatewayDbContext>>();
            services.AddScoped<IServiceFactory, ServiceFactory>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IEmailServices, EmailServices>();
            services.AddScoped<IGenerateEmailPage, GenerateEmailPage>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IPaystackPostRequest, PaystackPostRequest>();
            services.AddScoped<IPayoutService, PayoutService>();
            services.AddScoped<IPayoutServiceExtension, PayoutServiceExtension>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IAccountLockoutService, AccountLockoutService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleClaimService, RoleClaimService>();
        }


        public static void ConfigureCors(this IServiceCollection services) =>
             services.AddCors(options =>
             {
                 options.AddPolicy("CorsPolicy", builder =>
                 builder.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader());
             });


        public static void ConfigureIISIntegration(this IServiceCollection services) =>
             services.Configure<IISOptions>(options =>
             {
             });


        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
            {
                o.SignIn.RequireConfirmedAccount = false;
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<PaymentGatewayDbContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
            });
        }


        public static void ConfigureJWT(this IServiceCollection services, JwtConfig jwtConfig)
        {
            var jwtSettings = jwtConfig;
            var secretKey = jwtSettings.Secret;

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })


            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Authorization", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new AuthRequirement());
                    policy.Build();
                });
            });
        }


        public static void ConfigurationBinder(this IServiceCollection services, IConfiguration configuration)
        {
            Settings setting = configuration.Get<Settings>()!;
            services.AddSingleton(setting);

            JwtConfig jwtConfig = setting.JwtConfig;
            services.AddSingleton(jwtConfig);

            RedisConfig redisConfig = setting.redisConfig;
            services.AddSingleton(redisConfig);

            ZeroBounceConfig zeroBounceConfig = setting.ZeroBounceConfig;
            services.AddSingleton(zeroBounceConfig);

            EmailSenderOptions emailSenderOptions = setting.EmailSenderOptions;
            services.AddSingleton(emailSenderOptions);

            PaystackConfig paystack = setting.PaystackConfig;
            services.AddSingleton(paystack);

            services.ConfigureJWT(jwtConfig);
            services.AddRedisCache(redisConfig);

            services.ConfigureMiddleware();

        }


        public static void AddRedisCache(this IServiceCollection services, RedisConfig redisConfig)
        {

            ConfigurationOptions configurationOptions = new ConfigurationOptions();
            configurationOptions.SslProtocols = SslProtocols.Tls12;
            configurationOptions.SyncTimeout = 30000;
            configurationOptions.Ssl = true;
            configurationOptions.Password = redisConfig.Password;
            configurationOptions.AbortOnConnectFail = false;
            configurationOptions.EndPoints.Add(redisConfig.Host, redisConfig.Port);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configurationOptions.ToString();
                options.InstanceName = redisConfig.InstanceId;
            });

            services.AddSingleton<IConnectionMultiplexer>((x) =>
            {
                var connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    Password = configurationOptions.Password,
                    EndPoints = { configurationOptions.EndPoints[0] },
                    AbortOnConnectFail = false,
                    AllowAdmin = false,
                    ClientName = redisConfig.InstanceId
                });
                return connectionMultiplexer;
            });
            services.AddTransient<ICacheService, CacheService>();
        }



        public static void ConfigureMiddleware(this IServiceCollection services)
        {
            services.AddAuthentication("ApiKeyAuthorization")
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKeyAuthorization", options => { });
        }
    }
}
