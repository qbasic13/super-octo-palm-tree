using Microsoft.EntityFrameworkCore;
using BooksiteAPI.Data;
using Microsoft.Extensions.FileProviders;

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
	FileProvider = new PhysicalFileProvider(
		app.Configuration.GetSection("ImageStorage").GetValue<string>("DefaultImagePath"," ")),
	RequestPath = new PathString("/images")
});

app.MapControllers();

app.Run();
