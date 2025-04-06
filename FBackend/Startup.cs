using ApiCitaOdon.Data;
using ApiCitaOdon.Services;
using ApiCitaOdon.Services.Interfaces;
using FBackend.Services;
using FBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly string _corsPolicy = "CorsPolicy";

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Configuración de servicios
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Configuración de la base de datos
        //services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseMySql(
        //        _configuration.GetConnectionString("DefaultConnection"),
        //        ServerVersion.AutoDetect(_configuration.GetConnectionString("DefaultConnection"))
        //    ));
        services.AddDbContext<ApplicationDbContext>(options =>
                           options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")
                           ));
        // Servicios personalizados
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthService, AuthService>();

        //Add Service
        services.AddTransient<IProveedorService, ProveedorService>();
        services.AddTransient<IProductoService, ProductoService>();
        services.AddTransient<IDetallePedidoService, DetallePedidoService>();
        services.AddTransient<IPedidoService, PedidoService>();
        services.AddTransient<IPersonalizadoService, PersonalizadoService>();
        services.AddTransient<ICategoriaService, CategoriaService>();
        services.AddTransient<IInventarioService, InventarioService>();

        //mapper
        services.AddAutoMapper(typeof(Startup));

        // Configuración de CORS
        services.AddCors(options =>
        {
            options.AddPolicy(_corsPolicy, builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .WithExposedHeaders("Content-Disposition");
            });
        });

        // Configuración de JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
                };
            });

        // AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Configuración para Swagger
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token."
            });
            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
    }

    // Configuración del middleware
    public void Configure(WebApplication app)
    {
        app.UseSwagger();
        app.UseExceptionHandler("/error");
        app.UseHsts();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseCors(_corsPolicy);
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
