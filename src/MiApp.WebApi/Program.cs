using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MiApp.Application.Common.Behaviors;
using MiApp.Application.UseCases.Products.Commands;
using MiApp.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ── SERVICIOS ─────────────────────────────────────────────────────────────────

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "MiApp API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Ejemplo: 'Bearer {token}'",
        Name        = "Authorization",
        In          = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type        = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme      = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT
var config = builder.Configuration;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = config["Jwt:Issuer"],
            ValidateAudience         = true,
            ValidAudience            = config["Jwt:Audience"],
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin")));

// MediatR + Pipeline Behaviors
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CreateProductCommand>();
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
});

// FluentValidation — registra todos los validators de Application de una sola vez
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommand>();

// Infrastructure (DbContext, repositories, JWT service, BCrypt, GlobalExceptionHandler)
builder.Services.AddInfrastructure(builder.Configuration);

// AutoMapper
builder.Services.AddAutoMapper(typeof(MiApp.Application.Mappings.MappingProfile));

// CORS
builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend", p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// ── PIPELINE ──────────────────────────────────────────────────────────────────

var app = builder.Build();

app.UseExceptionHandler();       // PRIMERO — captura toda excepción de la cadena

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseAuthentication();         // ANTES que UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
