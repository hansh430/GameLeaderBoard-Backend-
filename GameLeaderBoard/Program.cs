using GameLeaderBoard.Data;
using GameLeaderBoard.Extensions;
using GameLeaderBoard.Models;
using GameLeaderBoard.Repositories;
using GameLeaderBoard.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddAuthentication(
JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer =
                builder.Configuration["Jwt:Issuer"],

            ValidAudience =
                builder.Configuration["Jwt:Audience"],

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        builder.Configuration["Jwt:Key"]!))
        };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = GameDbContext =>
        {
            GameDbContext.HandleResponse();
            GameDbContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            GameDbContext.Response.ContentType = "application/json";

            return GameDbContext.Response.WriteAsJsonAsync(new
            {
                success = false,
                statusCode = 401,
                message = "Authentication required. Please login."
            });
        },
        OnForbidden = GameDbContext =>
        {
            GameDbContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            GameDbContext.Response.ContentType = "application/json";
            return GameDbContext.Response.WriteAsJsonAsync(new
            {
                succes = false,
                statusCode = 403,
                message = "You do not have permission to perfom this action"
            });
        },
        OnTokenValidated = async context =>
        {
            var db = context.HttpContext.RequestServices
                .GetRequiredService<GameDbContext>();

            var jti = context.Principal?
                .FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (!string.IsNullOrEmpty(jti))
            {
                bool revoked = await db.RevokedTokens
                    .AnyAsync(x => x.JwtId == jti);

                if (revoked)
                {
                    context.Fail("Token has been revoked");
                }
            }
        }
    };
});


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddDbContext<GameDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Fix CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();


// Seed Admin User
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();

    context.Database.Migrate();

    if (!context.Users.Any(u => u.Role == "Admin"))
    {
        context.Users.Add(new User
        {
            Username = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "Admin"
        });

        context.SaveChanges();
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseGlobalException();
//app.UseHttpsRedirection();

app.UseCors("ReactPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
