using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SiJwtAuth;
using SiJwtAuth.Application.Services;
using SiJwtAuth.Application.Services.impl;
using SiJwtAuth.Application.Utils;
using SiJwtAuth.Application.Utils.Impl;
using SiJwtAuth.Data;
using SiJwtAuth.Data.Repositories;
using SiJwtAuth.Data.Repositories.impl;

var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;
var connectionString = conf[Const.ConfKey.AuthDbConnectionStringKey] ??
                       conf.GetConnectionString(Const.ConfKey.AuthDbConnectionStringKey);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddDbContextFactory<AuthContext>(o => o.UseNpgsql(connectionString))
    .AddSingleton<ITimespanParser, TimespanParser>()
    .AddSingleton<SecurityTokenHandler, JwtSecurityTokenHandler>()
    .AddSingleton<ITokensFactory, TokensFactory>()
    .AddSingleton<ITokenService, TokenService>()
    .AddSingleton<IUserAuthService, UserAuthService>()
    .AddSingleton<IUsersRepository, UsersRepository>()
    .AddSingleton<ITokensRepository, TokensRepository>()
    .AddHttpContextAccessor()
    .AddLogging();

if (builder.Environment.IsDevelopment())
{
    builder.Services
        .AddAuthorization()
        .AddAuthentication()
        .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = conf[Const.ConfKey.JwtIssuer],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf[Const.ConfKey.JwtKey]!))
                };
            }
        );
}


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.Services.GetService<ILogger<Program>>()!.LogInformation("ConnectionString {connectionString}", connectionString);
    app.UseSwagger();
    app.UseSwaggerUI();
    app.Map("/api/isValidToken", [Authorize](HttpContext ctx) => "Token is valid!");
}
else
{
    app.UseExceptionHandler(appBuilder => appBuilder.Run(
        async ctx =>
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await ctx.Response.WriteAsync("Internal server error");
        }));
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers().WithOpenApi();
app.Run();
