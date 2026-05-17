# 🔐 JWT AUTHENTICATION EN .NET 8 CON CLEAN ARCHITECTURE

**Versión:** 1.0  
**Fecha:** Mayo 2026  
**Estándar:** RFC 7519 (JWT)  
**Patrón:** Clean Architecture + SOLID Principles

---

## 📋 TABLA DE CONTENIDOS

1. [Conceptos Fundamentales](#conceptos-fundamentales)
2. [Arquitectura por Capas](#arquitectura-por-capas)
3. [Implementación Completa](#implementación-completa)
4. [Configuración del Program.cs](#configuración-del-programcs)
5. [Endpoints Protegidos](#endpoints-protegidos)
6. [Reglas de Oro](#reglas-de-oro)
7. [Troubleshooting](#troubleshooting)

---

## 🎯 CONCEPTOS FUNDAMENTALES

### JWT (JSON Web Token)

**Estructura:** `Header.Payload.Signature`

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.
SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

### Autenticación vs Autorización

| Concepto | Código HTTP | Descripción |
|----------|-------------|-------------|
| **Autenticación** ✓ | `401 Unauthorized` | ¿Eres realmente quién dices ser? Valida credenciales |
| **Autorización** ✗ | `403 Forbidden` | ¿Tienes permiso para acceder? Valida roles/permisos |

```
Login falla → 401 (No autenticado)
Token expirado → 401 (No autenticado)
Token válido pero sin permiso → 403 (No autorizado)
```

---

## 🏗️ ARQUITECTURA POR CAPAS

```
┌─────────────────────────────────────────┐
│        🌐 WebAPI (Presentation)         │
│  AuthController, [Authorize] Attributes │
├─────────────────────────────────────────┤
│      📦 Application (Business Logic)    │
│  LoginUseCase, ITokenService Interface  │
├─────────────────────────────────────────┤
│      🔧 Infrastructure (Technical)      │
│  JwtTokenService, JwtHandler, DbContext │
├─────────────────────────────────────────┤
│      🎯 Domain (Core Business)          │
│  User Entity, Role Enum, IUserRepository│
└─────────────────────────────────────────┘
```

---

# 🔧 IMPLEMENTACIÓN COMPLETA

## 1️⃣ CAPA DE DOMINIO (Domain)

### User Entity
```csharp
// src/MiApp.Domain/Entities/User.cs
using System;

namespace MiApp.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        
        // ⚠️ NUNCA almacenar contraseña en texto plano
        // Siempre almacenar PasswordHash (resultado de BCrypt)
        public string PasswordHash { get; set; } = null!;
        
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
    }
}
```

### Role Enum
```csharp
// src/MiApp.Domain/Enums/UserRole.cs
namespace MiApp.Domain.Enums
{
    public enum UserRole
    {
        User = 0,          // Usuario regular
        Manager = 1,       // Gestor de contenido
        Admin = 2          // Administrador del sistema
    }
}
```

### User Repository Interface
```csharp
// src/MiApp.Domain/Interfaces/IUserRepository.cs
using System;
using System.Threading.Tasks;
using MiApp.Domain.Entities;

namespace MiApp.Domain.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Obtiene un usuario por su email
        /// </summary>
        /// <remarks>
        /// Este es el punto crítico para autenticación.
        /// El email debe ser único en la base de datos.
        /// </remarks>
        Task<User?> GetByEmailAsync(string email);
        
        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        Task<User?> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        Task<User> CreateAsync(User user);
        
        /// <summary>
        /// Actualiza último login
        /// </summary>
        Task UpdateLastLoginAsync(Guid userId);
    }
}
```

---

## 2️⃣ CAPA DE APLICACIÓN (Application)

### Token Service Interface
```csharp
// src/MiApp.Application/Interfaces/ITokenService.cs
using System;
using System.Threading.Tasks;
using MiApp.Domain.Entities;

namespace MiApp.Application.Interfaces
{
    /// <summary>
    /// Define el contrato para generar y validar tokens JWT
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Genera un token JWT para un usuario autenticado
        /// </summary>
        /// <param name="user">Usuario autenticado</param>
        /// <returns>Token JWT en formato string</returns>
        Task<string> GenerateTokenAsync(User user);
        
        /// <summary>
        /// Valida la integridad del token
        /// </summary>
        /// <param name="token">Token a validar</param>
        /// <returns>true si es válido, false si no</returns>
        Task<bool> ValidateTokenAsync(string token);
        
        /// <summary>
        /// Extrae el ID del usuario desde el token
        /// </summary>
        Task<Guid?> GetUserIdFromTokenAsync(string token);
    }
}
```

### DTOs para Autenticación
```csharp
// src/MiApp.Application/DTOs/LoginDto.cs
namespace MiApp.Application.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
```

```csharp
// src/MiApp.Application/DTOs/AuthResponseDto.cs
namespace MiApp.Application.DTOs
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? Token { get; set; }
        public UserAuthDto? User { get; set; }
    }
}
```

```csharp
// src/MiApp.Application/DTOs/UserAuthDto.cs
using System;

namespace MiApp.Application.DTOs
{
    public class UserAuthDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
```

### Login UseCase (Service)
```csharp
// src/MiApp.Application/Services/AuthService.cs
using System;
using System.Threading.Tasks;
using AutoMapper;
using MiApp.Application.DTOs;
using MiApp.Application.Interfaces;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.Services
{
    /// <summary>
    /// Servicio de autenticación que orquesta el login
    /// Responsabilidades:
    /// 1. Validar credenciales
    /// 2. Generar token JWT
    /// 3. Actualizar último login
    /// 4. Retornar respuesta consistente
    /// </summary>
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Autentica un usuario y genera un JWT
        /// 
        /// Flujo:
        /// 1. Validar entrada (email y password no vacíos)
        /// 2. Buscar usuario por email en base de datos
        /// 3. Si no existe → Error 401
        /// 4. Si existe → Verificar password con BCrypt
        /// 5. Si password incorrecto → Error 401
        /// 6. Si password correcto → Generar JWT
        /// 7. Actualizar último login
        /// 8. Retornar token + datos del usuario
        /// </summary>
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            // 1️⃣ Validar entrada
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email y password son requeridos"
                };
            }

            // 2️⃣ Buscar usuario por email
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            // 3️⃣ Usuario no existe → 401 Unauthorized
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email o contraseña incorrectos"  // No reveles si existe el email
                };
            }

            // 4️⃣ Verificar contraseña usando BCrypt
            bool isPasswordValid = _passwordHasher.VerifyPassword(dto.Password, user.PasswordHash);

            // 5️⃣ Contraseña incorrecta → 401 Unauthorized
            if (!isPasswordValid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email o contraseña incorrectos"
                };
            }

            // 6️⃣ Generar JWT
            var token = await _tokenService.GenerateTokenAsync(user);

            // 7️⃣ Actualizar último login
            await _userRepository.UpdateLastLoginAsync(user.Id);

            // 8️⃣ Retornar respuesta exitosa
            return new AuthResponseDto
            {
                Success = true,
                Message = "Login exitoso",
                Token = token,
                User = new UserAuthDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role.ToString()
                }
            };
        }
    }
}
```

### Password Hasher Interface
```csharp
// src/MiApp.Application/Interfaces/IPasswordHasher.cs
namespace MiApp.Application.Interfaces
{
    /// <summary>
    /// Contrato para hash y verificación de contraseñas
    /// Implementación: BCrypt (recomendado)
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Genera un hash seguro de la contraseña
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash seguro (incluye salt)</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifica si una contraseña coincide con un hash
        /// </summary>
        /// <param name="password">Contraseña en texto plano a verificar</param>
        /// <param name="hash">Hash almacenado</param>
        /// <returns>true si coincide, false si no</returns>
        bool VerifyPassword(string password, string hash);
    }
}
```

### AutoMapper Configuration
```csharp
// src/MiApp.Application/Mappings/AuthMappingProfile.cs
using AutoMapper;
using MiApp.Application.DTOs;
using MiApp.Domain.Entities;

namespace MiApp.Application.Mappings
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            // User → UserAuthDto
            CreateMap<User, UserAuthDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        }
    }
}
```

### Validator para Login
```csharp
// src/MiApp.Application/Validators/LoginDtoValidator.cs
using FluentValidation;
using MiApp.Application.DTOs;

namespace MiApp.Application.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email es requerido")
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(255).WithMessage("Email muy largo");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Contraseña es requerida")
                .MinimumLength(6).WithMessage("Contraseña debe tener mínimo 6 caracteres")
                .MaximumLength(128).WithMessage("Contraseña muy larga");
        }
    }
}
```

---

## 3️⃣ CAPA DE INFRAESTRUCTURA (Infrastructure)

### JWT Token Service Implementation
```csharp
// src/MiApp.Infrastructure/Services/JwtTokenService.cs
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiApp.Application.Interfaces;
using MiApp.Domain.Entities;

namespace MiApp.Infrastructure.Services
{
    /// <summary>
    /// Implementación de JWT usando JwtSecurityToken
    /// 
    /// Flujo de Generación:
    /// 1. Crear Claims (Identity + autorización)
    /// 2. Crear credenciales de firma (Secret Key + algoritmo)
    /// 3. Crear descriptor del token
    /// 4. Generar y serializar a string
    /// 
    /// Claims Soportados:
    /// - "sub" (Subject): ID del usuario
    /// - "email": Email del usuario
    /// - "name": Nombre completo
    /// - "role": Rol para autorización
    /// </summary>
    public class JwtTokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        public JwtTokenService(IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            
            // ⚠️ CRÍTICO: SecretKey debe tener mínimo 256 bits (32 caracteres)
            _secretKey = jwtSettings["SecretKey"] 
                ?? throw new InvalidOperationException("JwtSettings:SecretKey no configurado");
            
            _issuer = jwtSettings["Issuer"] ?? "MiApp.API";
            _audience = jwtSettings["Audience"] ?? "MiApp.Client";
            _expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            // Validar longitud de la secret key
            if (_secretKey.Length < 32)
            {
                throw new InvalidOperationException(
                    "SecretKey debe tener mínimo 256 bits (32 caracteres)");
            }
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            return await Task.Run(() =>
            {
                // 1️⃣ Crear Claims (payload del JWT)
                var claims = new List<Claim>
                {
                    // Claims estándar
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // sub
                    new Claim(ClaimTypes.Email, user.Email),                    // email
                    new Claim(ClaimTypes.Name, user.FullName),                  // name
                    new Claim(ClaimTypes.Role, user.Role.ToString()),           // role
                    
                    // Custom claims
                    new Claim("aud", _audience),
                    new Claim("iss", _issuer)
                };

                // 2️⃣ Crear credenciales de firma
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // 3️⃣ Crear descriptor del token
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
                    Issuer = _issuer,
                    Audience = _audience,
                    SigningCredentials = credentials
                };

                // 4️⃣ Generar y serializar
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            });
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_secretKey);

                    var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = _issuer,
                        ValidateAudience = true,
                        ValidAudience = _audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero  // No permitir skew de tiempo
                    }, out SecurityToken validatedToken);

                    return validatedToken is JwtSecurityToken;
                }
                catch (Exception ex)
                {
                    // Log: Console.WriteLine($"Token validation failed: {ex.Message}");
                    return false;
                }
            });
        }

        public async Task<Guid?> GetUserIdFromTokenAsync(string token)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadJwtToken(token);
                    
                    var userIdClaim = jwtToken.Claims.FirstOrDefault(
                        c => c.Type == ClaimTypes.NameIdentifier);
                    
                    if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                    {
                        return userId;
                    }

                    return null;
                }
                catch
                {
                    return null;
                }
            });
        }
    }
}
```

### BCrypt Password Hasher Implementation
```csharp
// src/MiApp.Infrastructure/Services/BcryptPasswordHasher.cs
using MiApp.Application.Interfaces;
using BCrypt.Net;

namespace MiApp.Infrastructure.Services
{
    /// <summary>
    /// Implementación de hash de contraseña usando BCrypt
    /// 
    /// ¿Por qué BCrypt?
    /// - Algoritmo de hashing específicamente diseñado para contraseñas
    /// - Incluye salt automático
    /// - Adaptativo: más lento con el tiempo (resistencia a fuerza bruta)
    /// - Estándar de la industria (OWASP recomendado)
    /// 
    /// Características:
    /// - WorkFactor = 12: ~4GB o 50+ años para crackear una contraseña
    /// - Cada hash tiene un salt único
    /// - Imposible revertir (one-way function)
    /// </summary>
    public class BcryptPasswordHasher : IPasswordHasher
    {
        // WorkFactor: número de rondas de hashing (12 es estándar)
        private const int WorkFactor = 12;

        public string HashPassword(string password)
        {
            // BCrypt.Net.BCrypt genera automáticamente el salt
            // Resultado: $2a$12$[22 caracteres de salt][31 caracteres de hash]
            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                // Verifica la contraseña contra el hash (comparando sólo el hash, no el salt)
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
```

### User Repository Implementation
```csharp
// src/MiApp.Infrastructure/Repositories/UserRepository.cs
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiApp.Application.Interfaces;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;

namespace MiApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            // ⚠️ Important: Email debe ser case-insensitive
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateLastLoginAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
```

### DbContext Configuration
```csharp
// src/MiApp.Infrastructure/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using MiApp.Domain.Entities;
using MiApp.Infrastructure.Data.Configurations;

namespace MiApp.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar configuraciones
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            // ... otras configuraciones
        }
    }
}
```

### User Configuration
```csharp
// src/MiApp.Infrastructure/Data/Configurations/UserConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiApp.Domain.Entities;

namespace MiApp.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Tabla
            builder.ToTable("Users");

            // Llave primaria
            builder.HasKey(u => u.Id);

            // Propiedades
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Role)
                .HasConversion<int>()
                .HasDefaultValue(0);  // UserRole.User

            // Índices
            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            // Seed Data (Usuarios de ejemplo)
            builder.HasData(
                new User
                {
                    Id = Guid.Parse("a1b2c3d4-0000-0000-0000-000000000001"),
                    Email = "admin@example.com",
                    FullName = "Administrador",
                    PasswordHash = "$2a$12$...",  // Hash de "password123" con BCrypt
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
```

---

## 4️⃣ CAPA DE PRESENTACIÓN (WebAPI)

### Program.cs - Configuración Completa
```csharp
// src/MiApp.WebApi/Program.cs
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiApp.Application.Interfaces;
using MiApp.Application.Services;
using MiApp.Application.Validators;
using MiApp.Domain.Interfaces;
using MiApp.Infrastructure.Data;
using MiApp.Infrastructure.Repositories;
using MiApp.Infrastructure.Services;
using FluentValidation;
using AutoMapper;

var builder = WebApplicationBuilder.CreateBuilder(args);

// ==========================================
// 1. CONFIGURAR AUTENTICACIÓN (Authentication)
// ==========================================
// 🔐 JWT Setup
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

// ⚠️ CRÍTICO: Validar la configuración antes de usar
if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
{
    throw new InvalidOperationException(
        "JwtSettings:SecretKey debe estar configurado y tener mínimo 32 caracteres en appsettings.json");
}

// AddAuthentication: Define CÓMO se autentica el usuario
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Parámetros de validación del token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,          // ✓ Validar que firma sea correcta
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        
        ValidateIssuer = true,                    // ✓ Validar emisor
        ValidIssuer = issuer,
        
        ValidateAudience = true,                  // ✓ Validar audiencia
        ValidAudience = audience,
        
        ValidateLifetime = true,                  // ✓ Validar expiración
        ClockSkew = TimeSpan.Zero,                // ✗ NO permitir skew de tiempo
        
        // ⚠️ En DESARROLLO solamente: permitir tokens sin validación
        // RequireExpirationTime = false,          // Solo en dev
    };

    // Eventos (opcional, para debugging)
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Token validated for user: {context.Principal?.FindFirst("email")?.Value}");
            return Task.CompletedTask;
        }
    };
});

// ==========================================
// 2. CONFIGURAR AUTORIZACIÓN (Authorization)
// ==========================================
// AddAuthorization: Define QUÉ roles/permisos se requieren
builder.Services.AddAuthorization(options =>
{
    // Policy para Admin
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Policy para Manager o Admin
    options.AddPolicy("ManagerOrAdmin", policy =>
        policy.RequireRole("Manager", "Admin"));

    // Policy con claim custom
    options.AddPolicy("PremiumUser", policy =>
        policy.RequireClaim("subscription", "premium"));
});

// ==========================================
// 3. REGISTRAR SERVICIOS (Dependency Injection)
// ==========================================

// Base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();

// Application Services
builder.Services.AddScoped<AuthService>();

// Infrastructure Services
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando esquema Bearer.\r\n\r\n" +
                      "Introduce \"Bearer\" [espacio] y luego tu token.\r\n\r\n" +
                      "Ejemplo: \"Bearer eyJhbGc...\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// ==========================================
// 4. PIPELINE DE MIDDLEWARE (ORDEN CRÍTICO)
// ==========================================
// ⚠️ EL ORDEN AQUÍ ES CRÍTICO

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS antes de autenticación
app.UseCors("AllowFrontend");

// ✓ UseAuthentication ANTES que UseAuthorization
app.UseAuthentication();      // Identifica al usuario
app.UseAuthorization();       // Verifica permisos

// Middleware custom (si existe)
// app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
```

### appsettings.json - Configuración
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=apimarcos.db"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-with-minimum-32-characters-1234567890ab",
    "Issuer": "MiApp.API",
    "Audience": "MiApp.Client",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Auth Controller
```csharp
// src/MiApp.WebApi/Controllers/AuthController.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApp.Application.DTOs;
using MiApp.Application.Services;
using FluentValidation;

namespace MiApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IValidator<LoginDto> _loginValidator;

        public AuthController(
            AuthService authService,
            IValidator<LoginDto> loginValidator)
        {
            _authService = authService;
            _loginValidator = loginValidator;
        }

        /// <summary>
        /// Login - Obtener JWT Token
        /// 
        /// Request Body:
        /// {
        ///   "email": "user@example.com",
        ///   "password": "password123"
        /// }
        /// 
        /// Response (Success 200):
        /// {
        ///   "success": true,
        ///   "message": "Login exitoso",
        ///   "token": "eyJhbGciOiJIUzI1NiIs...",
        ///   "user": {
        ///     "id": "a1b2c3d4-0000-0000-0000-000000000001",
        ///     "email": "user@example.com",
        ///     "fullName": "John Doe",
        ///     "role": "User"
        ///   }
        /// }
        /// 
        /// Response (Failure 401):
        /// {
        ///   "success": false,
        ///   "message": "Email o contraseña incorrectos"
        /// }
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]  // ✓ Este endpoint NO requiere autenticación
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // Validar entrada
            var validationResult = await _loginValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validación fallida",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            // Ejecutar login
            var result = await _authService.LoginAsync(dto);

            // Retornar respuesta
            if (result.Success)
            {
                return Ok(result);
            }

            // 401 Unauthorized: Credenciales inválidas
            return Unauthorized(result);
        }

        /// <summary>
        /// Get Current User - Obtener datos del usuario autenticado
        /// 
        /// Headers requeridos:
        /// Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
        /// 
        /// Response (Success 200):
        /// {
        ///   "id": "a1b2c3d4-0000-0000-0000-000000000001",
        ///   "email": "user@example.com",
        ///   "fullName": "John Doe",
        ///   "role": "User"
        /// }
        /// 
        /// Response (Failure 401):
        /// "Unauthorized"
        /// </summary>
        [HttpGet("me")]
        [Authorize]  // ✓ Requiere token válido
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var email = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            var name = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            var role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            return Ok(new { userId, email, name, role });
        }

        /// <summary>
        /// Refresh Token - Obtener nuevo token (OPCIONAL)
        /// Este endpoint permite renovar el token sin hacer login nuevamente
        /// </summary>
        [HttpPost("refresh")]
        [Authorize]  // ✓ Requiere token actual válido (aunque esté próximo a expirar)
        public async Task<IActionResult> RefreshToken()
        {
            // Implementación: Generar nuevo token para usuario actual
            var userId = User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // TODO: Implementar lógica de refresh token
            return Ok(new { message = "Token refreshed" });
        }
    }
}
```

---

## 5️⃣ ENDPOINTS PROTEGIDOS

### Ejemplo: Protected Products Controller
```csharp
// src/MiApp.WebApi/Controllers/ProductsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApp.Application.DTOs;
using MiApp.Application.Services;

namespace MiApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // ✓ TODOS los endpoints requieren autenticación
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// GET /api/products
        /// 
        /// Requerimientos:
        /// - Autenticación: ✓ (Token JWT válido)
        /// - Autorización: ✗ (Cualquier usuario autenticado)
        /// - HTTP 401: Token no proporcionado o inválido
        /// - HTTP 403: Token válido pero sin permisos requeridos
        /// </summary>
        [HttpGet]
        [AllowAnonymous]  // ✗ Override: Sin autenticación
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// POST /api/products
        /// 
        /// Requerimientos:
        /// - Autenticación: ✓ (Token JWT válido)
        /// - Autorización: ✓ (Solo Admin)
        /// 
        /// Flujo:
        /// 1. Validar que Authorization header tenga "Bearer {token}"
        /// 2. Validar firma del token
        /// 3. Validar que no esté expirado
        /// 4. Extraer claim "role" del token
        /// 5. Verificar que role == "Admin"
        /// 6. Si no es Admin → 403 Forbidden
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]  // ✓ Solo Admin
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        /// <summary>
        /// PUT /api/products/{id}
        /// 
        /// Requerimientos:
        /// - Autenticación: ✓
        /// - Autorización: ✓ (Manager o Admin)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]  // ✓ Manager o Admin
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            var product = await _productService.UpdateProductAsync(id, dto);
            return Ok(product);
        }

        /// <summary>
        /// DELETE /api/products/{id}
        /// 
        /// Requerimientos:
        /// - Autenticación: ✓
        /// - Autorización: ✓ (Solo Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // ✓ Solo Admin
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductAsync(id);
            return product == null ? NotFound() : Ok(product);
        }
    }
}
```

---

## 🎯 REGLAS DE ORO

### ✅ HACER

```csharp
// ✓ 1. Guardar PasswordHash (BCrypt), NUNCA texto plano
user.PasswordHash = _passwordHasher.HashPassword(plainPassword);

// ✓ 2. Secret Key: Mínimo 32 caracteres (256 bits)
"your-super-secret-key-with-minimum-32-characters-1234567890ab"

// ✓ 3. Usar HTTPS en producción
// ✓ 4. Validar expiración del token
ValidateLifetime = true

// ✓ 5. Incluir solo datos públicos en el payload
claims.Add(new Claim(ClaimTypes.Email, user.Email));

// ✓ 6. Usar IPasswordHasher para validar contraseñas
bool isValid = _passwordHasher.VerifyPassword(input, user.PasswordHash);

// ✓ 7. Implementar rate limiting en login
if (loginAttempts > 5) return TooManyAttempts();

// ✓ 8. Registrar intentos de login fallidos
_logger.LogWarning($"Failed login attempt for {email}");

// ✓ 9. UseAuthentication antes que UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// ✓ 10. Usar [Authorize] para proteger endpoints sensibles
[Authorize(Roles = "Admin")]
```

### ❌ NO HACER

```csharp
// ✗ 1. Guardar contraseña en texto plano
user.Password = plainPassword;  // ¡¡NUNCA!!

// ✗ 2. Secret Key muy corta
"secret123"  // ¡¡INSEGURO!!

// ✗ 3. Incluir datos sensibles en payload
claims.Add(new Claim("password", user.PasswordHash));
claims.Add(new Claim("creditCard", "1234-5678-9012-3456"));

// ✗ 4. Desactivar validación de expiración
ValidateLifetime = false

// ✗ 5. Guardar secret key en el código
const string secretKey = "hardcoded-secret";

// ✗ 6. Comparación manual de contraseñas
if (inputPassword == user.PasswordHash) { }  // Vulnerable a timing attacks

// ✗ 7. Revelar si el email existe
if (userNotFound) return "User not found";  // ¡Información leak!

// ✗ 8. No validar la entrada del usuario
await _authService.LoginAsync(dto);  // Sin validación

// ✗ 9. Poner la Secret Key en appsettings.json publicado
{
  "JwtSettings": {
    "SecretKey": "public-secret-in-repo"  // ¡¡NUNCA!!
  }
}

// ✗ 10. UseAuthorization antes que UseAuthentication
app.UseAuthorization();
app.UseAuthentication();  // ¡¡Orden incorrecto!!
```

---

## 🔍 DIFERENCIA ENTRE 401 Y 403

### 401 Unauthorized - Autenticación Falla
```
El cliente NO ESTÁ autenticado o su token es inválido

Causas:
- Token no proporcionado
- Token expirado
- Token firmado incorrectamente
- Formato inválido: "Bearere token..." o "token..."
- Credenciales incorrectas en login

HTTP Response:
401 Unauthorized
Content-Type: application/json
{
  "message": "Unauthorized - Token missing or invalid"
}

Código Controller:
[HttpGet]
[Authorize]  // Token requerido
public IActionResult GetData()
{
    return Ok("Data");
}

// Cliente sin token → 401
// Cliente con token expirado → 401
```

### 403 Forbidden - Autorización Falla
```
El cliente ESTÁ autenticado pero NO tiene permisos

Causas:
- Token válido pero rol insuficiente
- Falta rol requerido
- Falta claim requerido
- Acceso denegado por policy

HTTP Response:
403 Forbidden
Content-Type: application/json
{
  "message": "Forbidden - Insufficient permissions"
}

Código Controller:
[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]  // Solo Admin
public IActionResult Delete(int id)
{
    return Ok("Deleted");
}

// Usuario autenticado con rol "User" → 403
// Usuario autenticado con rol "Admin" → 200 OK
```

### Comparativa Visual

```
┌─────────────────────────────────────────────────────────┐
│                   FLUJO DE VALIDACIÓN                   │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  Cliente envía request                                  │
│       ↓                                                  │
│  ¿Tiene Authorization header con "Bearer token"?       │
│   ├─ NO  → 401 Unauthorized (No autenticado)          │
│   └─ SÍ  → Validar firma y expiración                 │
│            ├─ Inválido → 401 Unauthorized             │
│            └─ Válido → Extraer claims (role, etc.)    │
│                      ↓                                  │
│              ¿Cumple con autorización?                 │
│              (Role, Policy, Claims)                    │
│               ├─ NO  → 403 Forbidden                  │
│               └─ SÍ  → 200 OK + Respuesta             │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## 🐛 TROUBLESHOOTING

### Problema: "401 Unauthorized" al acceder a endpoint protegido

**Causa 1: Token no enviado**
```csharp
// ❌ Incorrecto
GET /api/products HTTP/1.1

// ✓ Correcto
GET /api/products HTTP/1.1
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Causa 2: Formato de header incorrecto**
```csharp
// ❌ Incorrecto
Authorization: eyJhbGciOiJIUzI1NiIs...
Authorization: Bearere eyJhbGciOiJIUzI1NiIs...

// ✓ Correcto
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Causa 3: Token expirado**
```json
{
  "iat": 1234567890,
  "exp": 1234568950,
  "aud": "MiApp.Client"
}
// Si datetime.UtcNow > exp → Token expirado
```

---

### Problema: "403 Forbidden" con token válido

**Solución:**
```csharp
// Verificar que el usuario tiene el rol requerido
var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
// userRole debe ser "Admin" para [Authorize(Roles = "Admin")]
```

---

### Problema: "The token does not match validation parameters"

**Causas:**
```csharp
// 1. Secret key mismatch
string secretKeyGeneration = "key123";
string secretKeyValidation = "key456";  // ¡¡DIFERENTE!!

// 2. Issuer o Audience mismatch
options.ValidIssuer = "MyApp.API";
options.ValidAudience = "MyApp.Client";

// Pero token fue generado con:
Issuer = "OtherApp.API";
Audience = "OtherApp.Client";

// 3. Algoritmo mismatch
SigningCredentials(key, SecurityAlgorithms.HmacSha512)  // Generado
TokenValidationParameters.IssuerSigningKey = key;       // Validado con SHA256
```

---

### Problema: "ClaimsPrincipal is null"

**Causa:** Token no fue validado correctamente

```csharp
// ❌ Incorrecto - Sin [Authorize]
[HttpGet]
public IActionResult GetData()
{
    var user = User;  // NULL si no hay [Authorize]
    return Ok();
}

// ✓ Correcto
[HttpGet]
[Authorize]  // Primero validar
public IActionResult GetData()
{
    var user = User;  // Poblado con claims del token
    return Ok();
}
```

---

### Problema: Token válido pero no accede a endpoint con [Authorize(Roles = "Admin")]

**Solución:**
```csharp
// Verificar que el token contiene el claim "role" con valor "Admin"
var token = jwtHandler.ReadJwtToken(tokenString);
var roleClaim = token.Claims.FirstOrDefault(c => c.Type == "role");
Console.WriteLine($"Role claim: {roleClaim?.Value}");

// El claim debe ser "Admin" exactamente (case-sensitive)
```

---

## 📊 FLUJO COMPLETO

```
┌──────────────────────────────────────────────────────────────┐
│                   DIAGRAMA COMPLETO                          │
├──────────────────────────────────────────────────────────────┤
│                                                               │
│  FRONTEND                                                    │
│  ├─ POST /api/auth/login                                    │
│  │  { "email": "user@ex.com", "password": "pass123" }       │
│  └─→ BACKEND AuthController.Login()                         │
│       │                                                       │
│       ├─ Validar entrada (FluentValidation)                │
│       ├─ Buscar usuario por email (IUserRepository)        │
│       │  ├─ Usuario no existe → 401                        │
│       │  └─ Usuario existe ↓                                │
│       ├─ Verificar password con BCrypt                     │
│       │  ├─ Contraseña incorrecta → 401                    │
│       │  └─ Contraseña correcta ↓                          │
│       ├─ Generar JWT (ITokenService)                       │
│       │  ├─ Crear Claims (sub, email, role)                │
│       │  ├─ Firmar con Secret Key                          │
│       │  └─ Serializar a string                            │
│       ├─ Actualizar LastLogin                              │
│       └─ Retornar 200 + token                              │
│  ←───────────────────────────────────────────────────────── │
│  FRONTEND recibe token y lo almacena en localStorage       │
│  ├─ GET /api/products                                       │
│  │  Authorization: Bearer {token}                           │
│  └─→ BACKEND ProductsController.GetAll()                   │
│       │                                                       │
│       ├─ JwtBearerHandler extrae token                      │
│       ├─ Valida firma (SymmetricSecurityKey)               │
│       ├─ Valida expiración                                 │
│       ├─ Extrae Claims → ClaimsPrincipal                   │
│       │  ├─ Firma inválida → 401                           │
│       │  ├─ Token expirado → 401                           │
│       │  └─ Válido ↓                                        │
│       ├─ [Authorize] ejecuta → Check claims                │
│       └─ Acceso permitido → Retornar datos 200             │
│  ←──────────────────────────────────────────────────────── │
│  FRONTEND recibe datos                                       │
│                                                               │
└──────────────────────────────────────────────────────────────┘
```

---

## 📦 PAQUETES REQUERIDOS

```bash
# En MiApp.WebApi.csproj
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package BCrypt.Net-Next
dotnet add package FluentValidation
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection

# Versiones recomendadas (.NET 8.0):
# Microsoft.AspNetCore.Authentication.JwtBearer = "8.0.0"
# System.IdentityModel.Tokens.Jwt = "8.0.0"
# BCrypt.Net-Next = "4.0.3"
# FluentValidation = "11.9.0"
# AutoMapper = "12.0.1"
```

---

## 🚀 RESUMEN DE IMPLEMENTACIÓN

### Checklist
- [ ] Crear User entity con PasswordHash
- [ ] Crear UserRole enum (User, Manager, Admin)
- [ ] Implementar IUserRepository
- [ ] Implementar ITokenService con JWT
- [ ] Implementar IPasswordHasher con BCrypt
- [ ] Crear AuthService (Login UseCase)
- [ ] Configurar AddAuthentication + AddAuthorization en Program.cs
- [ ] Crear AuthController con endpoint /login
- [ ] Añadir [Authorize] a endpoints sensibles
- [ ] Configurar appsettings.json con JwtSettings
- [ ] Crear Migration para tabla Users
- [ ] Probar en Swagger con Bearer token
- [ ] Validar 401 (sin token) y 403 (sin permisos)

### Próximos Pasos Avanzados
- Implementar Refresh Tokens
- Integrar con OAuth 2.0 (Google, Microsoft)
- Agregar Two-Factor Authentication (2FA)
- Implementar Rate Limiting en login
- Añadir Revocation List para tokens
- Usar asymmetric keys (RSA) en lugar de HMAC

---

**Documento creado con estándares RFC 7519 y OWASP Best Practices**

*Last Updated: May 17, 2026*
