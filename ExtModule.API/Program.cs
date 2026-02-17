using ExtModule.API.Infrastructure;
using Microsoft.OpenApi.Models;
using System.Reflection;
using log4net.Config;
using log4net;
using ExtModule.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;




var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

//add AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AutoMapperProfile>();
});

//add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken);
            if (!string.IsNullOrEmpty(accessToken))
            {
                ctx.Token = accessToken;

            }
            return Task.CompletedTask;
        }
    };
    
});

builder.Services.Configure<CookiePolicyOptions>(options => { options.CheckConsentNeeded = context => true; options.MinimumSameSitePolicy = SameSiteMode.Strict; });

builder.Services.AddAuthorization();


// Add services to the container.

builder.Services.AddControllers();
//configure log4net
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

//Injecting services from infrastructure.
builder.Services.RegisterServices();




builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    //c.IncludeXmlComments(string.Format(@"{0}\ExtModule.API.xml", System.AppDomain.CurrentDomain.BaseDirectory));
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ExtModule.API",
    });
});
builder.Services.AddControllers().AddNewtonsoftJson();


var app = builder.Build();


// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();
// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnionArchitecture");
});

//Exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();

app.MapControllers();

app.Run();
