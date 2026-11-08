using System;
using System.Text;
using Gym_Core.Interfaces;
using Gym_Core.Middlewares;
using Gym_Core.Models;
using Gym_Core.Services;
using Gym_Core.Settings;
using Gym_Infrastructure.Data;
using Gym_Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("DbAddress");

builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseSqlServer(conn)
);

builder.Services.AddIdentity<AppUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<GymDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JWTsettings>(builder.Configuration.GetSection("JwtSettings"));  // for DI

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JWTsettings>();  // for writing settings of jwt in program.cs 
var secretKey = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,     
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,  
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});


builder.Services.AddScoped(typeof(IGeniricRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>(); 
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IMembershipPlanService, MembershipPlanService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();



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

app.MapControllers();

app.Run();
