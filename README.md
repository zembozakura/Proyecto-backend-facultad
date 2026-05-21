# MiApp API - Backend E-Commerce

Backend profesional desarrollado con **ASP.NET Core 8** implementando **Clean Architecture** para una plataforma e-commerce completa con autenticación JWT y múltiples métodos de pago.

## 📋 Tabla de Contenidos

- [Características](#características)
- [Stack Tecnológico](#stack-tecnológico)
- [Requisitos Previos](#requisitos-previos)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [API Endpoints](#api-endpoints)
- [Autenticación](#autenticación)
- [Base de Datos](#base-de-datos)
- [Métodos de Pago](#métodos-de-pago)
- [Documentación Swagger](#documentación-swagger)

---

## ✨ Características

### Autenticación & Autorización
- ✅ Autenticación JWT (JSON Web Tokens)
- ✅ Sistema de roles (Admin, User)
- ✅ Token Bearer con expiración configurable
- ✅ Middleware global de manejo de excepciones

### Gestión de Productos
- ✅ CRUD completo de productos
- ✅ Categorización de productos
- ✅ Stock management
- ✅ Validaciones con FluentValidation

### Gestión de Pedidos
- ✅ Creación y seguimiento de órdenes
- ✅ Detalles de items en órdenes
- ✅ Estados de órdenes (Pending, Processing, Completed, Cancelled)
- ✅ Relacionamiento con múltiples métodos de pago

### Procesamiento de Pagos
- ✅ Soporte para múltiples métodos de pago:
  - 💳 Tarjeta de crédito/débito
  - 🔗 MercadoPago integration
  - 🏦 Transferencia bancaria
  - 📱 Uala
  - 💰 Efectivo
- ✅ Seguimiento de estado de pagos
- ✅ Webhooks para notificaciones de pago
- ✅ Historial de transacciones

### Arquitectura & Calidad
- ✅ Clean Architecture (4 capas)
- ✅ Repository Pattern
- ✅ Dependency Injection
- ✅ AutoMapper para transformaciones
- ✅ Manejo global de errores
- ✅ CORS configurado
- ✅ Logging centralizado

---

## 🛠️ Stack Tecnológico

| Componente | Versión | Propósito |
|-----------|---------|----------|
| **.NET** | 8.0 | Runtime |
| **ASP.NET Core** | 8.0 | Framework Web |
| **Entity Framework Core** | 8.0 | ORM |
| **SQLite** | Latest | Base de datos |
| **JWT Bearer** | Latest | Autenticación |
| **AutoMapper** | 12.0.1 | Mapping de objetos |
| **FluentValidation** | Latest | Validaciones |
| **Swagger/OpenAPI** | Latest | Documentación API |

---

## 📋 Requisitos Previos

- **.NET SDK 8.0** o superior
- **Git** (para control de versiones)
- **Visual Studio 2022** o **Visual Studio Code**
- **Postman** o **Insomnia** (opcional, para probar endpoints)

### Verificar instalación de .NET:
```bash
dotnet --version
```

---

## 📦 Instalación

### 1. Clonar el repositorio
```bash
git clone https://github.com/zembozakura/Proyecto-backend-facultad.git
cd Proyecto-backend-facultad
```

### 2. Restaurar dependencias
```bash
dotnet restore
```

### 3. Aplicar migraciones de base de datos
```bash
dotnet ef database update --project src/MiApp.Infrastructure --startup-project src/MiApp.WebApi
```

### 4. Ejecutar el proyecto
```bash
dotnet run --project src/MiApp.WebApi
```

**La API estará disponible en:** `http://localhost:5134`

---

## ⚙️ Configuración

### Archivo `appsettings.json`

El archivo contiene configuración sensible y **NO está versionado en Git** por seguridad. Crear localmente:

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
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Variables de Entorno Importantes
- `SecretKey`: Mínimo 32 caracteres, cambiar en producción
- `ExpirationMinutes`: Duración del token JWT
- `DefaultConnection`: Ruta de la base de datos SQLite

---

## 📁 Estructura del Proyecto

```
MiApp/
├── src/
│   ├── MiApp.Domain/                    # Entidades y lógica de dominio
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Product.cs
│   │   │   ├── Category.cs
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   └── Payment.cs
│   │   └── Interfaces/
│   │       ├── IRepository.cs
│   │       ├── IUserRepository.cs
│   │       ├── IOrderRepository.cs
│   │       └── IPaymentRepository.cs
│   │
│   ├── MiApp.Application/               # Lógica de aplicación
│   │   ├── DTOs/
│   │   │   ├── ProductDto.cs
│   │   │   ├── OrderDto.cs
│   │   │   ├── PaymentDto.cs
│   │   │   └── ...
│   │   ├── Services/
│   │   │   ├── ProductService.cs
│   │   │   ├── OrderService.cs
│   │   │   ├── PaymentService.cs
│   │   │   └── LoginUseCase.cs
│   │   ├── Validators/
│   │   │   └── FluentValidation rules
│   │   ├── Interfaces/
│   │   │   └── ITokenService.cs
│   │   └── Mappings/
│   │       └── MappingProfile.cs
│   │
│   ├── MiApp.Infrastructure/            # Acceso a datos
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── Configurations/
│   │   │       ├── UserConfiguration.cs
│   │   │       ├── ProductConfiguration.cs
│   │   │       ├── OrderConfiguration.cs
│   │   │       ├── OrderItemConfiguration.cs
│   │   │       └── PaymentConfiguration.cs
│   │   ├── Repositories/
│   │   │   ├── Repository.cs (genérico)
│   │   │   ├── UserRepository.cs
│   │   │   ├── OrderRepository.cs
│   │   │   └── PaymentRepository.cs
│   │   ├── Services/
│   │   │   └── JwtTokenService.cs
│   │   └── Migrations/
│   │       └── EF Core migrations
│   │
│   └── MiApp.WebApi/                    # Presentación (Controllers)
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── ProductsController.cs
│       │   ├── CategoriesController.cs
│       │   ├── OrdersController.cs
│       │   ├── PaymentsController.cs
│       │   └── ...
│       ├── Middleware/
│       │   └── ExceptionHandlingMiddleware.cs
│       ├── Program.cs                   # Configuración principal
│       ├── appsettings.json             # (NO versionado)
│       └── appsettings.Development.json # (NO versionado)
│
└── APIMARCOS.sln                        # Solución Visual Studio
```

---

## 🔌 API Endpoints

### Autenticación
```
POST /api/auth/login
  Request:  { username, password }
  Response: { token, expiresIn }
```

### Productos
```
GET    /api/products                     # Listar todos
GET    /api/products/{id}                # Obtener detalle
POST   /api/products                     # Crear (Admin)
PUT    /api/products/{id}                # Actualizar (Admin)
DELETE /api/products/{id}                # Eliminar (Admin)
```

### Categorías
```
GET    /api/categories                   # Listar todas
POST   /api/categories                   # Crear (Admin)
PUT    /api/categories/{id}              # Actualizar (Admin)
DELETE /api/categories/{id}              # Eliminar (Admin)
```

### Órdenes
```
GET    /api/orders                       # Listar órdenes del usuario
GET    /api/orders/{id}                  # Obtener detalle
POST   /api/orders                       # Crear nueva orden
PUT    /api/orders/{id}/status           # Cambiar estado (Admin)
```

### Pagos
```
GET    /api/payments/{id}                # Obtener detalle
GET    /api/payments/order/{orderId}     # Listar pagos de orden
POST   /api/payments/mercado-pago/preference   # Crear pago MP
POST   /api/payments/bank-transfer       # Transferencia bancaria
POST   /api/payments/uala                # Pago con Uala
POST   /api/payments/mercado-pago/webhook     # Webhook MP (público)
```

---

## 🔐 Autenticación

### Flujo de Login

1. **Enviar credenciales**
```bash
curl -X POST http://localhost:5134/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"usuario","password":"password"}'
```

2. **Respuesta con JWT Token**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600
}
```

3. **Usar token en headers**
```bash
curl -X GET http://localhost:5134/api/products \
  -H "Authorization: Bearer <TOKEN>"
```

### Token JWT
- **Algoritmo**: HS256 (HMAC SHA256)
- **Duración**: 60 minutos (configurable)
- **Contenido**: Información de usuario y rol
- **Validación**: Issuer y Audience verificados

---

## 🗄️ Base de Datos

### Tecnología
- **Motor**: SQLite
- **Archivo**: `apimarcos.db` (local)
- **Versionamiento**: Entity Framework Core Migrations

### Tablas Principales

| Tabla | Descripción |
|-------|-------------|
| **Users** | Usuarios del sistema |
| **Products** | Catálogo de productos |
| **Categories** | Categorías de productos |
| **Orders** | Órdenes de compra |
| **OrderItems** | Detalles de items en órdenes |
| **Payments** | Transacciones de pago |

### Relaciones
```
Users
  ├── Orders (1:N)
  
Products
  ├── Category (M:1)
  ├── OrderItems (1:N)
  
Orders
  ├── OrderItems (1:N)
  ├── Payments (1:N)
  
Payments
  └── Order (M:1)
```

### Crear Migraciones
```bash
# Agregar nueva migración
dotnet ef migrations add MigracionName --project src/MiApp.Infrastructure

# Aplicar cambios
dotnet ef database update --project src/MiApp.Infrastructure

# Revertir última migración
dotnet ef database update PreviousMigration --project src/MiApp.Infrastructure
```

---

## 💳 Métodos de Pago

### Estructuras de Pago

Cada pago incluye:
- `Id`: Identificador único
- `OrderId`: Referencia a la orden
- `Amount`: Monto en ARS
- `Currency`: Moneda (por defecto ARS)
- `Status`: Pending, Processing, Completed, Failed, Cancelled, Refunded
- `Method`: CreditCard, DebitCard, MercadoPago, BankTransfer, Uala, Cash
- `CreatedAt` / `UpdatedAt`: Timestamps

### MercadoPago
```json
POST /api/payments/mercado-pago/preference
{
  "orderId": "uuid",
  "amount": 1000.00
}
```

**Respuesta**:
```json
{
  "initPoint": "https://checkout.mercadopago.com/checkout/v1/redirect?pref_id=xxx",
  "sandboxInitPoint": "https://sandbox.mercadopago.com/checkout/v1/redirect?pref_id=xxx"
}
```

### Transferencia Bancaria
```json
POST /api/payments/bank-transfer
{
  "orderId": "uuid",
  "amount": 1000.00,
  "bankName": "Banco Provincia",
  "reference": "Referencia para banco"
}
```

### Uala
```json
POST /api/payments/uala
{
  "orderId": "uuid",
  "amount": 1000.00,
  "phoneNumber": "+541234567890"
}
```

---

## 📚 Documentación Swagger

### Acceder a Swagger
```
http://localhost:5134/swagger
```

Swagger proporciona:
- ✅ Listado completo de endpoints
- ✅ Descripción de cada operación
- ✅ Esquemas de request/response
- ✅ Pruebas interactivas (Try it out)
- ✅ Autenticación Bearer integrada

### Autenticación en Swagger
1. Hacer click en "Authorize" (arriba a la derecha)
2. Ingresar token: `Bearer <tu_jwt_token>`
3. Hacer click en "Authorize"
4. Ahora todos los requests incluirán el token

---

## 🚀 Deployment

### Compilar para Producción
```bash
dotnet build -c Release
```

### Publicar
```bash
dotnet publish -c Release -o ./publish
```

### Requisitos en Servidor
- .NET Runtime 8.0
- Base de datos SQLite accesible
- Variables de entorno configuradas
- Certificado HTTPS

---

## 🧪 Testing

### Ejecutar Tests (cuando estén implementados)
```bash
dotnet test
```

### Probar Endpoints con cURL

**Obtener productos**:
```bash
curl http://localhost:5134/api/products
```

**Crear producto (requiere token Admin)**:
```bash
curl -X POST http://localhost:5134/api/products \
  -H "Authorization: Bearer <TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "name":"Laptop",
    "description":"Laptop gaming",
    "price":1500.00,
    "stock":10,
    "categoryId":1
  }'
```

---

## 📊 Manejo de Errores

La API devuelve errores estándar HTTP:

```json
{
  "message": "Descripción del error",
  "statusCode": 400,
  "timestamp": "2026-05-21T10:30:00Z"
}
```

### Códigos Comunes
- `200 OK`: Solicitud exitosa
- `201 Created`: Recurso creado
- `400 Bad Request`: Datos inválidos
- `401 Unauthorized`: Token no proporcionado/inválido
- `403 Forbidden`: Permiso insuficiente
- `404 Not Found`: Recurso no encontrado
- `500 Internal Server Error`: Error del servidor

---

## 🔧 Troubleshooting

### Error: "Connection refused"
- Verificar que el backend esté corriendo: `dotnet run --project src/MiApp.WebApi`
- Verificar puerto 5134 disponible

### Error: "Database not found"
```bash
# Aplicar migraciones
dotnet ef database update --project src/MiApp.Infrastructure --startup-project src/MiApp.WebApi
```

### Error: "Unauthorized"
- Verificar token JWT válido
- Verificar token no expirado
- Verificar header: `Authorization: Bearer <TOKEN>`

### Error: "Forbidden"
- Usuario sin permisos suficientes
- Requiere rol Admin para algunas operaciones

---

## 📝 Notas de Desarrollo

### Convenciones de Código
- PascalCase para nombres de clases y métodos públicos
- camelCase para variables locales
- Usar async/await para operaciones I/O
- DTOs para transferencia de datos entre capas

### Agregar Nuevo Endpoint
1. Crear entidad en `Domain/Entities`
2. Crear DTO en `Application/DTOs`
3. Crear repositorio en `Infrastructure/Repositories`
4. Crear servicio en `Application/Services`
5. Inyectar en `Program.cs`
6. Crear controller en `WebApi/Controllers`
7. Agregar mapping en `MappingProfile.cs`

### Security Best Practices
- ✅ JWT tokens con expiración
- ✅ Contraseñas hasheadas
- ✅ CORS configurado
- ✅ Validación de entrada
- ✅ Middleware de error handling
- ✅ Archivos sensibles ignorados en git

---

## 📄 Licencia

Este proyecto es académico y está disponible bajo licencia MIT.

---

## 👨‍💻 Autor

Desarrollado por **Marco** como proyecto de backend en Clean Architecture.

---

## 📞 Soporte

Para reportar problemas o sugerencias, crear un issue en el repositorio:
[GitHub Issues](https://github.com/zembozakura/Proyecto-backend-facultad/issues)

---

## 🔗 Enlaces Útiles

- [ASP.NET Core Docs](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [JWT.io](https://jwt.io/)
- [Swagger/OpenAPI](https://swagger.io/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

**¡Gracias por usar MiApp API!** 🚀
