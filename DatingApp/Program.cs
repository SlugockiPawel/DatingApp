using System.Text;
using DatingApp.Data;
using DatingApp.Extensions;
using DatingApp.Middleware;
using DatingApp.Services;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddApplicationServices(); // custom extension method ApplicationServiceExtensions to clean up Program.cs

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors();

builder.AddIdentityServices(); // custom extension method IdentityServiceExtensions to clean up Program.cs


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// app.UseRouting();
app.UseCors(policy =>
{
    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200");
});

app.MapControllers();

app.Run();
