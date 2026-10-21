# Requisitos Punto de Venta Web — Adaptación a NorthWind

Proyecto: `D:\ESTUDIANTES_PRACTICA\ESTUDIANTES_PRACTICA`
Fuente: `C:\Users\User\Desktop\Requisitos_Punto_de_Venta_Web.txt`

Este archivo documenta **cómo se cumple cada requisito**, **cómo cambiar los
ajustes clave** y el **log de cambios** realizados durante el desarrollo.

---

## 1. Decisiones de diseño

| Decisión | Valor | Detalle |
|---|---|---|
| IVA | **15%** | Ecuador, Decreto Ejecutivo 198 (vigente desde 01/abr/2024) |
| Cédula | ~~Ecuador, 13 dígitos~~ **ELIMINADA (2026-10-21)** | Se retiró la validación módulo 11; `CustomerId` ahora solo acepta el código legacy de 5 caracteres |
| Concurrencia | **Optimista (RowVersion)** | Columna `RowVersion` en `Products` (tabla con updates: inventario) |
| Estrategia | **Por fases** | Fase 1 negocio/API → Fase 2 UI (modales) → Fase 3 factura PDF |
| Código | **Inglés** | Nombres de código en inglés; textos de usuario en recursos `.resx` (español) |

---

## 2. Instrucciones de cambio rápido

### 2.1 Cambiar el porcentaje de IVA

**Archivo único de verdad:** `NorthWind/2. Application Business Rules/NorthWind.Sales.Backend.BusinessObjects/ValueObjects/OrderTotals.cs`

```csharp
// IVA Ecuador: 15% desde 2024-04-01 (Decreto Ejecutivo 198).
public const decimal TaxRate = 0.15m;
```

Cambiar `0.15m` al nuevo porcentaje (ej. `0.12m`) y **recompilar**.

**También** cambiar el valor en el front (misma fórmula mostrada en el resumen):
`NorthWind/3. Interface Adapters/Views/NorthWind.Sales.Frontend.Views/ViewModels/CreateOrder/CreateOrderViewModel.cs`
→ propiedad `TaxRate` (debe coincidir con el backend).
Y el texto visible del impuesto:
`.../Frontend.Views/Resources/CreateOrderMessages.resx` → clave `TaxLabel`
(ej. `IVA (15%)` → `IVA (12%)`). Recompilar.

> El backend es la fuente de verdad (persiste `Subtotal`, `Tax`, `Total` en
> `Orders`); el front solo muestra el cálculo en vivo mientras se llena el
> formulario y recalcula el mismo resultado tras guardar.

### 2.2 Reglas de validación (R6)

- Regex de letras/números: `NorthWind/1. Enterprise Business Rules/NorthWind.Sales.Validators.Entities/CreateOrder/CreateOrderDtoValidator.cs`
- Mensajes personalizados: `.../Validators.Entities/Resources/CreateOrderMessages.resx`
  (agregar la clave en `.resx` **y** la propiedad en `CreateOrderMessages.Designer.cs`).
- ~~Cédula ecuatoriana: helper `.../CreateOrder/EcuadorianId.cs`~~
  **ELIMINADO (2026-10-21):** se retiró la validación de cédula (módulo 11);
  `CustomerId` ahora solo valida el código legacy de 5 caracteres.

### 2.3 Concurrencia optimista (RowVersion)

- Entidad: `Repositories/Entities/Product.cs` → propiedad `RowVersion`.
- Configuración EF: `DataContexts.EFCore/Configurations/ProductConfiguration.cs`
  → `builder.Property(p => p.RowVersion).IsRowVersion();`
- Si se quita la concurrencia: eliminar `IsRowVersion()` + la propiedad y
  **crear una nueva migración** (`dotnet ef migrations add ...`).

### 2.4 Migraciones de base de datos

Desde la carpeta de la solución (`NorthWind/`):

```bash
# Crear migración (requiere que compile)
dotnet ef migrations add <NombreMigracion> \
  --project "4. Frameworks And Drivers/Frameworks/DataContexts/NorthWind.Sales.Backend.DataContexts.EFCore" \
  --startup-project "4. Frameworks And Drivers/UI/NorthWind.Sales.WebApi" \
  --context NorthWindSalesContext

# Aplicar a la BD local (SQL Express)
dotnet ef database update \
  --project "4. Frameworks And Drivers/Frameworks/DataContexts/NorthWind.Sales.Backend.DataContexts.EFCore" \
  --startup-project "4. Frameworks And Drivers/UI/NorthWind.Sales.WebApi" \
  --context NorthWindSalesContext
```

> El `dotnet-ef` 10.0 ya está instalado en la máquina.
> Si EF no puede crear el contexto en tiempo de diseño, se usa el factory:
> `DataContexts.EFCore/DataContexts/NorthWindSalesContextDesignFactory.cs`.

### 2.5 SMTP / Mailtrap (implementación ya completa)

- Credenciales: `NorthWind.Sales.WebApi/appsettings.json` → sección `SmtpOptions`.
- Reiniciar el API tras cualquier cambio ahí.
- Disparador: orden con **más de 3 productos** (`SpecialOrderSpecification`
  usa `OrderDetails.Count > 3`). Un producto repetido **se fusiona** en una
  sola línea → no cuenta como productos distintos.

---

## 3. Estado de requisitos

| # | Requisito | Estado | Dónde |
|---|---|---|---|
| 1 | Modelo E-R normalizado | ✅ | Esquema NorthWind (Orders, OrderDetails, Products, Customers) |
| 2 | Crear orden maestro/detalle | ✅ | `CreateOrderInteractor` + `CreateOrderComponent` |
| 3 | Producto no duplicado en orden | ✅ | `OrderAggregate.AddDetail` fusiona repetidos |
| 4 | Subtotales, IVA y totales | ✅ | `OrderTotals` (IVA 15%), persistido en `Orders` |
| 5 | Editar solo campos necesarios | ✅ (fase 2 refuerzo) | POS: campos editables; buscador modal evita digitar |
| 6 | Validaciones (letras/números/blancos/correo/cédula) | ✅ (cédula retirada 2026-10-21) | `CreateOrderDtoValidator` (CustomerId: solo código de 5 caracteres) |
| 7 | Disminución automática de inventario | ✅ | `CommandsRepository.CreateOrder` descuenta stock |
| 8 | Mostrar solo productos con stock > 0 | ✅ | `QueriesRepository.SearchProducts` filtra `UnitsInStock > 0` + modal producto |
| 9 | No vender más que el stock | ✅ | `CreateOrderProductValidator` |
| 10 | Eliminar producto antes de guardar | ✅ | Botón Eliminar por fila (`RemoveOrderDetail`) en `CreateOrderComponent` |
| 11 | Cambiar producto sin perder datos | ✅ | Botón Cambiar por fila (`ReplaceOrderDetailProduct`) conserva cantidades y dirección |
| 12 | Factura encabezado/detalle + PDF | ✅ | `Pages/Invoice.razor` + `InvoicePdfGenerator` (QuestPDF) vía `GetInvoicePdf` |
| 13 | Usabilidad | ✅ (mejoras fase 2) | Diseño UI actual |
| 14 | Errores/mensajes personalizados | ✅ | Exception handlers + recursos `.resx` |
| 15 | Modales búsqueda inteligente + paginación | ✅ | `SearchModal.razor` genérico (5 por página, Anterior/Siguiente), cliente y producto |
| 16 | Buenas prácticas, TODO EN INGLÉS | ✅ | Código en inglés, textos en `.resx` |
| 17 | Transacciones + concurrencia + mensajes en front | ✅ | `IDomainTransaction` + `RowVersion` + `InformationMessage` |
| 18 | Sin CRUD de clientes/productos | ✅ | Datos manuales con SQL |
| 19 | N-Capas | ✅ | Clean Architecture (4 capas) |
| V1 | Ventana crear POS | ✅ | `Pages/CreateOrder.razor` |
| V2 | Modal búsqueda cliente | ✅ | `SearchModal<CustomerResultDto>` en `CreateOrderComponent` |
| V3 | Modal búsqueda producto | ✅ | `SearchModal<ProductResultDto>` en `CreateOrderComponent` |
| V4 | Búsqueda/visualización de factura | ✅ | `Pages/Invoices.razor` (por número o cliente) → `Pages/Invoice.razor` + PDF |

---

## 4. Log de cambios

### Fase 1 — Negocio y API (2026-10-16)

**Nuevo**
- `BusinessObjects/ValueObjects/OrderTotals.cs`: cálculo de subtotal, IVA
  (15%) y total. Constante `TaxRate` (ver §2.1).
- `Validators.Entities/CreateOrder/EcuadorianId.cs`: validador de cédula
  ecuatoriana (módulo 11, 13 dígitos).
- `Validation.Entities` + `ValidationService.FluentValidation`: regla
  `Matches(pattern, message)` para validaciones por expresión regular.

**Modificado**
- `POCOEntities/Order.cs`: propiedades `Subtotal`, `Tax`, `Total` (persistidas).
- `OrderAggregate.From(...)`: calcula y asigna los totales al crear.
- `CreateOrderDtoValidator`: 
  - `ShipCity` / `ShipCountry`: solo letras y espacios.
  - `ShipPostalCode`: obligatorio + solo números.
  - `CustomerId`: longitud 5 **o** cédula ecuatoriana válida de 13 dígitos.
- `Repositories/Entities/Product.cs`: `RowVersion` (byte[]) — concurrencia optimista.
- `ProductConfiguration`: `IsRowVersion()`.
- `OrderConfiguration`: precisión (10,2) para `Subtotal`, `Tax`, `Total`;
  `CustomerId` ampliado a 13 (permite cédulas) en `Orders` y `Customers`.
- `CommandsRepository.CreateOrder`: descuenta `UnitsInStock` por cada
  detalle en la misma transacción (R7).
- `INorthWindSalesCommandsDataContext` +
  `NorthWindSalesCommandsDataContext`: método `DecreaseProductStockAsync`.
- `CreateOrderMessages.resx` (Validators): nuevos mensajes de validación.
- Front: `CreateOrderViewModel` expone `Subtotal`, `TaxAmount`, `Total`
  (resumen en vivo); mensaje de transacción completada/fallida;
  `CreateOrderComponent.razor` muestra el resumen de totales.

**Migración EF**
- `20261016234443_AddOrderTotalsAndProductRowVersion` — **APLICADA** a
  `NorthWindDB` (2026-10-16):
  - `Products.RowVersion` (rowversion) — concurrencia optimista.
  - `Orders.Subtotal/Tax/Total` decimal(10,2).
  - `Orders.CustomerId` y `Customers.Id`: `nchar(5)` → `nvarchar(13)`
    (permite cédulas de 13 dígitos).
  - Nota técnica: SQL Server exige dropear `FK_Orders_Customers_CustomerId`
    y `PK_Customers` antes de alterar esas columnas; la migración los
    dropea y recrea en orden (ver el archivo de migración).
- Factory de diseño nuevo: `DataContexts/NorthWindSalesContextFactory.cs`
  (necesario para que `dotnet ef` pueda crear el contexto — ver §2.4).

**Mensajes (R14/R17)**
- Front: `TransactionCompletedMessage` / `TransactionFailedMessage`
  (resumen de éxito/fracaso de la transacción en la página de POS).
- Validadores: mensajes de ciudad/país solo letras, código postal
  obligatorio y solo números, formato de identificador.

**Verificación Fase 1**
- `dotnet build NorthWind.sln` → 0 errores, 0 advertencias.
- Migración aplicada sin pérdida de datos.

---

### Fase 2 — Búsqueda inteligente y acciones del POS (2026-10-19)

**Nuevo (backend)**
- `Entities/Dtos/Search/`: `PagedResultDto<T>` (Items, Page, PageSize,
  TotalCount, TotalPages), `CustomerResultDto`, `ProductResultDto`.
- `INorthWindSalesQueriesDataContext` + impl: `CountAsync` para paginación.
- `IQueriesRepository` + `QueriesRepository`:
  `SearchCustomers` y `SearchProducts` — búsqueda por Id o Name
  (`OrderBy(Name)`, `Skip/Take`, clamp de página 1..50); productos
  **filtran `UnitsInStock > 0` (R8)**.
- InputPorts: `ISearchCustomersInputPort`, `ISearchProductsInputPort`.
- Interactors: `SearchCustomersInteractor`, `SearchProductsInteractor`
  (con `GuardUser.AgainstUnauthenticated`).
- Controllers: `Search/SearchCustomersController`,
  `Search/SearchProductsController` (GET con `search`, `page`,
  `pageSize` default 5, `RequireAuthorization`).
- `Endpoints.SearchCustomers` / `Endpoints.SearchProducts`.

**Nuevo (frontend)**
- `Frontend.BusinessObjects/Interfaces/`: `ISearchCustomersGateway`,
  `ISearchProductsGateway`.
- `WebApiGateways/SearchCustomersGateway.cs`,
  `SearchProductsGateway.cs` (query string escapado con
  `Uri.EscapeDataString`; errores vía `ExceptionDelegatingHandler`).
- `Components/SearchModal.razor` (+ `.razor.cs`): modal genérico
  `@typeparam TItem` con overlay propio (sin JS de Bootstrap),
  buscador (Enter o botón), tabla con selección por fila, vacío sin
  resultados y paginación Anterior/Siguiente (5 por página, R15).
- 22 claves nuevas en `CreateOrderMessages.resx` + Designer.

**Modificado**
- `DependencyContainer` (UseCases): registra los dos nuevos InputPorts.
- `EndpointsContainer` (IoC): mapea los dos nuevos controllers.
- GlobalUsings: UseCases (`...UseCases.Queries`), Controllers (Dtos.Search,
  Interfaces.Queries), WebApiGateways, Frontend.BusinessObjects, Views.
- `CreateOrderViewModel`: inyecta los dos gateways de búsqueda;
  `SelectCustomer` (R5), `AddProductFromSearch` (R8 + anti-duplicado),
  `ReplaceOrderDetailProduct` (R11), `RemoveOrderDetail` (R10);
  propiedad display `CustomerName`.
- `CreateOrderComponent` (+ `.cs`): botón **Buscar cliente** junto al Id,
  botón **Buscar producto**, columna **Acciones** con
  **Cambiar**/**Eliminar** por fila; referencias a ambos modales;
  modo de reemplazo por índice (`ProductChangeIndex`).
- `WebApiGateways/DependencyContainer`: registra los gateways de búsqueda.
- `wwwroot/css/app.css`: estilos del modal (overlay, tabla, footer de
  paginación, marca de selección) y acciones de fila.

**Verificación Fase 2**
- `dotnet build NorthWind.sln` → 0 errores, 0 advertencias.
- Fase 1 intacta (totales, IVA, stock, validaciones, concurrencia).

---

### Fase 3 — Factura, PDF y búsqueda de facturas (2026-10-19)

**Nuevo (backend)**
- `Entities/Dtos/Invoice/`: `InvoiceDto` (encabezado + detalle + totales),
  `InvoiceDetailDto` (con `LineTotal` calculado), `InvoiceSummaryDto`
  (resultado de búsqueda).
- `INorthWindSalesQueriesDataContext` + impl EFCore:
  `Orders` y `OrderDetails` como `IQueryable` (NOTracking).
- `IQueriesRepository` + `QueriesRepository`:
  - `GetInvoice(orderId)` — reconstruye factura (pedido + cliente +
    detalle con nombre de producto); `null` si no existe (R12).
  - `SearchInvoices(term, page, pageSize)` — por número de factura o
    cliente (Id/Nombre), orden descendente, paginado (V4).
- InputPorts `Interfaces/Invoice/`: `IGetInvoiceInputPort`,
  `ISearchInvoicesInputPort`; interactors con autenticación.
- Controllers `Invoice/`:
  - `GetInvoiceController` (GET, 404 si no existe),
  - `SearchInvoicesController` (5 por página),
  - `GetInvoicePdfController` (devuelve `application/pdf`,
    nombre `Invoice-{id}.pdf`).
  - `InvoicePdfGenerator` (QuestPDF **2026.9.1**, licencia Community):
    encabezado, datos de cliente/envío, tabla de detalle con filas
    alternas, totales con IVA 15% y pie con paginación. Textos desde
    `Resources/InvoicePdfMessages.resx` (R16).
- `Endpoints`: `GetInvoice`, `SearchInvoices`, `GetInvoicePdf`.
- Paquete `System.Security.Cryptography.Pkcs 10.0.13` (referencia
  directa para evitar NU1902/NU1903 de la transitiva 9.0.11).

**Nuevo (frontend)**
- `IInvoiceGateway` + `InvoiceGateway` (GET detalle con 404 → `null`,
  búsqueda paginada, bytes del PDF).
- `ViewModels/Invoice/`: `SearchInvoicesViewModel` (búsqueda + paginación),
  `InvoiceViewModel` (carga de factura + PDF con mensajes de error).
- `Pages/Invoices.razor` (+`.cs`): ventana de búsqueda inteligente
  `/invoices` — por número o cliente, paginación, botón Ver (V4).
- `Pages/Invoice.razor` (+`.cs`): `/invoice/{id}` — encabezado y detalle
  de la factura guardada, totales, botones **Ver PDF** (pestaña nueva),
  **Descargar PDF** y **Volver a la búsqueda** (R12).
- `InvoiceMessages.resx` + Designer (29 claves ES).
- `index.html`: JS `northwind.openPdf` / `northwind.downloadPdf`
  (Blob de los bytes recibidos).
- `MainLayout`: ítem de menú **Facturas** (enlace a `/invoices`, V4).
- `wwwroot/css/app.css`: estilos de búsqueda, tarjeta de factura,
  tabla de detalle, totales y acciones.

**Modificado (corrección importante)**
- `WebApiGateways/DependencyContainer`: **el handler de bearer token se
  aplica ahora a TODOS los gateways** (antes solo a `CreateOrderGateway`;
  sin esto los endpoints de búsqueda/factura devolvían 401 en el front).

**Verificación Fase 3**
- `dotnet build NorthWind.sln` → 0 errores, 0 advertencias.
- Smoke test del PDF: estructura válida (`%PDF-`, `%%EOF`, `xref`,
  fuentes Lato incrustadas por subconjunto).
- Fases 1 y 2 intactas.

---

### Prueba de ejecución (2026-10-19)

**Ambiente**
- SQL Server `localhost\SQLEXPRESS`: `NorthWindDB`, `NorthWindUsersDB`,
  `NorthWindLogsDB` ONLINE.
- WebApi `http://localhost:5103`, Blazor WASM `http://localhost:5249`
  (ambos `dotnet run`, arrancados y probados en vivo).

**Hallazgos corregidos durante la prueba**
1. **Migración aplicada a la BD equivocada**: la factory de diseño
   `NorthWindSalesContextFactory` apuntaba a `(localdb)\mssqllocaldb`
   mientras la app usa `SQLEXPRESS`; por eso `NorthWindDB` (SQLEXPRESS)
   estaba sin `Subtotal/Tax/Total`, sin `RowVersion` y con
   `CustomerId nchar(5)`. **Corregido**: factory alineada a la cadena de
   `appsettings.json` y migración
   `20261016234443_AddOrderTotalsAndProductRowVersion` **aplicada**
   (FK/PK de Customers recreados, filas existentes conservadas).
2. **Modal (fase 2)**: el clic en el overlay **ya no cierra** la ventana
   emergente — solo se cierra con la ✕ o al seleccionar una fila
   (`SearchModal.razor`).

**Resultados (curl, contra la API viva)**
| Prueba | Resultado |
|---|---|
| Endpoints protegidos sin token | 401 en SearchCustomers/Products/Invoices, GetInvoice, GetInvoicePdf |
| Registro `POST /user/Register` | 200 |
| Login `POST /user/Login` | 200 + JWT |
| SearchCustomers paginado | 200 — 5 clientes, `totalPages` correcto |
| SearchProducts (`c`, stock>0) | 200 — solo productos con stock |
| SearchInvoices por número (1) | 200 — 10 resultados, paginación 4 págs |
| SearchInvoices por cliente (ANATR) | 200 — 5 resultados |
| GetInvoice 18 | 200 — encabezado + detalle con nombres de producto |
| GetInvoice 9999 | 404 |
| GetInvoicePdf 18/19 | 200 — `application/pdf`, `Invoice-{id}.pdf`, `%PDF-1.4`…`%%EOF` (≈38 KB) |
| CreateOrder (2× Chai $35) | 200 → orden **19**; stock Chai 20→**18**; Subtotal **70.00**, IVA **10.50**, Total **80.50** |
| Blazor: index, `blazor.webassembly.js`, `css/app.css` | 200 |
| Logs WebApi/Blazor | sin errores ni excepciones |

- `dotnet build NorthWind.sln` → 0 errores, 0 advertencias (tras todo).

---

### Ajustes de UI solicitados en el POS (2026-10-19)

1. **Confirmación al eliminar producto (R10):** nuevo componente
   `ConfirmModal` (overlay **no** cierra con clic fuera; solo Sí/No).
   Al pulsar *Eliminar* se abre: *"¿Está seguro de eliminar el producto?"*
   con botones **Sí** / **No**; *Sí* ejecuta `RemoveOrderDetail`,
   *No* cancela. Mensajes en `CreateOrderMessages` (5 claves nuevas:
   `ConfirmRemoveTitle/Message`, `ConfirmYesButtonText`,
   `ConfirmNoButtonText`, `DuplicateProductsMessage`).
2. **Precio bloqueado:** el `InputNumber` de Precio lleva `disabled`
   (clase `.price-fixed`, estilo visible con texto oscuro) — no editable,
   solo visual; cantidad y Id siguen editables.
3. **Sin duplicados al crear la orden:** `CreateOrderViewModel.Send()`
   valida por **Id de producto** (agrupa `OrderDetails`, excluye Id 0) y
   aborta con mensaje listando los Ids duplicados. Las rutas del modal
   ya validaban (`AddProductFromSearch` / `ReplaceOrderDetailProduct`).
4. **Búsqueda en vivo en los modales (R15):** `SearchModal` ahora busca
   **automáticamente al escribir** (debounce 300 ms + control de versiones
   para descartar resultados obsoletos) y **al abrir muestra todos** los
   registros (resetea el término). El botón *Buscar* y Enter siguen
   disponibles. El backend ya filtraba por Id o Nombre.
   (Sin cambios de backend; solo front: `SearchModal`,
   `CreateOrderComponent`, `CreateOrderViewModel`, resx, `app.css`.)
5. **Precio automático al teclear el Id (manual):** el `InputNumber` de Id
   tiene `@oninput` (+ respaldo `@bind-Value:after` al confirmar el valor)
   → llama a `SearchProducts` con el Id exacto y llena `UnitPrice`
   (0 si no existe el producto). Control de versiones para descartar
   respuestas viejas al teclear rápido. Sin cambios de backend.

**Correcciones tras prueba automatizada (Playwright + Edge headless):**
- **`ConfirmModal` no aparecía** pese a estar cableado: `Open()` corre en el
  contexto del padre y el componente **no tiene RenderFragments**, por lo que
  el diff del padre lo omitía (parámetros "sin cambios") y `IsOpen=true`
  nunca se pintaba. **Fix:** `Open()` llama `StateHasChanged()` explícito
  (mismo patrón que `SearchModal.Open()`).
- Verificación automatizada final: modal visible con mensaje correcto,
  *No* conserva la fila, *Sí* la elimina; precio `65.00` al teclear `3`
  (1 llamada a `SearchProducts`); **0 errores de consola**.
  `dotnet build NorthWind.sln` → 0 errores, 0 advertencias.

- Verificado: `dotnet build NorthWind.sln` → 0 errores, 0 advertencias;
  WebApi `:5103` y Blazor `:5249` relanzados con los cambios, logs sin errores.

### Mejora de interfaz: estructura, comodidad y usabilidad (2026-10-19)

**Design system (`wwwroot/css/app.css`)**
- Variables CSS (`--nw-brand`, `--nw-bg`, `--nw-border`, `--nw-radius`,
  sombras, foco) y base renovada: fondo gris claro, tipografía 15.5 px,
  botones con radio 8 px y estados hover/active/focus-visibles,
  controles de 42 px, labels en negrita media, tablas con cabeceras
  en mayúsculas y filas alternadas/hover, alerts con barra de acento.
- Paneles (`.panel` con cabecera y barra de acento), `.page-header`,
  estados vacíos con borde punteado, `.nw-spinner` de carga.
- Animaciones de entrada para modales (`fade`/`pop`) respetando
  `prefers-reduced-motion`; utilidades responsive (≤900 px y ≤640 px).

**Layout**
- Sidebar con marca (cuadro "N" + nombre), menú con indicador activo
  (barra lateral + fondo), mejor padding y scrollbar; top-bar sticky
  con borde/sombra sutil; contenido centrado (máx. 1160 px) con
  padding cómodo; header con **chip de usuario** (punto verde + nombre)
  y CTA "Iniciar sesión" como botón para anónimos.

**Páginas**
- **Home:** dashboard de bienvenida (h1 + subtítulo) con tarjetas
  "Crear orden" / "Facturas" (icono, descripción, "Ir →") y aviso de
  login para anónimos. Reemplaza el "Hello, world!".
- **Crear orden:** h1 de página; paneles *Cliente y envío* (grid de
  campos, 2 columnas) y *Detalle de la orden* (cabecera en mayúsculas,
  filas como tarjetas, **estado vacío** con guía); footer con acciones
  a la izquierda, resumen (Subtotal/IVA/Total) y botón **Enviar**
  destacado; alertas al final del formulario.
- **Facturas:** h1; buscador + tabla + paginación dentro de un panel;
  spinner de carga; badges `#N` para el número de factura; totales en
  negrita.
- **Factura:** toolbar *← Volver a la búsqueda* arriba; badge de
  número de factura; bloques Cliente/Enviar a con estilo de tarjeta.
- **Login / Registro:** tarjeta centrada con marca "NorthWind Sales",
  título h1, CTA ancho (44 px) y enlace al pie (Crear cuenta ↔ Iniciar
  sesión). NotFound traducido con retorno al inicio.

**Modales**
- `SearchModal`: foco automático en el input al abrir.
- `ConfirmModal`: foco por defecto en **No** (opción segura),
  ícono de advertencia en el título.

**Recursos (resx + Designer, español)**
- `CreateOrderMessages`: `ShippingSectionTitle`, `OrderDetailSectionTitle`,
  `OrderEmptyHintText`.
- `CommonMessages`: `WelcomeHeading`, `WelcomeSubtitle`,
  `WelcomeNewOrderText`, `WelcomeInvoicesText`, `LoginRequiredHint`,
  `ExploreOptionsText`, `NotFoundTitle`, `NotFoundText`.

**Aviso de empaquetado (importante)**
- `NorthWind.Membership.Frontend.RazorViews` se consume como **paquete
  NuGet local** (no como `ProjectReference`). Tras editar sus fuentes:
  `dotnet pack … -o NorthWind.Packages` → borrar
  `~/.nuget/packages/northwind.membership.frontend.razorviews/1.0.0` →
  `dotnet build`. Sin este paso los cambios de Login/Registro/Header no
  se reflejan (seguiría corriendo el paquete anterior).

**Verificación**
- `dotnet build NorthWind.sln` → **0 errores, 0 advertencias**.
- Tests Playwright (`test.js`, `test-typing.js`): login OK, precio
  `65.00` al teclear Id, modal Sí/No correcto, **0 errores de consola**.
- 17 capturas (desktop + móvil + menú) sin errores de runtime.

**Observación de datos (no es UI):** 17 órdenes anteriores a la Fase 1
tienen `Subtotal/Tax/Total` en 0 en la BD; la factura 18 muestra
totales $0.00 pese a tener ítems (el PDF usa los mismos valores
guardados, así que es consistente). Backfill opcional pendiente de
decisión.

### Auto-carga de la lista de facturas (2026-10-19)

- `Invoices.razor.cs`: nuevo `OnInitializedAsync` → si el ViewModel
  aún no tiene resultados (`Result is null`) ejecuta `SearchAsync()`.
  Al entrar a `/invoices` **ya se muestran las facturas sin pulsar
  Buscar**; al volver desde el detalle se conserva la lista y el
  término de búsqueda (sin recargar). Búsqueda manual/Enter/paginación
  siguen igual.
- Verificado con Playwright: tabla visible sin clic (5 filas, Página 1
  de 7), vuelva desde detalle conserva la lista, **0 errores de
  consola**; `dotnet build` → 0 errores.

### Limpieza de datos y retiro de validación de cédula (2026-10-21)

**1) Borrado de órdenes con total en 0 (legado pre-Fase 1):**
- Eliminadas de `NorthWindDB` **17 órdenes** (`Subtotal=0 AND Tax=0
  AND Total=0`, Ids 1–12 y 14–18) y sus **54 líneas** de `OrderDetails`
  (transacción SQL; antes 31 órdenes → quedan **14** con totales
  reales). No afecta Customers/Products ni las BDs de usuarios/logs.
- Verificado tras el borrado: `SearchInvoices` → `totalCount: 14`,
  3 páginas.

**2) Eliminación total de la validación de cédula (dígito verificador):**
- Borrado `Validators.Entities/CreateOrder/EcuadorianId.cs` (algoritmo
  módulo 11, 13 dígitos).
- `CreateOrderDtoValidator`: `CustomerId` ahora **solo** valida el
  código legacy de 5 caracteres (se quitó la rama de cédula).
- Mensaje `CustomerIdInvalidFormat` (resx) reescrito: *"El identificador
  debe ser un código de 5 caracteres."* (ya no menciona cédula).
- Verificado por API: cédula de 13 dígitos → **400** con el nuevo
  mensaje; `ALFKI` → **200** (orden creada). `dotnet build` → **0
  errores**. Sin referencias restantes a `EcuadorianId`/cédula en
  código.

**3) Notificación de órdenes "especiales" (4+ productos) — confirmada:**
- Existe y está cableada: `SpecialOrderSpecification`
  (`OrderDetails.Count > 3`) → `CreateOrderInteractor` lanza
  `SpecialOrderCreatedEvent` → `SendEMailWhenSpecialOrderCreatedEventHandler`
  → `MailService.SendMailToAdministratorAsync` (asunto + cuerpo con
  OrderId y cantidad de productos, destino `AdministratorEmail`) vía
  SMTP **Mailtrap** (`sandbox.smtp.mailtrap.io:2525`, `SmtpOptions` en
  `appsettings.json`).

---
