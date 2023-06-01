using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NomRentals.Api.Data;
using NomRentals.Api.Entities;
using NomRentals.Api.Repository;
using System.Text;
using NLog;
using NLog.Web;
using Microsoft.Extensions.Logging;
using NomRentals.Api.Services.EmailService;


var logger =NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try 
{

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.RequireHttpsMetadata = false;
    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };

});

  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();


  builder.Services.AddDbContext<CustomerApiDbContext>(options => options.UseSqlite(builder.Configuration["ConnectionStrings:DbConnections"]));
  builder.Services.AddIdentity<UserProfile, IdentityRole>().AddEntityFrameworkStores<CustomerApiDbContext>().AddDefaultTokenProviders();

    builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromHours(10));

  builder.Services.AddScoped<IAccountRepository, AccountRepository>();
    builder.Services.AddScoped<IEmailService, EmailService>(); 
 


  var app = builder.Build();

  // Configure the HTTP request pipeline.
  if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

  app.UseHttpsRedirection();

  app.UseAuthorization();
  app.UseRouting();
  app.UseAuthentication();

  app.MapControllers();

  app.Run();
}
catch(Exception ex) 
{
    logger.Error(ex);
    throw(ex);
}
finally 
{
    //Ensure to flush and stop internal timers/threads before application exist(Avoid segmentation fault on Linq)
    NLog.LogManager.Shutdown();
}
