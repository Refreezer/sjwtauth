using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SiJwtAuth.Controllers;
using SiJwtAuth.Dao;
using SiJwtAuth.Repositories;
using SiJwtAuth.Repositories.impl;
using SiJwtAuth.Services;
using SiJwtAuth.Services.impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddAuthorization()
    .AddDbContext<AuthContext>(o => o.UseInMemoryDatabase("inmdb"))
    .AddSingleton<SecurityTokenHandler, JwtSecurityTokenHandler>()
    .AddSingleton<ITokenService, TokenService>()
    .AddScoped<IUserAuthService, UserAuthService>()
    .AddScoped<IUsersRepository, UsersRepository>()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
        {
            var conf = builder.Configuration;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = conf["Jwt:Issuer"],
                ValidAudience = conf["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["Jwt:Key"]!))
            };
        }
    );



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers().WithOpenApi();
app.Run();
