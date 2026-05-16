# 🎨 GUÍA PARA CREAR FRONTEND - ECOMMERCE

## 🎯 ¿QUÉ VAMOS A CONSTRUIR?

```
┌─────────────────────────────────────────┐
│         ECOMMERCE FRONTEND              │
├─────────────────────────────────────────┤
│ ✅ Catálogo de Productos                │
│ ✅ Carrito de Compras                   │
│ ✅ Sistema de Órdenes                   │
│ ✅ Búsqueda y Filtros                   │
│ ✅ Gestión de Categorías                │
│ ✅ Responsivo (Mobile + Desktop)        │
└─────────────────────────────────────────┘
           ↓
    Consume tu API
   (http://localhost:5134)
```

---

## 🚀 OPCIÓN 1: REACT (RECOMENDADO)

### Paso 1: Crear el proyecto

```bash
# Crear app con Vite (más rápido que CRA)
npm create vite@latest mi-ecommerce -- --template react
cd mi-ecommerce
npm install

# Instalar dependencias
npm install axios react-router-dom zustand
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```

### Paso 2: Estructura de carpetas

```
src/
├── components/
│   ├── ProductCard.jsx
│   ├── ProductList.jsx
│   ├── Cart.jsx
│   ├── Navbar.jsx
│   └── Footer.jsx
├── pages/
│   ├── Home.jsx
│   ├── Products.jsx
│   ├── Cart.jsx
│   ├── Checkout.jsx
│   └── OrderConfirmation.jsx
├── services/
│   └── api.js
├── store/
│   └── cartStore.js
├── styles/
│   └── tailwind.css
├── App.jsx
└── main.jsx
```

### Paso 3: Configurar API client

```javascript
// src/services/api.js
import axios from 'axios';

const API_BASE_URL = 'http://localhost:5134/api';

export const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json'
    }
});

// Productos
export const productService = {
    getAll: () => api.get('/products'),
    getById: (id) => api.get(`/products/${id}`),
    create: (data) => api.post('/products', data),
    update: (id, data) => api.put(`/products/${id}`, data),
    delete: (id) => api.delete(`/products/${id}`)
};

// Órdenes
export const orderService = {
    getAll: () => api.get('/orders'),
    getById: (id) => api.get(`/orders/${id}`),
    getActive: (page = 1, pageSize = 10) => 
        api.get(`/orders/active/paginated`, { params: { pageNumber: page, pageSize } }),
    create: (data) => api.post('/orders', data),
    cancel: (id) => api.post(`/orders/${id}/cancel`)
};

// Categorías
export const categoryService = {
    getAll: () => api.get('/categories'),
    getById: (id) => api.get(`/categories/${id}`),
    create: (data) => api.post('/categories', data),
    update: (id, data) => api.put(`/categories/${id}`, data),
    delete: (id) => api.delete(`/categories/${id}`)
};
```

### Paso 4: State Management con Zustand

```javascript
// src/store/cartStore.js
import { create } from 'zustand';

export const useCartStore = create((set, get) => ({
    items: [],
    
    addToCart: (product, quantity) => set((state) => {
        const existingItem = state.items.find(item => item.id === product.id);
        if (existingItem) {
            return {
                items: state.items.map(item =>
                    item.id === product.id
                        ? { ...item, quantity: item.quantity + quantity }
                        : item
                )
            };
        }
        return {
            items: [...state.items, { ...product, quantity }]
        };
    }),

    removeFromCart: (productId) => set((state) => ({
        items: state.items.filter(item => item.id !== productId)
    })),

    updateQuantity: (productId, quantity) => set((state) => ({
        items: state.items.map(item =>
            item.id === productId
                ? { ...item, quantity }
                : item
        )
    })),

    clearCart: () => set({ items: [] }),

    getTotalPrice: () => {
        const { items } = get();
        return items.reduce((total, item) => total + (item.price * item.quantity), 0);
    }
}));
```

### Paso 5: Componente Principal

```jsx
// src/App.jsx
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import Home from './pages/Home';
import Products from './pages/Products';
import Cart from './pages/Cart';
import Checkout from './pages/Checkout';
import OrderConfirmation from './pages/OrderConfirmation';

function App() {
    return (
        <Router>
            <Navbar />
            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/products" element={<Products />} />
                <Route path="/cart" element={<Cart />} />
                <Route path="/checkout" element={<Checkout />} />
                <Route path="/order/:orderId" element={<OrderConfirmation />} />
            </Routes>
        </Router>
    );
}

export default App;
```

### Paso 6: Página de Productos

```jsx
// src/pages/Products.jsx
import { useEffect, useState } from 'react';
import { productService } from '../services/api';
import ProductCard from '../components/ProductCard';

export default function Products() {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const response = await productService.getAll();
                setProducts(response.data);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchProducts();
    }, []);

    if (loading) return <div className="text-center p-4">Cargando...</div>;
    if (error) return <div className="text-red-500 p-4">Error: {error}</div>;

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-3xl font-bold mb-8">Catálogo de Productos</h1>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {products.map(product => (
                    <ProductCard key={product.id} product={product} />
                ))}
            </div>
        </div>
    );
}
```

### Paso 7: Componente de Producto

```jsx
// src/components/ProductCard.jsx
import { useCartStore } from '../store/cartStore';

export default function ProductCard({ product }) {
    const addToCart = useCartStore(state => state.addToCart);

    return (
        <div className="border rounded-lg shadow-md p-4 hover:shadow-lg transition">
            <div className="bg-gray-200 h-48 rounded mb-4 flex items-center justify-center">
                <span className="text-gray-500">Imagen del producto</span>
            </div>
            <h3 className="text-xl font-bold">{product.name}</h3>
            <p className="text-gray-600 text-sm mb-2">{product.description}</p>
            <p className="text-lg font-bold text-green-600 mb-4">${product.price}</p>
            <p className="text-sm text-gray-500 mb-4">
                Stock: {product.stock > 0 ? product.stock : 'Agotado'}
            </p>
            <button
                onClick={() => addToCart(product, 1)}
                disabled={product.stock === 0}
                className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 disabled:bg-gray-400"
            >
                Agregar al carrito
            </button>
        </div>
    );
}
```

### Paso 8: Página del Carrito

```jsx
// src/pages/Cart.jsx
import { useCartStore } from '../store/cartStore';
import { Link } from 'react-router-dom';

export default function Cart() {
    const { items, removeFromCart, updateQuantity, clearCart, getTotalPrice } = useCartStore();

    if (items.length === 0) {
        return (
            <div className="container mx-auto p-4 text-center">
                <h1 className="text-3xl font-bold mb-4">Carrito Vacío</h1>
                <Link to="/products" className="text-blue-600 underline">
                    Volver a productos
                </Link>
            </div>
        );
    }

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-3xl font-bold mb-8">Mi Carrito</h1>
            
            <table className="w-full mb-8">
                <thead>
                    <tr className="border-b">
                        <th className="text-left">Producto</th>
                        <th className="text-center">Cantidad</th>
                        <th className="text-right">Precio</th>
                        <th className="text-right">Subtotal</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {items.map(item => (
                        <tr key={item.id} className="border-b">
                            <td>{item.name}</td>
                            <td className="text-center">
                                <input
                                    type="number"
                                    min="1"
                                    value={item.quantity}
                                    onChange={(e) => updateQuantity(item.id, parseInt(e.target.value))}
                                    className="w-12 text-center border rounded"
                                />
                            </td>
                            <td className="text-right">${item.price}</td>
                            <td className="text-right">${(item.price * item.quantity).toFixed(2)}</td>
                            <td className="text-right">
                                <button
                                    onClick={() => removeFromCart(item.id)}
                                    className="text-red-600 hover:text-red-800"
                                >
                                    Eliminar
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <div className="flex justify-between items-center mb-8">
                <button
                    onClick={clearCart}
                    className="bg-gray-600 text-white px-4 py-2 rounded hover:bg-gray-700"
                >
                    Limpiar carrito
                </button>
                <div className="text-2xl font-bold">
                    Total: ${getTotalPrice().toFixed(2)}
                </div>
            </div>

            <Link
                to="/checkout"
                className="block w-full bg-green-600 text-white py-3 rounded text-center font-bold hover:bg-green-700"
            >
                Proceder al Checkout
            </Link>
        </div>
    );
}
```

### Paso 9: Página de Checkout

```jsx
// src/pages/Checkout.jsx
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCartStore } from '../store/cartStore';
import { orderService } from '../services/api';

export default function Checkout() {
    const navigate = useNavigate();
    const { items, getTotalPrice, clearCart } = useCartStore();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [orderNumber, setOrderNumber] = useState(`ORD-${Date.now()}`);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            const orderData = {
                orderNumber,
                items: items.map(item => ({
                    productId: item.id,
                    quantity: item.quantity
                }))
            };

            const response = await orderService.create(orderData);
            clearCart();
            navigate(`/order/${response.data.id}`);
        } catch (err) {
            setError(err.response?.data?.message || 'Error al crear la orden');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-3xl font-bold mb-8">Checkout</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div>
                    <h2 className="text-xl font-bold mb-4">Resumen del Pedido</h2>
                    <div className="border rounded p-4 mb-8">
                        {items.map(item => (
                            <div key={item.id} className="flex justify-between mb-2">
                                <span>{item.name} x{item.quantity}</span>
                                <span>${(item.price * item.quantity).toFixed(2)}</span>
                            </div>
                        ))}
                        <div className="border-t pt-2 mt-2 text-lg font-bold">
                            Total: ${getTotalPrice().toFixed(2)}
                        </div>
                    </div>
                </div>

                <div>
                    <h2 className="text-xl font-bold mb-4">Información de Envío</h2>
                    <form onSubmit={handleSubmit}>
                        {error && <div className="bg-red-100 text-red-700 p-3 rounded mb-4">{error}</div>}

                        <div className="mb-4">
                            <label className="block text-sm font-medium mb-2">Número de Orden</label>
                            <input
                                type="text"
                                value={orderNumber}
                                onChange={(e) => setOrderNumber(e.target.value)}
                                className="w-full border rounded px-3 py-2"
                                required
                            />
                        </div>

                        <div className="mb-4">
                            <label className="block text-sm font-medium mb-2">Nombre Completo</label>
                            <input type="text" className="w-full border rounded px-3 py-2" required />
                        </div>

                        <div className="mb-4">
                            <label className="block text-sm font-medium mb-2">Email</label>
                            <input type="email" className="w-full border rounded px-3 py-2" required />
                        </div>

                        <div className="mb-4">
                            <label className="block text-sm font-medium mb-2">Dirección</label>
                            <input type="text" className="w-full border rounded px-3 py-2" required />
                        </div>

                        <button
                            type="submit"
                            disabled={loading}
                            className="w-full bg-green-600 text-white py-2 rounded hover:bg-green-700 disabled:bg-gray-400"
                        >
                            {loading ? 'Procesando...' : 'Completar Compra'}
                        </button>
                    </form>
                </div>
            </div>
        </div>
    );
}
```

### Paso 10: Ejecutar

```bash
npm run dev
# Abre http://localhost:5173
```

---

## 🎨 OPCIÓN 2: VUE 3

```bash
npm create vue@latest mi-ecommerce
cd mi-ecommerce
npm install
npm install axios vue-router pinia
npm run dev
```

**Estructura similar a React pero con Composition API**

---

## 🎨 OPCIÓN 3: ANGULAR

```bash
ng new mi-ecommerce
cd mi-ecommerce
ng add @angular/material
npm install axios
ng serve
```

---

## 📱 CARACTERÍSTICAS RECOMENDADAS

| Característica | Propósito |
|---|---|
| **Búsqueda** | Filtrar productos por nombre |
| **Filtros por categoría** | Mostrar solo productos de una categoría |
| **Paginación** | Mostrar 10 productos por página |
| **Favoritos** | Guardar productos favoritos en localStorage |
| **Reviews** | Calificaciones y comentarios (requiere backend adicional) |
| **Wishlist** | Lista de deseos |
| **Notificaciones** | Toast notifications para acciones |
| **Dark Mode** | Tema oscuro/claro |

---

## 🛒 FLUJO COMPLETO

```
1. Usuario abre la app → Home Page
2. Navega a Productos → ProductList (GET /api/products)
3. Selecciona categoría → Filtra productos
4. Agrega al carrito → useCartStore
5. Abre carrito → Muestra items
6. Procede a checkout → Checkout Page
7. Completa datos y paga → POST /api/orders
8. Backend crea orden atómica
9. Confirmación → OrderConfirmation Page
10. Usuario ve el número de orden
```

---

## 🚀 DEPLOYMENT

### Frontend en Vercel (Recomendado para React)

```bash
# Instalar Vercel CLI
npm i -g vercel

# Deploy
vercel
```

### Backend en Railway o Heroku

```bash
# Railway
railway link
railway up

# O en Azure App Service
az webapp up --name mi-api --resource-group mi-grupo
```

---

## 📊 ESTRUCTURA FINAL DEL PROYECTO

```
mi-ecommerce-fullstack/
├── Backend (ASP.NET Core)
│   ├── src/
│   │   ├── MiApp.Domain/
│   │   ├── MiApp.Application/
│   │   ├── MiApp.Infrastructure/
│   │   └── MiApp.WebApi/
│   ├── apimarcos.db
│   └── APIMARCOS.sln
│
└── Frontend (React)
    ├── src/
    │   ├── components/
    │   ├── pages/
    │   ├── services/
    │   ├── store/
    │   └── App.jsx
    ├── package.json
    └── vite.config.js
```

---

## ✅ CHECKLIST FRONTEND

- [ ] ✅ React/Vue/Angular instalado
- [ ] ✅ Axios/Fetch configurado
- [ ] ✅ Rutas definidas
- [ ] ✅ Componentes creados
- [ ] ✅ State management implementado
- [ ] ✅ CORS funcionando (backend permitiendo frontend)
- [ ] ✅ Carrito funcionando
- [ ] ✅ Checkout completado
- [ ] ✅ Órdenes mostrando correctamente
- [ ] ✅ Error handling implementado
- [ ] ✅ Loading states implementados
- [ ] ✅ Responsive design
- [ ] ✅ Testeable

---

¡Ahora tienes todo para construir un ecommerce profesional! 🚀

**Recomendación:** Usa React con Vite + TailwindCSS = Combinación ganadora
