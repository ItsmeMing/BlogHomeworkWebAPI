using DataAccess.Blog.EntityFramework;
using DataAccess.Blog.IServices;
using DataAccess.Blog.Services;
using DataAccess.Blog.UnitOfWork;
using DataAccess.Eshop.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddDbContext<BlogDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ConnStr")));
builder.Services.AddDbContext<BlogDbContext>(options =>
	options.UseNpgsql(configuration.GetConnectionString("PsqlConnStr")));
builder.Services.AddTransient<IPostRepository, PostRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IBlogUnitOfWork, BlogUnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
