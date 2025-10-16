using SocialMedia.WebAPI.Configuration;
using SocialMedia.WebAPI.Endpoints;
using SocialMedia.WebAPI.Formatters;
using SocialMedia.WebAPI.JsonConverters;
using SocialMedia.WebAPI.ModelBinders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(configure: options =>
{
    options.InputFormatters.Add(new PlainTextInputFormatter());
    options.ModelBinderProviders.Insert(0, new IdModelBinderProvider());
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new IdJsonConverter());
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddJsonOptions();
builder.Services.AddAuth0Management(builder.Configuration);
builder.Services.AddCors(builder.Configuration);
builder.Services.AddDomainServices(builder.Configuration, args);
builder.Services.AddSecirity(builder.Configuration);

var version = VersionInfo.NewVersionInfo<Program>();

builder.Services.AddSingleton(version);
builder.Services.AddSwagger(version);
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddHealthChecks()
//    .AddCheck<Domain.HealthChecks.DatabaseHealthCheck>("Database")
//    .AddCheck<Domain.HealthChecks.Auth0HealthCheck>("Auth0 Management API");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicies.AllowedOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Minimal API endpoints
app.MapCreatePost();

//app.MigrateDatabase();

app.Run();
