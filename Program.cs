using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Dolarium.Data;
using Dolarium.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<DolarService>();
builder.Services.AddScoped<BancoService>();
builder.Services.AddScoped<KeyService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();