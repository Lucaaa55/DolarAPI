using Dolarium.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<DolarService>();
builder.Services.AddScoped<BancoService>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();