using MajorWebAPI.Extensions;
using MajorWebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using WebAPI.Core.Service;

var builder = WebApplication.CreateBuilder(args);
var ConfigurationSerilog = builder.Configuration;

Serilog();
// Add services to the container.

ConfigureService.Configrue(builder.Services,builder.Configuration);

//builder.Services.AddTransient<IAuthService,AuthService>();
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["JWT:Issuer"],
//        ValidAudience = builder.Configuration["JWT:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
//        ClockSkew = TimeSpan.Zero
//    };
//});

//builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void Serilog()
{
    //Serilog Configuration
    var logPath = ConfigurationSerilog.GetSection("Serilog:LogFilePath").Value;
    var fileCountLimit = ConfigurationSerilog.GetSection("Serilog:FileCountLimit").Value;
    var fileSizeConfig = ConfigurationSerilog.GetSection("Serilog:FileSizeInBytes").Value;
    int? limit = !string.IsNullOrEmpty(fileCountLimit) ? (int?)Convert.ToInt32(fileCountLimit) : null;
    int fileSizeInBytes = !string.IsNullOrEmpty(fileSizeConfig) ? Convert.ToInt32(fileSizeConfig) : 100000;

    var logger = new LoggerConfiguration()
         .ReadFrom.Configuration(ConfigurationSerilog)
         .WriteTo.Debug()
         .WriteTo.Async(async => async.Map("0", "Other-Other", (folder_filename, writeTo) =>
                writeTo.File($"{logPath}{folder_filename.Split('-')[0]}\\{folder_filename.Split('-')[1]}_.log",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: fileSizeInBytes,
                retainedFileCountLimit: limit), sinkMapCountLimit: 0))
         .CreateLogger();

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(logger);
}