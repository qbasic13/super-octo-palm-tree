using Microsoft.EntityFrameworkCore;
using BooksiteAPI.Data;
using BooksiteAPI.Models.Mail;
using BooksiteAPI.Services;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Certificate;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add database connectivity
builder.Services.AddDbContext<BooksiteContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
);

//Configure authorization and authentication
TokenValidationParameters tokenValidation = new()
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuers = builder.Configuration.GetSection("JWT")
                .GetRequiredSection("ValidIssuers").Get<string[]>(),
    ValidAudience = builder.Configuration["JWT:ValidAudience"],
    IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(tokenValidation);
builder.Services.AddAuthentication(authOpts =>
{
    authOpts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOpts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOpts =>
{
    jwtOpts.TokenValidationParameters = tokenValidation;
    jwtOpts.Events = new JwtBearerEvents();
    jwtOpts.Events.OnTokenValidated = async (context) =>
    {

        IJWTService? jwtService = context.Request.HttpContext
            .RequestServices.GetService<IJWTService>();
        if (jwtService is null)
        {
            context.Fail("Internal service error");
            return;
        }
        var jwtToken = context.SecurityToken as JwtSecurityToken;
        if (jwtToken is null || !await jwtService.ValidateAccessAsync(jwtToken))
            context.Fail("Invalid Token Details");
        return;
    };
});
builder.Services.AddTransient<IJWTService, JWTService>();

//get images folder path
string pathToImages = builder.Configuration.GetSection("ImageStorage")
    .GetValue<string>("DefaultPath", "");
if (pathToImages.Length < 1)
{
    pathToImages = Directory.GetCurrentDirectory();
    pathToImages = Path.GetFullPath(pathToImages +
        builder.Configuration.GetSection("ImageStorage")
        .GetValue<string>("RelativeToWorkDirPath", "?"));
}

if (!Directory.Exists(pathToImages))
{
    throw new Exception("Incorrect path to images directory specified");
}

// Configure mail service
builder.Services.Configure<MailSettings>(
    builder.Configuration.GetSection("MailSettings"));

builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IBookService, BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(pathToImages),
    RequestPath = new PathString("/img")
});

app.MapControllers();

app.Run();
