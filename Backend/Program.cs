using Backend.Persistence.Context;
using Backend.Persistence.Repositories;
using Backend.Persistence.Logging;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// 1. Servicios básicos
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de CORS 
builder.Services.AddCors(options => { 
    options.AddPolicy("AllowAll", 
            policy => policy.AllowAnyOrigin() 
                            .AllowAnyMethod() 
                            .AllowAnyHeader()); 
                        });

// Configuración de la conexión SQLite 
builder.Services.AddScoped<ConexionSql>();
/* Nota para el próximo desarrollador:
    AddScoped: Significa que por cada solicitud HTTP que llegue a la aplicación, se creará una nueva instancia de la clase ConexionSql.
*/

//Logg
builder.Services.AddSingleton<IFileLogger, FileLogger>();

// Repositorios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
/* Nota para el próximo desarrollador:
    AddScoped: Significa que por cada solicitud HTTP que llegue a la aplicación, se creará una nueva instancia del ClienteRepository. 
    Esta instancia será utilizada durante toda la vida de esa solicitud y luego se descartará.
    ¿Xq? Esto es útil cuando el repositorio maneja datos específicos de una solicitud, como transacciones o contexto de usuario.
*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();    
}

app.UseCors("AllowAll"); 
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
var summaries = new[]{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary){
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
