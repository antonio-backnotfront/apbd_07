using API;
using Logic;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Connection string (read from appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("UniversityDatabase");

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inject DeviceService
builder.Services.AddScoped<IDeviceRepository>(sp => new DeviceRepository(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();