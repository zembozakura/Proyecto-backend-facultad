# 🏗️ ARQUITECTURA COMPLETA - EXPLICACIÓN DETALLADA

## 📚 TABLA DE CONTENIDOS

1. [Architektura General](#arquitectura)
2. [Explicación Capa por Capa](#capas)
3. [Flujo de una Petición HTTP](#flujo)
4. [Endpoints Disponibles](#endpoints)
5. [Integración Frontend](#frontend)

---

## 🏛️ ARQUITECTURA

```
┌─────────────────────────────────────┐
│         CLIENTE (Frontend)          │  ← React, Vue, Angular, Svelte
├─────────────────────────────────────┤
│  HTTP Requests (JSON)               │
└────────────────┬────────────────────┘
                 ↓
┌─────────────────────────────────────┐
│      API REST (ASP.NET Core)        │  ← Lo que acabamos de crear
│   Controllers → Services → Repo      │
├─────────────────────────────────────┤
│  Port: 5134 (HTTP) / 7249 (HTTPS)   │
└────────────────┬────────────────────┘
                 ↓
┌─────────────────────────────────────┐
│    SQLite Database (apimarcos.db)   │  ← Almacenamiento de datos
│  Tables: Categories, Products, ...  │
└─────────────────────────────────────┘
```

---

## 🎯 EXPLICACIÓN CAPA POR CAPA

### 1️⃣ **DOMAIN LAYER** (Capa de Dominio)
**Ubicación:** `src/MiApp.Domain/`

**Qué contiene:**
- **Entities:** Product, Category, Order, OrderItem
- **Interfaces:** IRepository<T>, IOrderRepository
- **Enums:** OrderStatus

**Responsabilidades:**
- Define las entidades del negocio (datos puros)
- Define contratos (interfaces)
- SIN dependencias externas (es la más independiente)

**Ejemplo:**
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }  // Foreign Key
    public Category? Category { get; set; }  // Relación
}
```

---

### 2️⃣ **APPLICATION LAYER** (Capa de Aplicación)
**Ubicación:** `src/MiApp.Application/`

**Qué contiene:**
- **DTOs:** ProductDto, CreateProductDto, UpdateProductDto
- **Services:** ProductService, OrderService (lógica de negocio)
- **Validators:** Validaciones con FluentValidation
- **Mappings:** AutoMapper para convertir Entity ↔ DTO

**Responsabilidades:**
- Orquestar la lógica de negocio
- Convertir entre Entities y DTOs
- Validar datos
- NO accede directamente a BD

**Ejemplo:**
```csharp
public class ProductService
{
    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        // 1. Validar
        // 2. Mapear DTO → Entity
        // 3. Guardar en BD
        // 4. Mapear Entity → DTO
        // 5. Retornar
    }
}
```

**Por qué DTOs?**
- ✅ Evita exponer la entidad completa
- ✅ Control sobre qué campos se envían
- ✅ Seguridad
- ✅ Evolución sin romper la API

---

### 3️⃣ **INFRASTRUCTURE LAYER** (Capa de Infraestructura)
**Ubicación:** `src/MiApp.Infrastructure/`

**Qué contiene:**
- **DbContext:** ApplicationDbContext (configuración EF Core)
- **Configurations:** Mapeos de entidades a tablas
- **Repositories:** Acceso a datos (CRUD, transacciones, etc)

**Responsabilidades:**
- Conectar a la BD
- Implementar repositorios
- Ejecutar migraciones

**Ejemplo:**
```csharp
public class OrderRepository : IOrderRepository
{
    // Obtiene orden con items en UN SOLO QUERY
    public async Task<Order?> GetOrderWithItemsAsync(Guid orderId)
    {
        return await _context.Orders
            .Include(o => o.Items)  // Eager Loading
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }
}
```

---

### 4️⃣ **PRESENTATION LAYER** (Capa de Presentación)
**Ubicación:** `src/MiApp.WebApi/`

**Qué contiene:**
- **Controllers:** ProductsController, OrdersController, CategoriesController
- **Middleware:** ExceptionHandlingMiddleware (manejo de errores)
- **Program.cs:** Configuración de DI, CORS, etc

**Responsabilidades:**
- Recibir requests HTTP
- Validar (con FluentValidation)
- Llamar servicios
- Devolver respuestas JSON

**Ejemplo:**
```csharp
[HttpPost]
public async Task<ActionResult<ProductDto>> CreateProduct(
    [FromBody] CreateProductDto dto,  // ← DTO
    CancellationToken ct)
{
    var validationResult = await _createValidator.ValidateAsync(dto);
    if (!validationResult.IsValid)
        return BadRequest(validationResult.Errors);
    
    var product = await _productService.CreateProductAsync(dto, ct);
    return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
}
```

---

## 🔄 FLUJO DE UNA PETICIÓN HTTP

```
1. CLIENTE envía:
   POST /api/products
   Body: { "name": "Laptop", "price": 1000, "categoryId": "..." }

2. CONTROLLER recibe:
   ✓ Valida que el JSON es válido
   ✓ Llama al validator de FluentValidation

3. VALIDATOR verifica:
   ✓ Name no vacío y entre 3-100 caracteres
   ✓ Price > 0
   ✓ CategoryId no vacío
   ✓ Si falla → BadRequest 400

4. SERVICE ejecuta lógica:
   ✓ Mapea CreateProductDto → Product Entity
   ✓ Calcula valores derivados (si es necesario)
   ✓ Llama al REPOSITORY para guardar

5. REPOSITORY accede BD:
   ✓ EntityFramework genera SQL
   ✓ Ejecuta INSERT en tabla Products
   ✓ Retorna la entidad guardada

6. SERVICE mapea respuesta:
   ✓ Mapea Product Entity → ProductDto
   ✓ Retorna al CONTROLLER

7. CONTROLLER devuelve:
   ✓ HTTP 201 Created
   ✓ Location header con la nueva URL
   ✓ Body: { "id": 1, "name": "Laptop", "price": 1000, ... }

8. CLIENTE recibe respuesta JSON lista para usar
```

---

## 📡 ENDPOINTS DISPONIBLES

### 🏷️ CATEGORÍAS

```
GET    /api/categories          → Obtiene todas
GET    /api/categories/{id}     → Obtiene una por ID
POST   /api/categories          → Crea nueva
PUT    /api/categories/{id}     → Actualiza
DELETE /api/categories/{id}     → Elimina
```

### 📦 PRODUCTOS

```
GET    /api/products            → Obtiene todas
GET    /api/products/{id}       → Obtiene una por ID
POST   /api/products            → Crea nueva
PUT    /api/products/{id}       → Actualiza
DELETE /api/products/{id}       → Elimina
```

### 📋 ÓRDENES

```
GET    /api/orders              → Obtiene todas
GET    /api/orders/{id}         → Obtiene una (con items)
GET    /api/orders/active/paginated?pageNumber=1&pageSize=10
                                → Órdenes activas paginadas
POST   /api/orders              → Crea nueva (con items)
POST   /api/orders/{id}/cancel  → Cancela una orden
```

---

## 🎨 INTEGRACIÓN FRONTEND

### 🎯 TECNOLOGÍAS RECOMENDADAS

#### **Opción 1: React (Recomendado para ecommerce)**
```bash
npx create-react-app mi-ecommerce
npm install axios react-router-dom zustand
```

**Por qué React?**
- ✅ Mejor ecosistema para ecommerce
- ✅ Comunidad enorme
- ✅ State management (Zustand, Redux)
- ✅ Muchas librerías (Stripe, PayPal, etc)

#### **Opción 2: Vue 3**
```bash
npm create vue@latest
npm install axios vue-router pinia
```

**Por qué Vue?**
- ✅ Más fácil de aprender
- ✅ Excelente documentación
- ✅ Bueno para startups

#### **Opción 3: Angular**
```bash
ng new mi-ecommerce
ng add @angular/material
```

**Por qué Angular?**
- ✅ Full-featured framework
- ✅ TypeScript nativo
- ✅ Enterprise ready

---

### 📝 EJEMPLO: CONSUMIR API CON REACT + AXIOS

```typescript
// services/api.ts
import axios from 'axios';

const API_BASE_URL = 'http://localhost:5134/api';

export const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json'
    }
});

// Obtener productos
export const getProducts = async () => {
    const response = await api.get('/products');
    return response.data;
};

// Crear producto
export const createProduct = async (data) => {
    const response = await api.post('/products', data);
    return response.data;
};

// Obtener órdenes paginadas
export const getActiveOrders = async (pageNumber = 1, pageSize = 10) => {
    const response = await api.get('/orders/active/paginated', {
        params: { pageNumber, pageSize }
    });
    return response.data;
};
```

```typescript
// components/ProductList.tsx
import { useEffect, useState } from 'react';
import { getProducts } from '../services/api';

export default function ProductList() {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const data = await getProducts();
                setProducts(data);
            } catch (error) {
                console.error('Error:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchProducts();
    }, []);

    if (loading) return <div>Cargando...</div>;

    return (
        <div>
            {products.map(product => (
                <div key={product.id}>
                    <h3>{product.name}</h3>
                    <p>${product.price}</p>
                    <button>Agregar al carrito</button>
                </div>
            ))}
        </div>
    );
}
```

---

### 🛒 FLUJO TÍPICO DE ECOMMERCE

```
1. USUARIO navega y ve PRODUCTOS
   GET /api/products

2. USUARIO elige CATEGORÍA
   GET /api/categories
   GET /api/products?categoryId=...

3. USUARIO AGREGA AL CARRITO
   (en el frontend, sin llamar API)

4. USUARIO hace CHECKOUT
   POST /api/orders
   Body: {
       "orderNumber": "ORD-001",
       "items": [
           { "productId": 1, "quantity": 2 },
           { "productId": 3, "quantity": 1 }
       ]
   }

5. BACKEND crea ORDEN ATÓMICA
   - Valida productos existan
   - Valida stock disponible
   - Crea Order + OrderItems en transacción
   - Si falla, revierte TODO

6. USUARIO VE CONFIRMACIÓN
   GET /api/orders/{orderId}

7. USUARIO PUEDE CANCELAR (si no está completada)
   POST /api/orders/{orderId}/cancel
```

---

### 🔐 SEGURIDAD (SIGUIENTES PASOS)

**Agregar Autenticación/Autorización:**

```bash
# 1. Instalar packages
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt

# 2. Configurar en Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { ... });

# 3. Proteger endpoints
[Authorize]
[HttpGet]
public async Task<ActionResult<List<OrderDto>>> GetAllOrders() { ... }
```

---

### 🚀 DEPLOY (SIGUIENTES PASOS)

**Frontend:** Vercel, Netlify, GitHub Pages
**Backend:** Azure App Service, Railway, Heroku

---

## 📊 RESUMEN DE LO QUE HEMOS CREADO

| Componente | Ubicación | Propósito |
|-----------|-----------|----------|
| **Entities** | Domain/Entities | Define el modelo de negocio |
| **Services** | Application/Services | Lógica de negocio |
| **DTOs** | Application/DTOs | Transferencia de datos segura |
| **Validators** | Application/Validators | Validaciones de entrada |
| **Repositories** | Infrastructure/Repositories | Acceso a BD |
| **Controllers** | WebApi/Controllers | Endpoints HTTP |
| **Middleware** | WebApi/Middleware | Manejo global de errores |
| **DbContext** | Infrastructure/Data | Configuración EF Core |

**Total:**
- ✅ 4 Entidades (Category, Product, Order, OrderItem)
- ✅ 3 Controllers (Categories, Products, Orders)
- ✅ 2 Services (ProductService, OrderService)
- ✅ 6 DTOs
- ✅ 2 Validators
- ✅ 2 Repositories (genérico + especializado)
- ✅ 1 Middleware de excepciones
- ✅ CORS configurado
- ✅ Logging configurado
- ✅ AutoMapper configurado
- ✅ FluentValidation configurado

---

## 🎯 PRÓXIMOS PASOS

1. **Crear Frontend** (React, Vue o Angular)
2. **Agregar Autenticación** (JWT)
3. **Agregar Pagos** (Stripe, PayPal)
4. **Agregar Email** (SendGrid)
5. **Agregar Search/Filtros** avanzados
6. **Unit Tests** (xUnit)
7. **Integration Tests** (Testcontainers)
8. **Deploy** en producción

---

¡Tu API está lista para ser consumida! 🚀
