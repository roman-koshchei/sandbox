using Data;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sandbox.Configuration;
using Sandbox.Hubs;
using Unator;

// Load env variables form .env (in development)
Env.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
string connectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "db", "sandbox.db")}";
builder.Services.AddDbContext<Db>(options => options.UseSqlite(connectionString));

builder.Services
    .AddIdentity<SandboxUser, IdentityRole>()
    .AddEntityFrameworkStores<Db>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<DataProtectorTokenProvider<SandboxUser>>("email");

builder.Services.AddSignalR();

builder.Services.AddJwt();
builder.Services.AddEmail();

var app = builder.Build();

// Use Swagger even in production mode
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();