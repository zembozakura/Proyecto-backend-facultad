# 📞 GUÍA DE PETICIONES HTTP - EJEMPLOS PRÁCTICOS

## 🎯 BASE URL
```
http://localhost:5134/api
```

---

## 🏷️ CATEGORÍAS

### GET - Obtener todas las categorías
```bash
curl -X GET "http://localhost:5134/api/categories" \
  -H "Accept: application/json"
```

**Respuesta (200 OK):**
```json
[
    {
        "id": "a1b2c3d4-0000-0000-0000-000000000001",
        "name": "Electrónica"
    },
    {
        "id": "a1b2c3d4-0000-0000-0000-000000000002",
        "name": "Ropa"
    },
    {
        "id": "a1b2c3d4-0000-0000-0000-000000000003",
        "name": "Hogar"
    }
]
```

### GET - Obtener una categoría por ID
```bash
curl -X GET "http://localhost:5134/api/categories/a1b2c3d4-0000-0000-0000-000000000001" \
  -H "Accept: application/json"
```

### POST - Crear nueva categoría
```bash
curl -X POST "http://localhost:5134/api/categories" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Deportes"
  }'
```

**Respuesta (201 Created):**
```json
{
    "id": "d5e6f7g8-0000-0000-0000-000000000004",
    "name": "Deportes"
}
```

### PUT - Actualizar categoría
```bash
curl -X PUT "http://localhost:5134/api/categories/d5e6f7g8-0000-0000-0000-000000000004" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Deportes y Recreación"
  }'
```

### DELETE - Eliminar categoría
```bash
curl -X DELETE "http://localhost:5134/api/categories/d5e6f7g8-0000-0000-0000-000000000004"
```

**Respuesta (204 No Content)** - Sin body

---

## 📦 PRODUCTOS

### GET - Obtener todos los productos
```bash
curl -X GET "http://localhost:5134/api/products" \
  -H "Accept: application/json"
```

**Respuesta (200 OK):**
```json
[
    {
        "id": 1,
        "name": "Laptop Dell",
        "description": "Laptop gaming 15.6 pulgadas",
        "price": 1500.00,
        "stock": 5,
        "createdAt": "2026-05-15T23:14:26.1234567Z",
        "categoryId": "a1b2c3d4-0000-0000-0000-000000000001",
        "category": {
            "id": "a1b2c3d4-0000-0000-0000-000000000001",
            "name": "Electrónica"
        }
    }
]
```

### GET - Obtener un producto por ID
```bash
curl -X GET "http://localhost:5134/api/products/1" \
  -H "Accept: application/json"
```

### POST - Crear nuevo producto
```bash
curl -X POST "http://localhost:5134/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Monitor LG 27\"",
    "description": "Monitor 4K 144Hz",
    "price": 599.99,
    "stock": 10,
    "categoryId": "a1b2c3d4-0000-0000-0000-000000000001"
  }'
```

**Errores posibles (400 Bad Request):**
```json
{
    "errors": [
        {
            "propertyName": "Price",
            "errorMessage": "El precio debe ser mayor a 0"
        }
    ]
}
```

### PUT - Actualizar producto
```bash
curl -X PUT "http://localhost:5134/api/products/1" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop Dell XPS",
    "price": 1699.99,
    "stock": 3
  }'
```

**Nota:** Los campos vacíos (null) se ignoran

### DELETE - Eliminar producto
```bash
curl -X DELETE "http://localhost:5134/api/products/1"
```

---

## 📋 ÓRDENES

### GET - Obtener todas las órdenes
```bash
curl -X GET "http://localhost:5134/api/orders" \
  -H "Accept: application/json"
```

### GET - Obtener una orden con sus items
```bash
curl -X GET "http://localhost:5134/api/orders/a1b2c3d4-0000-0000-0000-000000000005" \
  -H "Accept: application/json"
```

**Respuesta (200 OK):**
```json
{
    "id": "a1b2c3d4-0000-0000-0000-000000000005",
    "orderNumber": "ORD-001",
    "createdAt": "2026-05-15T23:45:00Z",
    "status": "Pending",
    "totalAmount": 2099.98,
    "items": [
        {
            "id": "b2c3d4e5-0000-0000-0000-000000000001",
            "orderId": "a1b2c3d4-0000-0000-0000-000000000005",
            "productId": 1,
            "productName": "Laptop Dell",
            "quantity": 2,
            "unitPrice": 1050.00,
            "total": 2100.00
        }
    ]
}
```

### GET - Obtener órdenes activas CON PAGINACIÓN
```bash
curl -X GET "http://localhost:5134/api/orders/active/paginated?pageNumber=1&pageSize=10" \
  -H "Accept: application/json"
```

**Respuesta (200 OK):**
```json
{
    "data": [
        {
            "id": "a1b2c3d4-0000-0000-0000-000000000005",
            "orderNumber": "ORD-001",
            "createdAt": "2026-05-15T23:45:00Z",
            "status": "Pending",
            "totalAmount": 2099.98,
            "items": [...]
        }
    ],
    "pagination": {
        "pageNumber": 1,
        "pageSize": 10,
        "total": 5,
        "totalPages": 1
    }
}
```

### POST - Crear nueva orden (con items)
```bash
curl -X POST "http://localhost:5134/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "orderNumber": "ORD-002",
    "items": [
        {
            "productId": 1,
            "quantity": 1
        },
        {
            "productId": 2,
            "quantity": 3
        }
    ]
  }'
```

**Respuesta (201 Created):**
```json
{
    "id": "c3d4e5f6-0000-0000-0000-000000000006",
    "orderNumber": "ORD-002",
    "createdAt": "2026-05-16T00:00:00Z",
    "status": "Pending",
    "totalAmount": 3750.00,
    "items": [
        {
            "id": "d4e5f6g7-0000-0000-0000-000000000001",
            "orderId": "c3d4e5f6-0000-0000-0000-000000000006",
            "productId": 1,
            "productName": "Laptop Dell",
            "quantity": 1,
            "unitPrice": 1500.00,
            "total": 1500.00
        },
        {
            "id": "d4e5f6g7-0000-0000-0000-000000000002",
            "orderId": "c3d4e5f6-0000-0000-0000-000000000006",
            "productId": 2,
            "productName": "Monitor LG 27\"",
            "quantity": 3,
            "unitPrice": 599.99,
            "total": 1799.97
        }
    ]
}
```

### POST - Cancelar una orden
```bash
curl -X POST "http://localhost:5134/api/orders/c3d4e5f6-0000-0000-0000-000000000006/cancel" \
  -H "Accept: application/json"
```

**Respuesta (200 OK):**
```json
{
    "message": "Orden cancelada exitosamente"
}
```

---

## ❌ CÓDIGOS DE ERROR COMUNES

### 400 - Bad Request (Validación fallida)
```json
{
    "errors": [
        {
            "propertyName": "Name",
            "errorMessage": "El nombre del producto es requerido"
        }
    ]
}
```

### 404 - Not Found
```json
{
    "message": "Producto 999 no encontrado"
}
```

### 500 - Internal Server Error
```json
{
    "success": false,
    "message": "Ocurrió un error interno",
    "details": "..."
}
```

---

## 🧪 PRUEBAS CON POSTMAN

1. **Descargar Postman** desde https://www.postman.com/downloads/

2. **Crear colección:** `Mi Ecommerce API`

3. **Agregar requests:**
   - GET /api/categories
   - POST /api/products
   - POST /api/orders
   - etc.

4. **Variables de entorno:**
   ```json
   {
     "baseUrl": "http://localhost:5134/api",
     "categoryId": "a1b2c3d4-0000-0000-0000-000000000001",
     "orderId": "a1b2c3d4-0000-0000-0000-000000000005"
   }
   ```

---

## 🧬 TESTS CON JAVASCRIPT (Fetch API)

```javascript
// Obtener todos los productos
async function getProducts() {
    const response = await fetch('http://localhost:5134/api/products');
    const products = await response.json();
    console.log(products);
}

// Crear una orden
async function createOrder() {
    const response = await fetch('http://localhost:5134/api/orders', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            orderNumber: 'ORD-003',
            items: [
                { productId: 1, quantity: 1 }
            ]
        })
    });
    const order = await response.json();
    console.log(order);
}

// Con error handling
async function getProductsWithErrorHandling() {
    try {
        const response = await fetch('http://localhost:5134/api/products');
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const products = await response.json();
        console.log(products);
    } catch (error) {
        console.error('Error:', error);
    }
}

// Ejecutar
getProducts();
createOrder();
```

---

## 🎨 INTEGRACIÓN CON REACT (Axios)

```typescript
import axios from 'axios';

const apiClient = axios.create({
    baseURL: 'http://localhost:5134/api'
});

// Obtener productos
export const fetchProducts = async () => {
    const { data } = await apiClient.get('/products');
    return data;
};

// Crear producto
export const createProduct = async (productData) => {
    const { data } = await apiClient.post('/products', productData);
    return data;
};

// Crear orden
export const createOrder = async (orderData) => {
    const { data } = await apiClient.post('/orders', orderData);
    return data;
};

// En tu componente React
import { useEffect, useState } from 'react';
import { fetchProducts, createOrder } from './api';

function ProductCatalog() {
    const [products, setProducts] = useState([]);

    useEffect(() => {
        fetchProducts().then(setProducts);
    }, []);

    const handleCheckout = async (items) => {
        const order = await createOrder({
            orderNumber: `ORD-${Date.now()}`,
            items: items
        });
        console.log('Orden creada:', order);
    };

    return (
        <div>
            {products.map(p => (
                <div key={p.id}>
                    <h3>{p.name}</h3>
                    <p>${p.price}</p>
                </div>
            ))}
        </div>
    );
}

export default ProductCatalog;
```

---

## ✅ CHECKLIST ANTES DE PRODUCCIÓN

- [ ] ✅ API compilando sin errores
- [ ] ✅ Swagger funcionando (http://localhost:5134/swagger)
- [ ] ✅ Todas las rutas probadas
- [ ] ✅ CORS configurado correctamente
- [ ] ✅ Validaciones funcionando
- [ ] ✅ Manejo de errores funcionando
- [ ] ✅ Logging configurado
- [ ] ✅ Database migrations aplicadas
- [ ] ✅ Tests unitarios (si aplica)
- [ ] ✅ Documentación completada

---

¡Ahora estás listo para conectar tu frontend! 🚀
