---

## [2.0.0] - 2026-09-26

### 🚀 Añadido

## Ecommerce Core

Implementación completa del flujo principal de ecommerce.


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
Estado del Pedido


---

### 🛒 Checkout

#### DTOs

- CheckoutResponseDto

#### Interfaces

- ICheckoutRepository

#### Repositorios

- CheckoutRepository

#### Controladores

- CheckoutController

#### Endpoints

http
GET /api/checkout/{orderId}


#### Funcionalidades

- Resumen previo al pago
- Obtención de productos del pedido
- Obtención de cantidadItems
- Obtención de total
- Validación de estado
- Validación de propiedad del pedido
- Indicador PuedePagar

---

### 💳 Payments

#### DTOs

- PaymentRequestDto
- PaymentResponseDto

#### Interfaces

- IPaymentRepository

#### Repositorios

- PaymentRepository

#### Controladores

- PaymentsController

#### Endpoints

http
POST /api/payments

GET /api/payments/{orderId}


#### Funcionalidades

- Creación de pagos
- Consulta de pagos
- Persistencia en PostgreSQL
- Relación pedido-pago
- Asociación por usuario

---

### 📦 Orders

#### Mejoras

- Historial de pedidos
- Detalle individual de pedidos
- Observaciones
- Cantidad de productos
- Validación de carrito vacío
- Generación automática de order_items

#### Nuevo Endpoint

http
PATCH /api/orders/{id}/status


---

### 🔄 Estados de Pedido

Se incorporó soporte para:


PendientePago

Pagado

EnPreparacion

Enviado

Entregado

Cancelado


#### DTOs

- UpdateOrderStatusDto

#### Repositorios

- UpdateStatusAsync()

#### Controladores

- PATCH /api/orders/{id}/status

---

### 🗄 Base de Datos

#### Nuevas Tablas

##### payments


id
order_id
user_id
monto
estado
transaction_id
fecha_creacion


#### Relación implementada


orders
 ↓
payments


---

### ✅ Validado en Postman

#### Checkout

http
GET /api/checkout/{orderId}


Validado:


✅ JWT

✅ Resumen Pedido

✅ Total

✅ CantidadItems

✅ PuedePagar


---

#### Payments

http
POST /api/payments

GET /api/payments/{orderId}


Validado:


✅ Persistencia PostgreSQL

✅ Asociación al pedido

✅ Asociación al usuario

✅ Respuesta DTO


---

#### Orders

http
PATCH /api/orders/{id}/status


Validado:


✅ Cambio de estado

✅ Seguridad JWT

✅ Validación de usuario propietario


---

### 🔐 Seguridad

Validaciones aplicadas en:


Cart

Orders

Checkout

Payments


Mediante:


JWT Authentication

User Ownership Validation

Protected Endpoints


---

### 📊 Estado del Proyecto

#### Implementado


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


#### Próxima versión


Wompi Integration

Payment Webhooks

Actualización automática de estados

Panel Administrativo

Dashboard de métricas


---

### 🎯 Hito

Se completa la primera versión funcional del backend ecommerce.


Backend Ecommerce Core

█████████████████████████░ 98%
