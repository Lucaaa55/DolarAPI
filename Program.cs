using Dolarium.Data;
using Dolarium.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Dolarium.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 50;
        options.Window = TimeSpan.FromSeconds(Math.Abs(Math.Pow(60, 2) * 24)); // 1 dia
        options.QueueLimit = 2;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IDolarService, DolarService>();
builder.Services.AddScoped<IBancoService, BancoService>();
builder.Services.AddScoped<IIndiceService, IndiceService>();
builder.Services.AddScoped<KeyService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();