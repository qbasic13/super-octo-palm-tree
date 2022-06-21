using Microsoft.EntityFrameworkCore;
using BooksiteAPI.Data;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BooksiteContext>(
	options => options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"))
);

//get images folder path
string pathToImages = builder.Configuration.GetSection("ImageStorage")
	.GetValue<string>("DefaultPath","");
if (pathToImages.Length < 1) {
	pathToImages = Directory.GetCurrentDirectory();
	pathToImages = Path.GetFullPath( pathToImages +
		builder.Configuration.GetSection("ImageStorage")
		.GetValue<string>("RelativeToWorkDirPath", "?"));
}

if (!Directory.Exists(pathToImages))
{
	throw new Exception("Incorrect path to images directory specified");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions()
{
	FileProvider = new PhysicalFileProvider(pathToImages),
	RequestPath = new PathString("/img")
});

app.MapControllers();

app.Run();
