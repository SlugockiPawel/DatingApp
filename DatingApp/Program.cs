using DatingApp.Data;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Middleware;
using DatingApp.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddApplicationServices(); // custom extension method ApplicationServiceExtensions to clean up Program.cs

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

builder.AddIdentityServices(); // custom extension method IdentityServiceExtensions to clean up Program.cs
builder.Services.AddSignalR();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

var app = builder.Build();

// Seed Data
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // enable legacy DateTime behavior

await Seed.ManageDataAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

// app.UseRouting();

// UseCors should be used before UseAuthentication(); and UseAuthorization();
// AND after UseHttpsRedirection(); further details => https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0#middleware-order
app.UseCors(policy =>
{
    policy
        .WithOrigins("https://localhost:4200", "http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

app.Run();


// TODO add rate limiter for registering user (1 per day?)
// TODO add rate limiter to photo upload (2 req per day?) - limit max photo per request to 3