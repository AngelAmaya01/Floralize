var builder = WebApplication.CreateBuilder(args);

// Instancia de Startup
var startup = new Startup(builder.Configuration);

// Configuración de servicios
startup.ConfigureServices(builder.Services);

// Construcción de la aplicación
var app = builder.Build();

// Configuración de middleware
startup.Configure(app);

// Ejecutar la aplicación
app.Run();
