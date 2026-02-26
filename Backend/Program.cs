using Backend.Persistence.Context;
using Backend.Persistence.Repositories;
using Backend.Persistence.Repositories.Interfaces;
using Backend.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios básicos
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Configuración de Serilog

builder.Host.UseSerilog((context, services, configuration) => 
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)   // <-- Esto inyecta ICorrelationContextAccessor
        .Enrich.FromLogContext()
        .Enrich.WithCorrelationId()    // <-- Necesita el servicio registrado
);

var correlationOptions = builder.Configuration
    .GetSection("CorrelationIdOptions")
    .Get<CorrelationIdOptions>();

builder.Services.AddDefaultCorrelationId(options => { 
    options.RequestHeader = correlationOptions?.RequestHeader ?? "X-Correlation-Id"; 
    options.ResponseHeader = correlationOptions?.ResponseHeader ?? "X-Correlation-Id"; 
    options.UpdateTraceIdentifier = correlationOptions?.UpdateTraceIdentifier ?? true; 
    options.IncludeInResponse = correlationOptions?.IncludeInResponse ?? true;
    options.AddToLoggingScope = true;
});

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; 
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile); 
    c.IncludeXmlComments(xmlPath);
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Factura API", Version = "v1" });
    // 1. Definir el esquema de seguridad
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = "JWT Authorization encabezado usando el esquema Bearer. Ejemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 2. Aplicar la seguridad de forma global en Swagger
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {{
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configuración de CORS 
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular", 
            policy => policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
});
    

var dbPath = Path.Combine(AppContext.BaseDirectory, "Database");
if (!Directory.Exists(dbPath)){
    Directory.CreateDirectory(dbPath);
}

// Configuración de la conexión SQLite 
builder.Services.AddScoped<ConexionSql>();
/* Nota para el próximo desarrollador:
    AddScoped: Significa que por cada solicitud HTTP que llegue a la aplicación, se creará una nueva instancia de la clase ConexionSql.
*/

// Repositorios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<IFormasPagoRepository, FormasPagoRepository>();
/* Nota para el próximo desarrollador:
    AddScoped: Significa que por cada solicitud HTTP que llegue a la aplicación, se creará una nueva instancia del ClienteRepository. 
    Esta instancia será utilizada durante toda la vida de esa solicitud y luego se descartará.
    ¿Xq? Esto es útil cuando el repositorio maneja datos específicos de una solicitud, como transacciones o contexto de usuario.
*/

//Autehnticacion
// JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Servicios para autenticacion
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

var app = builder.Build();
app.UseCorrelationId();

var dbFile = Path.Combine(AppContext.BaseDirectory, "Database", "facturacion.db");
Console.WriteLine("DB Path: " + dbFile);
Console.WriteLine("DB Exists: " + File.Exists(dbFile));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()|| app.Environment.IsProduction()){
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();
app.Run();
