using ExpenseTracker.Database.Context;
using ExpenseTracker.Database.Models;
using ExpenseTracker.Features.Expenses.Commands;
using ExpenseTracker.Features.Expenses.Queries;
using ExpenseTracker.Features.Categories.Commands;
using ExpenseTracker.Features.Categories.Queries;
using ExpenseTracker.Features.Dashboard.Queries;
using ExpenseTracker.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// CQRS command handlers
builder.Services.AddScoped<CreateExpenseCommandHandler>();
builder.Services.AddScoped<UpdateExpenseCommandHandler>();
builder.Services.AddScoped<DeleteExpenseCommandHandler>();
builder.Services.AddScoped<CreateCategoryCommandHandler>();
builder.Services.AddScoped<UpdateCategoryCommandHandler>();
builder.Services.AddScoped<DeleteCategoryCommandHandler>();

// CQRS query handlers
builder.Services.AddScoped<GetExpenseListQueryHandler>();
builder.Services.AddScoped<GetExpenseByIdQueryHandler>();
builder.Services.AddScoped<GetCategoryListQueryHandler>();
builder.Services.AddScoped<GetCategoryByIdQueryHandler>();
builder.Services.AddScoped<GetDashboardQueryHandler>();

// Existing validators are used by command handlers.
builder.Services.AddScoped<CategoryValidator>();
builder.Services.AddScoped<ExpenseValidator>();

// For Password Hashing and Authentication
builder.Services.AddScoped<IPasswordHasher<User>,PasswordHasher<User>>();

builder.Services.AddDbContext<ExpenseTrackerDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// For Login User
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";

        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy =
            CookieSecurePolicy.Always;

        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
