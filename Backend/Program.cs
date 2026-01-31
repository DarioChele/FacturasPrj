using Microsoft.Data.Sqlite;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar controladores 
builder.Services.AddControllers(); 

// Configuración de CORS 
builder.Services.AddCors(options => { 
    options.AddPolicy("AllowAll", 
            policy => policy.AllowAnyOrigin() 
                            .AllowAnyMethod() 
                            .AllowAnyHeader()); 
                        });
// Configuración de la conexión SQLite 
var connectionString = "Data Source=Database/facturacion.db"; 
builder.Services.AddSingleton(new SqliteConnection(connectionString)); 
/* Nota para el próximo desarrollador:
    AddSingleton: Significa que la aplicación creará una sola instancia de la conexión y la compartirá durante todo el tiempo que la aplicación esté encendida.
    ¿Xq? SQLite es una base de datos de archivo único y al tener una sola instancia compartida lo hacemos eficiente para aplicaciones pequeñas o medianas. =)
*/

// Registro del Repositorio
builder.Services.AddScoped<Backend.Persistence.Repositories.ClienteRepository>();
/* Nota para el próximo desarrollador:
    AddScoped: Significa que por cada solicitud HTTP que llegue a la aplicación, se creará una nueva instancia del ClienteRepository. 
    Esta instancia será utilizada durante toda la vida de esa solicitud y luego se descartará.
    ¿Xq? Esto es útil cuando el repositorio maneja datos específicos de una solicitud, como transacciones o contexto de usuario.
*/


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();    
}

app.UseCors("AllowAll"); 
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
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

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
