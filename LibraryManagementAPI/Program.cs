using FluentValidation;
using LibraryManagementAPI.Controllers;
using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.DTOs.Authors;
using LibraryManagementAPI.DTOs.Categories;
using LibraryManagementAPI.Mapping;
using LibraryManagementAPI.Validators.Author;
using LibraryManagementAPI.Validators.Category;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<AuthorMapper>();
builder.Services.AddSingleton<CategoryMapper>();
builder.Services.AddScoped<IValidator<CategoryCreateDto>, CreateCategoryValidator>();
builder.Services.AddScoped<IValidator<CategoryUpdateDto>, UpdateCategoryValidator>();
builder.Services.AddScoped<IValidator<AuthorCreateDto>, CreateAuthorValidator>();
builder.Services.AddValidation();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapCategoryEndpoints();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
