using CommandLine;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.Persistence.SqlServer;
using SocialMedia.WebAPI.Configuration;
using SocialMedia.WebAPI.Formatters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(configure: options =>
{
    options.InputFormatters.Add(new PlainTextInputFormatter());
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["authentication:authority"];
    options.Audience = builder.Configuration["authentication:audience"];
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy =>
    {
        policy.RequireClaim("permissions", "admin");
    });
});

var allowedOrigins = "allowedOrigins";
builder.Services.AddCors(options =>
{
    var origins = builder.Configuration.GetSection("cors:allowedOrigins").Get<string[]>();

    if (origins != null)
    {
        options.AddPolicy(name: allowedOrigins,
        policy =>
        {
            policy.WithOrigins(origins)
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
        });
    }
});

var domainAssemblies = new[] { typeof(GetHelpText).Assembly };
var requestTypes = domainAssemblies.SelectMany(assembly => assembly.ExportedTypes
    .Where(t => typeof(IBaseRequest).IsAssignableFrom(t.GetTypeInfo())))
    .ToArray();

var cliRequestTypes = requestTypes
    .Where(t => Attribute.GetCustomAttribute(t, typeof(VerbAttribute)) != null)
    .ToArray();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(domainAssemblies);
});
builder.Services.AddTransient<ICliRequestBuilder>(services =>
    new CliRequestBuilder(requestTypes));
builder.Services.AddSingleton(new HelpTextConfiguration(cliRequestTypes));
builder.Services.AddDbContext<SocialMediaDbContext>(options =>
    options.UseSqlServer(builder.Configuration["connectionStrings:database"]));

builder.Services.AddAuth0ManagementServices(builder.Configuration);

var app = builder.Build();

app.UseCors(allowedOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
