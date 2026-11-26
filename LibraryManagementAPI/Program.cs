using FluentValidation;
using LibraryManagementAPI.Controllers;
using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.DTOs.Authors;
using LibraryManagementAPI.DTOs.Categories;
using LibraryManagementAPI.DTOs.Members;
using LibraryManagementAPI.DTOs.Loans;
using LibraryManagementAPI.Filter;
using LibraryManagementAPI.Mapping;
using LibraryManagementAPI.Middleware;
using LibraryManagementAPI.Services;
using LibraryManagementAPI.Services.Contracts;
using LibraryManagementAPI.Validators.Author;
using LibraryManagementAPI.Validators.Category;
using LibraryManagementAPI.Validators.Member;
using LibraryManagementAPI.Validators.Loan;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Diagnostics;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<AuthorMapper>();
builder.Services.AddSingleton<CategoryMapper>();
builder.Services.AddSingleton<BookMapper>();
builder.Services.AddSingleton<MemberMapper>();
builder.Services.AddSingleton<LoanMapper>();
builder.Services.AddScoped<IValidator<CreateCategoryDto>, CreateCategoryValidator>();
builder.Services.AddScoped<IValidator<UpdateCategoryDto>, UpdateCategoryValidator>();
builder.Services.AddScoped<IValidator<CreateAuthorDto>, CreateAuthorValidator>();
builder.Services.AddScoped<IValidator<CreateMemberDto>, CreateMemberValidator>();
builder.Services.AddScoped<IValidator<UpdateMemberDto>, UpdateMemberValidator>();
builder.Services.AddScoped<IValidator<CreateLoanDto>, CreateLoanValidator>();
builder.Services.AddScoped<IValidator<UpdateLoanDto>, UpdateLoanValidator>();
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
//builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapCategoryEndpoints();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestTimingMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
