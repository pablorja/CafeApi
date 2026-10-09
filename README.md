# ☕ CafeApi

API REST desarrollada con ASP.NET Core 10, PostgreSQL y Supabase para la gestión de una tienda de café de especialidad.

El proyecto implementa un flujo completo de ecommerce:


Autenticación
    ↓
Catálogo
    ↓
Carrito
    ↓
Pedido
    ↓
Checkout
    ↓
Pago
    ↓
Gestión de Estados


---

# 🚀 Descripción

CafeApi es una API REST construida siguiendo una arquitectura basada en capas mediante Controllers, Interfaces y Repositories.

La aplicación permite gestionar:

- Usuarios
- Autenticación JWT
- Especialidades
- Cafés
- Imágenes
- Carrito de compras
- Pedidos
- Checkout
- Pagos
- Estados de pedidos

La API utiliza PostgreSQL alojado en Supabase y Cloudinary para el almacenamiento de imágenes.

---

# 🛠 Tecnologías Utilizadas

## Backend

- ASP.NET Core 10
- C#
- REST API
- JWT Authentication
- Swagger
- OpenAPI

## Base de Datos

- PostgreSQL
- Supabase
- Npgsql

## Almacenamiento

- Cloudinary

## Herramientas

- Visual Studio
- VS Code
- Postman
- Git
- GitHub

---

# 🏗 Arquitectura


CafeApi
│
├── Controllers
├── DTOs
├── Interfaces
├── Middleware
├── Models
├── Repositories
├── Services
├── Configurations
├── Properties
│
├── Program.cs
├── appsettings.json
├── README.md
└── CHANGELOG.md


Arquitectura utilizada:


Controller
    ↓
Interface
    ↓
Repository
    ↓
PostgreSQL


---

# 🗄️ Configuración de Base de Datos


Motor     : PostgreSQL
Proveedor : Supabase
Conector  : Npgsql
Puerto    : 5432


---

# 📊 Modelo de Datos

Tablas implementadas:


users

especialidades

cafes

carts

cart_items

orders

order_items

payments


---

# 🔐 Seguridad

La API utiliza autenticación basada en JWT.

## Roles

### Administrador

Permisos:


Crear cafés
Actualizar cafés
Eliminar cafés
Gestionar catálogo


### Cliente

Permisos:


Consultar cafés
Gestionar carrito
Crear pedidos
Realizar pagos
Consultar historial


---

# ✅ Módulos Implementados

## Auth

Autenticación de usuarios.

### Endpoints

http
POST /api/auth/register

POST /api/auth/login


### Funcionalidades


Registro

Login

JWT

Validación de credenciales


---

## Users

Gestión de usuarios del dominio de negocio.

### Funcionalidades


Roles

Usuarios autenticados

Integración JWT


---

## Especialidades

CRUD de especialidades.

### Endpoints

http
GET    /api/especialidades

POST   /api/especialidades

PUT    /api/especialidades/{id}

DELETE /api/especialidades/{id}


---

## Cafés

CRUD completo de productos.

### Endpoints

http
GET    /api/cafes

GET    /api/cafes/{id}

POST   /api/cafes

PUT    /api/cafes/{id}

DELETE /api/cafes/{id}


### Funcionalidades


Catálogo

Control de stock

Imágenes

Especialidades


---

## Imágenes

Almacenamiento mediante Cloudinary.

### Endpoint

http
POST /api/images/upload


### Flujo


Cliente
 ↓
Cloudinary
 ↓
URL
 ↓
PostgreSQL


---

## 🛒 Cart

Gestión completa del carrito de compras.

### Endpoints

http
GET    /api/cart

POST   /api/cart/items

PUT    /api/cart/items/{id}

DELETE /api/cart/items/{id}


### Funcionalidades


Agregar productos

Modificar cantidades

Eliminar productos

Vaciar carrito

Calcular subtotales

Calcular total

Validar stock

Seguridad por usuario


---

## 📦 Orders

Conversión de carrito a pedido.

### Endpoints

http
POST /api/orders

GET /api/orders

GET /api/orders/{id}

PATCH /api/orders/{id}/status


### Funcionalidades


Crear pedido

Historial

Detalle de pedido

Order Items

Observaciones

Estados


### Estados disponibles


PendientePago

Pagado

EnPreparacion

Enviado

Entregado

Cancelado


---

## 💳 Checkout

Resumen previo al pago.

### Endpoint

http
GET /api/checkout/{orderId}


### Información


Pedido

Items

CantidadItems

Total

Observaciones

Estado

PuedePagar


---

## 💰 Payments

Gestión de pagos.

### Endpoints

http
POST /api/payments

GET /api/payments/{orderId}


### Funcionalidades


Crear pago

Persistir pago en PostgreSQL

Consultar pago

Relación Pedido-Pago


---

# 🔄 Flujo Ecommerce


Login
 ↓
Catálogo
 ↓
Carrito
 ↓
Pedido
 ↓
Checkout
 ↓
Pago
 ↓
Actualización Estado Pedido


---

# 📡 Resumen de Endpoints

## Auth

http
POST /api/auth/register

POST /api/auth/login


---

## Cafés

http
GET    /api/cafes

GET    /api/cafes/{id}

POST   /api/cafes

PUT    /api/cafes/{id}

DELETE /api/cafes/{id}


---

## Especialidades

http
GET    /api/especialidades

POST   /api/especialidades

PUT    /api/especialidades/{id}

DELETE /api/especialidades/{id}


---

## Cart

http
GET    /api/cart

POST   /api/cart/items

PUT    /api/cart/items/{id}

DELETE /api/cart/items/{id}


---

## Orders

http
POST   /api/orders

GET    /api/orders

GET    /api/orders/{id}

PATCH  /api/orders/{id}/status


---

## Checkout

http
GET /api/checkout/{orderId}


---

## Payments


POST /api/payments

GET /api/payments/{orderId}


---

# 🧪 Testing

La API dispone de colección Postman completa.

Módulos probados:


✅ Auth

✅ Users

✅ Especialidades

✅ Cafes

✅ Images

✅ Cart

✅ Orders

✅ Checkout

✅ Payments

✅ Order Status


---

# 📖 Documentación

Swagger disponible en:


/swagger


OpenAPI:


/openapi/v1.json


---

# 🚧 Roadmap

Próximas funcionalidades:


Integración Wompi

Webhooks

Dashboard administrativo

Notificaciones

Métricas y reportes


---

# 📈 Estado del Proyecto

Backend Ecommerce:


✅ Auth

✅ Users

✅ Especialidades

✅ Cafes

✅ Images

✅ Cart

✅ Orders

✅ Checkout

✅ Payments

✅ Estados de Pedido


Progreso aproximado:


█████████████████████████░ 98%


---

# 👨‍💻 Autor

**Pablo Santamaría**

Proyecto desarrollado como plataforma ecommerce para café de especialidad utilizando ASP.NET Core, PostgreSQL, Supabase y Cloudinary.