using Microsoft.AspNetCore.Mvc;
using SocialMedia.WebAPI.Configuration;
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

builder.Services.AddAuth0Management(builder.Configuration);
builder.Services.AddCors(builder.Configuration);
builder.Services.AddDomainServices(builder.Configuration, args);
builder.Services.AddSecirity(builder.Configuration);
//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.SuppressModelStateInvalidFilter = true;
//    options.SuppressMapClientErrors = true;
//});

var version = VersionInfo.NewVersionInfo<Program>();

builder.Services.AddSingleton(version);
builder.Services.AddSwagger(version);

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

//app.MigrateDatabase();

app.Run();
