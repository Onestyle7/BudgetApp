using Backend;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Backend.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAuthentication(options =>{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(Options =>{
    Options.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c=>{
    // Dodanie definicji bezpieczeństwa dla JWT w Swaggerze
c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    Description = "JWT Authorization header using the Bearer scheme",
    Name = "Authorization", // Nazwa nagłówka, który będzie zawierał token JWT
    In = ParameterLocation.Header, // Token będzie przekazywany w nagłówku HTTP
    Type = SecuritySchemeType.ApiKey, // Typ schematu bezpieczeństwa
    Scheme = "Bearer" // Nazwa schematu
});

// Dodanie wymagań bezpieczeństwa dla Swaggera, aby każdy endpoint wymagał tokenu JWT
c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme, // Odniesienie do schematu bezpieczeństwa
                Id = "Bearer" // Nazwa schematu zdefiniowana powyżej
            }
        },
        new string[] {} // Puste stringi oznaczają brak dodatkowych wymagań
    }
});
});
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000") // Adres Twojego frontendu
        .AllowAnyMethod()
        .AllowAnyHeader());
});
builder.Services.AddScoped<IUserService, UserService>();
// Rejestracja serwisu dla obsługi transakcji w kontenerze DI
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ISavingGoalsService, SavingGoalsService>();
// Rejestracja AutoMappera, aby mógł automatycznie mapować DTO na modele i odwrotnie
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapControllers();
app.Run();

