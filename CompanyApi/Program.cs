using CompanyApi.Data;
using CompanyApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Регистрация контроллеров и сервисов Swagger для документирования API.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройка контекста базы данных с использованием PostgreSQL.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .LogTo(Console.WriteLine, LogLevel.Information));  // Логирование SQL запросов на консоль.

// Настройка Identity с пользовательскими настройками паролей.
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Настройка JWT аутентификации.
var jwtIssuer = builder.Configuration["AuthSettings:Issuer"];
var jwtKey = builder.Configuration["AuthSettings:Key"];
var jwtAudience = builder.Configuration["AuthSettings:Audience"];

builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(builder.Configuration.GetSection("AuthSettings:Token").Value)),
        ValidateIssuer = false,
        ValidIssuer = jwtIssuer,
        ValidateAudience = false,
        ValidAudience = jwtAudience,
        RequireExpirationTime = false,
    };
});

// Настройка политики CORS для доступа к API из определенных доменов.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
        policy.WithOrigins("http://example.com")  // Замените на актуальный URL, который нужно разрешить.
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Конфигурация HTTP-пайплайна.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowSpecificOrigin");

app.MapControllers();

app.Run();
