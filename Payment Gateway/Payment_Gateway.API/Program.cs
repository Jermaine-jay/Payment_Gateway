using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog;
using Payment_Gateway.API.Extensions;
using Payment_Gateway.API.Filter;
using Payment_Gateway.BLL.Implementation.Services;
using Payment_Gateway.BLL.Interfaces.IServices;
using Payment_Gateway.BLL.Paystack.Implementation;
using Payment_Gateway.BLL.Paystack.Interfaces;
using Payment_Gateway.DAL;
using Payment_Gateway.DAL.Context;
using System.Reflection;
using Payment_Gateway.DAL.Seeds;



namespace Payment_Gateway.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(),
            "/nlog.config"));
            // Add services to the container.
            builder.Services.ConfigureCors();
            builder.Services.ConfigureIISIntegration();

            builder.Services.AddDbContext<PaymentGatewayDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));

            builder.Services.ConfigurationBinder(builder.Configuration);


            //builder.Services.AddDatabaseConnection();
            builder.Services.ConfigureIdentity();
            //builder.Services.ConfigureSqlContext(builder.Configuration);

            builder.Services.AddScoped<ValidationFilterAttribute>();

            //builder.Services.AddHttpContextAccessor();
            builder.Services.AddAutoMapper(Assembly.Load("Payment_Gateway.Shared"));
            builder.Services.AddScoped<IMakePaymentService, MakePaymentService>();
            builder.Services.AddScoped<IAdminServices, AdminServices>();
            builder.Services.AddScoped<IAdminProfileServices, AdminProfileServices>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Payment Gateway", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
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
                            Array.Empty<string>()
                    },
                });
            });

            builder.Services.RegisterServices();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            //app.UseCors("AllowAll");
            //app.UseMiddleware<ApiKeyMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers();});
            app.MapControllers();

            //await app.SeedRole();
            //await app.SeededUserAsync();
            //await app.EnsurePopulatedAsync();

            app.Run();
        }
    }
}