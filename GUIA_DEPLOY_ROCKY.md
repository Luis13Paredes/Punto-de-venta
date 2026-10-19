# Guía de despliegue NorthWind POS en una VM con Rocky Linux 8 (minimal)

> **Contenido de esta guía:**
> - **PARTE 1** (de la sección "Arquitectura final" hasta "Comandos útiles"): solo
>   el **backend** (API + SQL Server) en la VM; el Blazor se ejecuta en Windows.
> - **PARTE 2** (al final del archivo): **sitio completo** montado con un web
>   server (**Nginx**) en la VM, sirviendo también la interfaz Blazor.
>
> Si es tu primera vez, haz primero la PARTE 1 completa y verifica que todo
> funciona; la PARTE 2 solo agrega Nginx + el front publicado.

> **Nota sobre la distro:** esta guía apunta a **Rocky Linux 8 en versión
> minimal** (compatible con RHEL 8). El nombre del archivo conserva "DEBIAN"
> por historial, pero todos los comandos son de Rocky/RHEL (`dnf`, `firewalld`,
> SELinux, `conf.d`). Si necesitas la versión Debian, los cambios son: `apt`
> en lugar de `dnf`, `ufw` en lugar de `firewalld`, no SELinux, Nginx usa
> `sites-available/sites-enabled` y los estáticos van a `www-data`.

---

## Qué cambia respecto a Debian (resumen)

| Área | Debian | Rocky Linux 8 |
|---|---|---|
| Gestor de paquetes | `apt install` | `dnf install` |
| Docker oficial | `docker.io` (repo del sistema) | **Docker CE** (repo `download.docker.com/linux/centos`) |
| Firewall | `ufw` | **`firewalld`** (`firewall-cmd`) |
| SELinux | no existe | **Activo (enforcing)** → permisos para Nginx (`semanage`/`restorecon`/`setsebool`) |
| Config de Nginx | `/etc/nginx/sites-available` + symlink | **`/etc/nginx/conf.d/northwind.conf`** (y comentar el `server` por defecto de `nginx.conf`) |
| MIME de `.wasm` | bloque `types` en server | **`default_type`** en un `location` (no altera los demás tipos) |
| Dueño de estáticos | `www-data` | **`nginx`** |
| .NET runtime (opcional) | repo `packages.microsoft.com/config/debian/12` | repo **`config/rhel/8`** (aun así se recomienda *self-contained*) |
| sqlcmd en el host | repo Debian | repo **`config/rhel/8`** (o usarlo dentro del contenedor, como en esta guía) |
| Certbot | repo del sistema | **EPEL** (`epel-release`) |

Lo que **no** cambia: SQL Server en Docker (agnóstico a la distro), la
publicación `dotnet publish -r linux-x64 --self-contained` (glibc de Rocky 8
= 2.28 ≥ mínimo requerido), connection strings con SQL auth, systemd, JWT y
las pruebas desde Windows.

---

## PARTE 1 — Backend (API + SQL Server) en la VM

> **Objetivo:** ejecutar la API (`NorthWind.Sales.WebApi`, .NET 10) dentro de
> una máquina virtual Rocky Linux 8, con SQL Server 2022 en Docker,
> conservando los datos actuales (órdenes, usuarios, logs).
>
> **Datos verificados del proyecto:**
> - TFM: `net10.0` (Kestrel), publicación **self-contained linux-x64**.
> - CORS ya configurado con `AllowAnyOrigin` (`Startup.cs`).
> - Swagger solo se expone con `ASPNETCORE_ENVIRONMENT=Development`.
> - 3 connection strings usan **autenticación Windows** → **deben cambiarse a
>   SQL auth en Linux**.
> - 3 contextos EF: `NorthWindSalesContext` → `NorthWindDB`,
>   `NorthWindDomainLogsContext` → `NorthWindLogsDB`,
>   `NorthWindMembershipContext` → `NorthWindUsersDB`.
> - La app **no crea ni migra las BDs** al arrancar: deben existir antes.

---

## Arquitectura final

```
┌─────────────────────┐         http://IP-VM:5103        ┌──────────────────────────┐
│  PC Windows (host)  │ ───────────────────────────────► │   VM Rocky Linux 8       │
│  - Blazor WASM      │                                  │  - API .NET 10 (:5103)   │
│    (:5249)          │ ◄─────────────────────────────── │  - SQL Server 2022       │
│  - SSMS (opcional)  │         JSON + JWT               │    Docker (:1433)        │
└─────────────────────┘                                  └──────────────────────────┘
```

**Requisitos de la VM:** Rocky Linux 8.x **minimal** (solo terminal), **2–4 GB
RAM** (SQL Server necesita mínimo ~2 GB), 20 GB de disco libre, red en modo
**Bridged / Host-Only / NAT+redirección** (ver Paso 0).

---

## Paso 0 — Preparar la VM (Rocky Linux 8 minimal)

1. Crear la VM con la ISO **Rocky-8-x.x-x86_64-minimal.iso** (durante la
   instalación crearás el usuario y podrías activar la red en arranque).
2. **Red** (elegir una opción):

   | Opción | Configuración | Resultado |
   |---|---|---|
   | **A (recomendada)** Bridged Adapter | Adaptador → Bridged → tu tarjeta física | La VM obtiene IP de tu router (`192.168.x.x`), accesible desde cualquier equipo |
   | **B** Host-Only | Adaptador → Solo anfitrión (`vboxnet0`) | Red privada host↔VM, sin internet en la VM |
   | **C** NAT + redirección | Preferencias → Red → NAT → Avanzado → **Redirección de puertos**: Nombre `api`, TCP, IP anfitrís `127.0.0.1`, Puerto anfitr. `5103`, Puerto invit. `5103` | API en `http://localhost:5103` del host; la VM necesita NAT para internet |

   > Con la opción C agrega también una redirección `sql`: TCP,
   > `127.0.0.1:1433` → `1433` (para restaurar backups desde SSMS).

3. Obtener la IP dentro de la VM:
   ```bash
   ip a
   hostname -I
   ```
4. Probar conectividad desde **Windows** (PowerShell/CMD): `ping IP-VM`.

### 0.1 — Preparar el sistema (minimal)

El ISO minimal no trae utilidades habituales; instalarlas (y `libicu`, que
.NET **necesita** para globalización):

```bash
sudo dnf update -y
sudo dnf install -y nano wget curl rsync tar libicu
sudo systemctl enable --now sshd        # para scp desde Windows
```

> **SELinux** viene **activo (enforcing)** en Rocky 8: es normal y se deja
> así; los pasos de Nginx (PARTE 2) incluyen los permisos necesarios.

---

## Paso 1 — Backup de las 3 bases de datos (en Windows, SSMS)

*(Idéntico a cualquier distro: es un paso de Windows.)*

1. Abrir **SQL Server Management Studio** → conectar a `localhost\SQLEXPRESS`
   (autenticación integrada).
2. Para **cada** base de datos (`NorthWindDB`, `NorthWindLogsDB`,
   `NorthWindUsersDB`):
   - Clic derecho → **Tasks** → **Back Up...**
   - Backup type: **Full** → **Add** → carpeta `C:\TempBackups\` con nombre
     `<BD>.bak` → OK hasta *"completed successfully"*.
3. Verificar los 3 archivos `.bak` en `C:\TempBackups\`.

---

## Paso 2 — Publicar la API para Linux (en Windows)

1. Abrir terminal en la raíz del repo (`...\NorthWind`):
   ```bash
   dotnet publish "4. Frameworks And Drivers/UI/NorthWind.Sales.WebApi/NorthWind.Sales.WebApi.csproj" \
     -c Release -r linux-x64 --self-contained true -o publish/linux
   ```
   - `--self-contained true` → la VM **no necesita .NET instalado** (todo es
     managed: EF, SqlClient, QuestPDF) y funciona igual en Debian o Rocky.
   - **Alternativa ligera:** omitir `-r`/`--self-contained` e instalar el
     runtime en la VM (Paso 3b); si el paquete no existe para RHEL 8,
     quédate con *self-contained*.
2. Verificar la salida: `publish/linux` debe contener el ejecutable
   `NorthWind.Sales.WebApi`, `*.dll`, `appsettings.json`, `wwwroot/`.
3. Copiar a la VM:
   ```bash
   scp -r publish/linux usuario@IP-VM:/tmp/api-publish
   ```
   (O carpeta compartida de VirtualBox: Máquina → Configuración → Carpetas
   compartidas → montar con `sudo mount -t vboxsf nombre /mnt/shared`.)
4. Colocarla en destino (en la VM):
   ```bash
   sudo mkdir -p /opt/northwind-api
   sudo cp -r /tmp/api-publish/* /opt/northwind-api/
   sudo chown -R $USER:$USER /opt/northwind-api
   chmod +x /opt/northwind-api/NorthWind.Sales.WebApi
   ```

---

## Paso 3 — SQL Server 2022 en Docker (Rocky 8)

### 3a. Instalar Docker CE y arrancar SQL Server

Rocky 8 **no** trae Docker en sus repos por defecto (viene Podman como
alternativa); se usa el repo oficial de Docker CE (compatible con RHEL/Rocky 8):

```bash
sudo dnf install -y dnf-plugins-core
sudo dnf config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo
sudo dnf install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
sudo systemctl enable --now docker
sudo docker --version

sudo docker volume create sqldata   # persistencia entre reinicios

sudo docker run -d \
  --name northwind-sql \
  --restart unless-stopped \
  -e ACCEPT_EULA=Y \
  -e MSSQL_SA_PASSWORD='Northwind2026!' \
  -p 1433:1433 \
  -v sqldata:/var/opt/mssql \
  mcr.microsoft.com/mssql/server:2022-latest
```
> La contraseña de `sa` debe cumplir política SQL (mayúscula + minúscula +
> número + carácter especial): `Northwind2026!`. **Anotarla.**
> La imagen oficial de SQL Server es multi-distro: en Rocky 8 corre igual.

**Verificación** (20–60 s la primera vez):
```bash
sudo docker logs -f northwind-sql
# Esperar: "Ready for client connections on TCP port 1433"  (Ctrl+C para salir)
sudo docker ps    # northwind-sql en STATUS "Up ..."
```

### 3b. Solo si publicaste framework-dependent: .NET 10 Runtime

```bash
sudo rpm -Uvh https://packages.microsoft.com/config/rhel/8/packages-microsoft-prod.rpm
sudo dnf install -y aspnetcore-runtime-10.0
dotnet --list-runtimes | grep "Microsoft.AspNetCore.App 10"
```
> Si el paquete `10.0` no está disponible para RHEL 8 en tu momento, usa la
> publicación **self-contained** del Paso 2 (no requiere nada aquí).

### 3c. `sqlcmd` (opcional; la guía usa el del contenedor)

La imagen de SQL 2022 incluye las herramientas dentro del contenedor
(`/opt/mssql-tools18/bin/sqlcmd`), que es lo que se usa en esta guía. Para
instalarlo en el host (opcional):

```bash
sudo curl -o /etc/yum.repos.d/mssql-release.repo https://packages.microsoft.com/config/rhel/8/prod.repo
sudo ACCEPT_EULA=Y dnf install -y mssql-tools18
# PATH: /opt/mssql-tools18/bin/sqlcmd
```

---

## Paso 4 — Restaurar las 3 bases de datos

### Opción A (recomendada): restaurar los backups (conserva datos)

**A1. Copiar los `.bak` al contenedor** (desde la VM; súbelos primero a la
VM por scp/carpeta compartida):
```bash
sudo docker cp /tmp/NorthWindDB.bak      northwind-sql:/var/opt/mssql/data/
sudo docker cp /tmp/NorthWindLogsDB.bak  northwind-sql:/var/opt/mssql/data/
sudo docker cp /tmp/NorthWindUsersDB.bak northwind-sql:/var/opt/mssql/data/
```

**A2. Restaurar** desde SSMS en Windows conectado a `IP-VM,1433` (`sa`), o
con `sqlcmd` dentro del contenedor. Para cada BD:

```sql
-- 1) Nombres lógicos del archivo:
RESTORE FILELISTONLY FROM DISK = '/var/opt/mssql/data/NorthWindDB.bak';

-- 2) Restaurar (rutas Linux, no Windows):
RESTORE DATABASE NorthWindDB FROM DISK = '/var/opt/mssql/data/NorthWindDB.bak'
WITH REPLACE,
  MOVE 'NorthWindDB'     TO '/var/opt/mssql/data/NorthWindDB.mdf',
  MOVE 'NorthWindDB_log' TO '/var/opt/mssql/data/NorthWindDB_log.ldf';
```
Repetir con `NorthWindLogsDB` y `NorthWindUsersDB` usando **sus** nombres
lógicos.

**A3. Verificar:**
```sql
SELECT COUNT(*) FROM NorthWindDB.dbo.Orders;           -- 18+ (incluye la 19)
SELECT COUNT(*) FROM NorthWindUsersDB.dbo.AspNetUsers;  -- 8 (incluye pos.test)
SELECT TABLE_NAME FROM NorthWindLogsDB.INFORMATION_SCHEMA.TABLES;
```

### Opción B: instalación limpia vía scripts EF (sin datos)

**B1. Generar los scripts en Windows** (no conectan a la BD):
```bash
dotnet ef migrations script --idempotent \
  --context NorthWindSalesContext \
  --project "4. Frameworks And Drivers/Frameworks/DataContexts/NorthWind.Sales.Backend.DataContexts.EFCore/NorthWind.Sales.Backend.DataContexts.EFCore.csproj" \
  --startup-project "4. Frameworks And Drivers/UI/NorthWind.Sales.WebApi/NorthWind.Sales.WebApi.csproj" \
  -o sql/NorthWindDB.sql

dotnet ef migrations script --idempotent \
  --context NorthWindDomainLogsContext \
  --project "4. Frameworks And Drivers/Frameworks/DataContexts/NorthWind.Sales.Backend.DataContexts.EFCore/NorthWind.Sales.Backend.DataContexts.EFCore.csproj" \
  --startup-project "4. Frameworks And Drivers/UI/NorthWind.Sales.WebApi/NorthWind.Sales.WebApi.csproj" \
  -o sql/NorthWindLogsDB.sql

dotnet ef migrations script --idempotent \
  --context NorthWindMembershipContext \
  --project "NorthWind.Membership/NorthWind.Membership.Backend.AspNetIdentity/NorthWind.Membership.Backend.AspNetIdentity.csproj" \
  --startup-project "4. Frameworks And Drivers/UI/NorthWind.Sales.WebApi/NorthWind.Sales.WebApi.csproj" \
  -o sql/NorthWindUsersDB.sql
```

**B2. Crear las BDs vacías y aplicar los scripts:**
```sql
CREATE DATABASE NorthWindDB;
CREATE DATABASE NorthWindLogsDB;
CREATE DATABASE NorthWindUsersDB;
GO
-- Luego: sqlcmd -S localhost -U sa -P 'Northwind2026!' -d NorthWindDB -i NorthWindDB.sql -C
```
(El script idempotente trae `IF NOT EXISTS` pero **no** crea la BD.)

**B3.** Registrar un usuario inicial (`POST /user/Register` o desde el Blazor).

---

## Paso 5 — Configurar `appsettings.json` en la VM

Editar `/opt/northwind-api/appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "DBOptions": {
    "ConnectionString": "Server=localhost,1433;Database=NorthWindDB;User Id=sa;Password=Northwind2026!;TrustServerCertificate=True;Encrypt=True;",
    "DomainLogsConnectionString": "Server=localhost,1433;Database=NorthWindLogsDB;User Id=sa;Password=Northwind2026!;TrustServerCertificate=True;Encrypt=True;"
  },
  "SmtpOptions": {
    "SmtpHost": "sandbox.smtp.mailtrap.io",
    "SmtpPort": 2525,
    "SmtpHostPort": 2525,
    "SmtpUserName": "22b172130b5464",
    "SmtpPassword": "aad9d17c6d4e2b",
    "SenderEmail": "no-reply@northwind.com",
    "AdministratorEmail": "admin@northwind.com"
  },
  "MembershipDBOptions": {
    "ConnectionString": "Server=localhost,1433;Database=NorthWindUsersDB;User Id=sa;Password=Northwind2026!;TrustServerCertificate=True;Encrypt=True;"
  },
  "JwtOptions": {
    "SecurityKey": "KeyMustBeAtLeast32CharactersLong",
    "ValidIssuer": "NorthWindSales",
    "ValidAudience": "NorthWindSalesClient",
    "ExpireInMinutes": 1440,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ValidateIssuerSigningKey": true
  }
}
```

**Puntos clave:**
- ❌ Nada de `Trusted_Connection` → usar `User Id`/`Password` (SQL auth).
- Puerto SQL con **coma**: `Server=localhost,1433`.
- `JwtOptions` igual → tokens compatibles.
- SMTP Mailtrap requiere internet saliente desde la VM (solo afecta el envío
  de correo, no el login).

---

## Paso 6 — Ejecutar la API (prueba manual)

```bash
cd /opt/northwind-api
chmod +x NorthWind.Sales.WebApi

ASPNETCORE_URLS="http://0.0.0.0:5103" \
ASPNETCORE_ENVIRONMENT=Development \
./NorthWind.Sales.WebApi
```

- `0.0.0.0` → Kestrel escucha en todas las interfaces (si no, solo la VM
  se conecta).
- `Development` → habilita Swagger (`/swagger`). En producción: `Production`.

**Verificaciones desde la VM:**
```bash
curl -i "http://localhost:5103/SearchCustomers?search=a"   # → 401 (exige token)

curl -s -X POST "http://localhost:5103/user/Login" \
  -H "Content-Type: application/json" \
  -d '{"email":"pos.test@example.com","password":"Test123!xyz"}'
#   → {"accessToken":"eyJ..."}   (si restauraste los datos)

curl -i "http://localhost:5103/swagger/index.html"          # → 200
```
Detener con `Ctrl+C`.

---

## Paso 7 — Servicio permanente con systemd

1. Crear `/etc/systemd/system/northwind-api.service`:
   ```bash
   sudo nano /etc/systemd/system/northwind-api.service
   ```
   ```ini
   [Unit]
   Description=NorthWind POS WebApi
   After=network.target docker.service

   [Service]
   Type=simple
   User=$USER
   WorkingDirectory=/opt/northwind-api
   ExecStart=/opt/northwind-api/NorthWind.Sales.WebApi
   Environment=ASPNETCORE_URLS=http://0.0.0.0:5103
   Environment=ASPNETCORE_ENVIRONMENT=Development
   Restart=always
   RestartSec=5
   SyslogIdentifier=northwind-api

   [Install]
   WantedBy=multi-user.target
   ```
   > Escribe literalmente tu usuario en `User=` (p. ej. `rocky`), `nano` no
   > expande `$USER`.
   >
   > **Framework-dependent:** `ExecStart=/usr/bin/dotnet /opt/northwind-api/NorthWind.Sales.WebApi.dll`

2. Activar:
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl enable --now northwind-api
   sudo systemctl status northwind-api      # active (running)
   ```

3. Logs:
   ```bash
   journalctl -u northwind-api -f
   ```

4. Prueba de arranque automático:
   ```bash
   sudo reboot
   # tras volver:
   systemctl is-active northwind-api                              # active
   curl -i "http://localhost:5103/SearchCustomers?search=a"       # 401
   ```

---

## Paso 8 — Firewall (firewalld) y prueba desde Windows

### En la VM (firewalld, NO ufw)

```bash
sudo systemctl enable --now firewalld
sudo firewall-cmd --state                                   # running

sudo firewall-cmd --permanent --add-port=80/tcp             # solo PARTE 2
sudo firewall-cmd --permanent --add-port=5103/tcp           # API (Opción A de la PARTE 2; en Opción B se puede omitir)
sudo firewall-cmd --reload
sudo firewall-cmd --list-ports

# NO abrir 1433 (SQL solo interno de la VM)
```
> Si usaste **NAT + redirección** (opción C del Paso 0), el tráfico del host
> entra por `127.0.0.1` y no necesita reglas额外; aun así abrir 80/5103 no
> hace daño en una VM de laboratorio.

### Desde Windows (PowerShell)

```powershell
# 1) Sin token debe dar 401
curl -i "http://IP-VM:5103/SearchCustomers?search=a"

# 2) Login con JWT
$login = curl -s -X POST "http://IP-VM:5103/user/Login" `
  -H "Content-Type: application/json" `
  -d '{"email":"pos.test@example.com","password":"Test123!xyz"}'
$token = ($login | ConvertFrom-Json).accessToken

# 3) Búsquedas autenticadas
curl -s -H "Authorization: Bearer $token" "http://IP-VM:5103/SearchCustomers?search=a&page=1&pageSize=3"
curl -s -H "Authorization: Bearer $token" "http://IP-VM:5103/SearchInvoices?search=18&page=1&pageSize=5"

# 4) PDF de la factura 18 (debe empezar con %PDF-)
curl -s -H "Authorization: Bearer $token" "http://IP-VM:5103/GetInvoicePdf?orderId=18" -o factura.pdf
```
**Criterio de éxito:** 401 sin token, 200 + JSON con token, PDF descargado.

---

## Paso 9 — Apuntar el Blazor (Windows) a la VM

1. Editar
   `4. Frameworks And Drivers/UI/NorthWind.Sales.BlazorClient/wwwroot/appsettings.json`:
   ```json
   { "WebApiAddress": "http://IP-VM:5103" }
   ```
   (Con la opción NAT: `http://localhost:5103`.)
2. Reiniciar el Blazor:
   ```bash
   dotnet run --project "4. Frameworks And Drivers/UI/NorthWind.Sales.BlazorClient/NorthWind.Sales.BlazorClient.csproj" --launch-profile http
   ```
3. Abrir `http://localhost:5249` → login → crear orden → factura → PDF.

**No tocar CORS:** la API ya permite cualquier origen (`Startup.cs`).

---

## Checklist final PARTE 1

- [ ] `sudo docker ps` → `northwind-sql` en estado `Up`.
- [ ] `docker logs northwind-sql` → *"Ready for client connections"*.
- [ ] Las 3 BDs restauradas (`SELECT COUNT(*) FROM NorthWindDB.dbo.Orders` > 0).
- [ ] `appsettings.json` con auth SQL (sin `Trusted_Connection`).
- [ ] `systemctl status northwind-api` → `active (running)`.
- [ ] `firewall-cmd --list-ports` incluye 5103 (si se expone directo).
- [ ] `401` sin token y `200` + JWT en login (desde VM y desde Windows).
- [ ] Blazor (`:5249`) consume la API de la VM (login + orden + factura PDF).

---

## Errores típicos y solución (PARTE 1)

| Síntoma | Causa probable | Solución |
|---|---|---|
| `A network-related or instance-specific error...` | Connection string con `Trusted_Connection` | Cambiar a SQL auth (Paso 5) |
| `Connection refused` desde Windows | API escucha solo en localhost | `ASPNETCORE_URLS=http://0.0.0.0:5103` |
| El contenedor se reinicia solo | RAM insuficiente | Subir la VM a ≥ 3 GB |
| `Couldn't find a valid ICU package` al arrancar la API | Falta `libicu` (minimal) | `sudo dnf install -y libicu` y reiniciar el service |
| `dnf config-manager` no existe | Falta `dnf-plugins-core` | `sudo dnf install -y dnf-plugins-core` |
| `curl: (6) Could not resolve host` / repo Docker falla | Sin internet o repo mal | Verificar NAT/puerto y el `docker-ce.repo` |
| `sqlcmd: not found` en el host | No instalaste mssql-tools18 | Usar el del contenedor (`/opt/mssql-tools18/bin/sqlcmd -C`) o SSMS desde Windows |
| Restaurar falla con "cannot open file" | Rutas Windows en el `RESTORE` | `MOVE ... TO '/var/opt/mssql/data/...'` (Paso 4A2) |
| Registro no envía correo | VM sin internet o bloqueo 2525 | Verificar salida; el login no depende del correo |
| Swagger 404 | Entorno `Production` | `ASPNETCORE_ENVIRONMENT=Development` |
| El service no arranca (`status=203/EXEC`) | Ruta/`ExecStart` incorrecto | Revisar Paso 7 según tipo de publicación |

---

## Comandos útiles de operación (PARTE 1)

```bash
# Estado de todo
sudo docker ps && systemctl status northwind-api --no-pager

# Logs de la API
journalctl -u northwind-api -n 100 -f

# Logs de SQL
sudo docker logs --tail 100 northwind-sql

# Reiniciar / detener
sudo systemctl restart northwind-api
sudo systemctl stop northwind-api

# sqlcmd rápido (dentro del contenedor)
sudo docker exec -it northwind-sql /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'Northwind2026!' -C -Q "SELECT COUNT(*) FROM NorthWindDB.dbo.Orders"

# Firewall: reglas actuales
sudo firewall-cmd --list-ports

# Actualizar la API tras un nuevo publish (en Windows)
sudo systemctl stop northwind-api
scp publish/linux/* usuario@IP-VM:/tmp/     # (o rsync)
# reemplazar /opt/northwind-api/* (conservar appsettings.json modificado)
sudo systemctl start northwind-api
```

---
---

# PARTE 2 — Sitio completo con web server (Nginx) en la VM

> **Objetivo:** que la VM sirva **todo**: la interfaz Blazor (estáticos) y la
> API (reverse proxy), con un único web server público (**Nginx**). El
> navegador en Windows solo abre `http://IP-VM/`.
>
> **Prerrequisito:** PARTE 1 completa (Pasos 0–7): VM, Docker con SQL Server,
> BDs restauradas, API publicada, `appsettings.json` con SQL auth y service
> systemd corriendo.

## Arquitectura PARTE 2

```
Navegador (Windows)  ─── http://IP-VM/ ───►
┌─────────────────────────── VM Rocky Linux 8 ─────────────────────────┐
│  NGINX (:80) — /etc/nginx/conf.d/northwind.conf                      │
│   ├─ /            → estáticos Blazor (/var/www/northwind)            │
│   └─ rutas API    → proxy → 127.0.0.1:5103   (Opción B)              │
│                                                                       │
│  API .NET 10 (systemd) → Kestrel                                     │
│  SQL Server 2022 (Docker, :1433 solo interno)                        │
│  SELinux: httpd_sys_content_t en /var/www + bool                     │
│           httpd_can_network_connect (para el proxy)                  │
└───────────────────────────────────────────────────────────────────────┘
```

**Dato clave del proyecto:** los gateways construyen URLs de API **con `/`
inicial** (`/SearchCustomers`, `/user/Login`, …). Por la regla RFC 3986, el
prefijo de path del `BaseAddress` se descarta al combinar con rutas
absolutas → **no sirve un prefijo `/api/`**; por eso hay dos opciones:

| | **Opción A** (2 puertos) | **Opción B** (1 puerto, recomendada) |
|---|---|---|
| Estáticos Blazor | Nginx `:80` | Nginx `:80` |
| API | directa en `:5103` (0.0.0.0) | proxy de rutas → `127.0.0.1:5103` |
| `WebApiAddress` | `http://IP-VM:5103` | `http://IP-VM/` |
| Puertos en firewalld | `80` y `5103` | solo `80` |
| CORS | relevante (ya `AllowAnyOrigin` ✓) | irrelevante (mismo origen) |

---

## P2-Paso A — Publicar el Blazor como sitio estático (en Windows)

1. Desde la raíz del repo (`...\NorthWind`):
   ```bash
   dotnet publish "4. Frameworks And Drivers/UI/NorthWind.Sales.BlazorClient/NorthWind.Sales.BlazorClient.csproj" \
     -c Release -o publish/web
   ```
2. Verificar: `publish/web` con `index.html`, `_framework/`, `css/`,
   `appsettings.json` en la raíz (proyecto `Microsoft.NET.Sdk.BlazorWebAssembly`).
3. Copiar a la VM:
   ```bash
   scp -r publish/web/* usuario@IP-VM:/tmp/web
   ```
4. Colocarlo (lectura para todos; el `chown nginx` se hace en el P2-Paso C
   cuando el usuario `nginx` ya existe):
   ```bash
   sudo mkdir -p /var/www/northwind
   sudo cp -r /tmp/web/* /var/www/northwind/
   sudo chmod -R a+rX /var/www/northwind
   ```

> ⚠️ Si vuelves a modificar el código del front (confirmación al eliminar,
> precio bloqueado, precio automático al teclear el Id, sin duplicados,
> búsqueda en vivo), **repite este paso** y reemplaza
> `/var/www/northwind/` (conserva el `appsettings.json` con `WebApiAddress`).
> Ver *Comandos útiles PARTE 2* → *"Actualizar el front"*.

---

## P2-Paso B — Instalar Nginx

```bash
sudo dnf install -y nginx
sudo systemctl enable --now nginx
sudo systemctl status nginx         # active (running)
```

---

## P2-Paso C — Configurar el sitio (conf.d estilo RHEL)

Rocky/RHEL **no** usa `sites-available/sites-enabled`; la config va en
`/etc/nginx/conf.d/`.

### C.1 — Quitar el sitio por defecto

```bash
# a) Si existe el default en conf.d, eliminarlo:
sudo rm -f /etc/nginx/conf.d/default.conf

# b) Si /etc/nginx/nginx.conf tiene un bloque 'server { ... }' activo,
#    comentarlo (este es el que muestra "Welcome to nginx" en RHEL):
grep -n "server {" /etc/nginx/nginx.conf
sudo nano /etc/nginx/nginx.conf    # poner # al bloque server completo
```
(En algunos paquetes el bloque no existe; si `grep` no lo encuentra, no se
nada que comentar.)

### C.2 — Crear `/etc/nginx/conf.d/northwind.conf`

```bash
sudo nano /etc/nginx/conf.d/northwind.conf
```

### Opción B — un solo puerto (recomendada)

```nginx
server {
    listen 80;
    server_name _;

    root /var/www/northwind;
    index index.html;

    # ---------- API: las 8 rutas del backend → Kestrel (solo localhost) ----------
    # Endpoints.cs (Sales y Membership):
    #   /CreateOrder /SearchCustomers /SearchProducts /SearchInvoices
    #   /GetInvoice /GetInvoicePdf /user/Login /user/Register
    location ~ ^/(SearchCustomers|SearchProducts|SearchInvoices|GetInvoice|GetInvoicePdf|CreateOrder|user)/ {
        proxy_pass http://127.0.0.1:5103;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_read_timeout 60s;              # el PDF puede tardar
        proxy_buffering off;                 # streaming del PDF
    }

    # ---------- Estáticos del Blazor (fallback SPA obligatorio) ----------
    # /invoices y /invoice/19 son rutas de cliente → deben volver index.html
    location / {
        try_files $uri $uri/ /index.html;
    }

    # ---------- MIME para WebAssembly ----------
    # Si mime.types no trae application/wasm (nginx antiguo), este
    # default_type lo resuelve sin afectar a los demás tipos.
    location ~* \.wasm$ {
        default_type application/wasm;
    }
}
```

**Ajuste en la PARTE 1 (solo Opción B)** — la API deja de exponerse:
```bash
sudo nano /etc/systemd/system/northwind-api.service
# Cambiar:   Environment=ASPNETCORE_URLS=http://0.0.0.0:5103
# Por:       Environment=ASPNETCORE_URLS=http://127.0.0.1:5103
sudo systemctl daemon-reload && sudo systemctl restart northwind-api
```

**Configurar el front** (sin recompilar):
```bash
sudo nano /var/www/northwind/appsettings.json
# { "WebApiAddress": "http://IP-VM/" }
```

### Opción A — más simple (2 puertos)

```nginx
server {
    listen 80;
    server_name _;

    root /var/www/northwind;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location ~* \.wasm$ {
        default_type application/wasm;
    }

    # La API se llama directo: http://IP-VM:5103
}
```
- `WebApiAddress`: `"http://IP-VM:5103"`; service con `0.0.0.0:5103`.

### C.3 — Permisos de archivo (usuario nginx)

```bash
sudo chown -R nginx:nginx /var/www/northwind
```

### C.4 — SELinux (obligatorio en Rocky 8)

Sin esto Nginx da **403 Forbidden** (no puede leer `/var/www`) y en la
Opción B el proxy da **502/403** (no puede conectarse a `127.0.0.1:5103`):

```bash
# a) Permitir que httpd lea /var/www/northwind:
sudo dnf install -y policycoreutils-python-utils     # provee 'semanage'
sudo semanage fcontext -a -t httpd_sys_content_t "/var/www/northwind(/.*)?"
sudo restorecon -Rv /var/www/northwind

# b) Permitir conexiones de red de Nginx (proxy a la API) — solo Opción B:
sudo setsebool -P httpd_can_network_connect 1

# Verificar:
ls -Zd /var/www/northwind        # debe decir ... httpd_sys_content_t ...
getsebool httpd_can_network_connect   # --> on
```

---

## P2-Paso D — Validar y recargar

```bash
sudo nginx -t          # syntax is ok / test is successful
sudo systemctl reload nginx
```

---

## P2-Paso E — Firewall (firewalld)

```bash
sudo firewall-cmd --permanent --add-port=80/tcp
sudo firewall-cmd --reload
sudo firewall-cmd --list-ports     # 80 (y 5103 si usas la Opción A)
# 1433 NO se abre
```

---

## P2-Paso F — Verificaciones

Desde la VM:

```bash
# 1) El sitio responde y es el Blazor (no "Welcome to nginx")
curl -s -o /dev/null -w "%{http_code}\n" http://localhost/              # 200
curl -s http://localhost/ | grep -i blazor                              # debe coincidir

# 2) El framework WASM se sirve
curl -s -o /dev/null -w "%{http_code}\n" http://localhost/_framework/blazor.webassembly.js   # 200

# 3) Fallback SPA: un deep link recarga sin 404
curl -s -o /dev/null -w "%{http_code}\n" http://localhost/invoices      # 200 (devuelve index.html)

# 4) API sin token → 401 esperado
#    Opción B (vía proxy):
curl -i "http://localhost/SearchCustomers?search=a"                     # 401
#    Opción A (directa):
curl -i "http://localhost:5103/SearchCustomers?search=a"                # 401
```

Desde **Windows** (navegador):

1. Abrir `http://IP-VM/`.
2. Login: `pos.test@example.com` / `Test123!xyz` (o el usuario registrado).
3. Recorrer: Nueva orden (modales, IVA, crear) → **Facturas** → abrir factura →
   **Descargar PDF**.
4. **Comportamientos de UI a verificar en la Nueva orden:**
   - Modales de cliente/producto: **al abrir ya muestran todos** los
     registros y **al escribir filtran solo** por Id o nombre (sin pulsar
     *Buscar*; el botón y Enter siguen disponibles).
   - Clic **fuera** de cualquier modal → **no** debe cerrarlo.
   - *Eliminar* → modal *"¿Está seguro de eliminar el producto?"* con
     **Sí** / **No**; fuera tampoco lo cierra.
   - Campo **Precio**: gris y no editable; cantidad e Id sí.
   - Teclear un Id de producto a mano **llena el precio** automáticamente
     (si no existe, queda en 0 con su validación).
   - Enviar con el mismo Id en dos filas → mensaje de **duplicados** y no
     crea la orden.
5. Swagger (si la API corre en `Development` y la Opción A):
   `http://IP-VM:5103/swagger`.

**Criterio de éxito PARTE 2:** todo el flujo (login → orden → factura PDF),
incluidos los comportamientos del punto 4, funciona abriendo **únicamente**
`http://IP-VM/`.

---

## P2-Paso G — HTTPS (opcional)

```bash
sudo dnf install -y epel-release
sudo dnf install -y certbot python3-certbot-nginx
sudo certbot --nginx -d tu-dominio.com
sudo systemctl reload nginx     # normalmente certbot lo hace solo
```
En laboratorio sin dominio se puede omitir.

---

## Checklist PARTE 2

- [ ] `systemctl is-active nginx` → `active`.
- [ ] `nginx -t` → `syntax is ok`.
- [ ] `curl http://localhost/` devuelve el `index.html` del Blazor
      (si sale *"Welcome to nginx"*, ver P2-Paso C.1).
- [ ] `/_framework/blazor.webassembly.js` responde 200.
- [ ] `http://IP-VM/invoices` recarga sin error (fallback SPA).
- [ ] Endpoint sin token → 401 (vía proxy o puerto directo según opción).
- [ ] SELinux: `ls -Zd /var/www/northwind` → `httpd_sys_content_t`;
      `getsebool httpd_can_network_connect` → `on`.
- [ ] `WebApiAddress` apunta a la URL correcta (`/` o `:5103`).
- [ ] Opción B: API solo en `127.0.0.1:5103`; firewalld solo con 80.
- [ ] Login + crear orden + PDF de factura funcionando desde `http://IP-VM/`.
- [ ] Modales: listan todo al abrir y filtran solo al escribir; clic fuera
      no los cierra.
- [ ] *Eliminar* muestra el modal **Sí/No**; precio bloqueado.
- [ ] Teclear un Id de producto llena el precio automáticamente.
- [ ] Enviar con productos duplicados (mismo Id) muestra el error.

---

## Errores típicos PARTE 2

| Síntoma | Causa probable | Solución |
|---|---|---|
| "Welcome to nginx" | Bloque `server` por defecto activo | P2-Paso C.1: borrar `conf.d/default.conf` o comentar el `server` de `nginx.conf` |
| `403 Forbidden` en `/` | **SELinux**: Nginx no lee `/var/www` | P2-Paso C.4a (`semanage` + `restorecon`) |
| `502 Bad Gateway` (Opción B) | API caída o **SELinux** bloquea el proxy | `systemctl status northwind-api`; P2-Paso C.4b (`httpd_can_network_connect`) |
| `404` al recargar `/invoices` | Falta el fallback SPA | `try_files $uri $uri/ /index.html` |
| `blazor.webassembly.js` 404 o MIME raro | `root` mal apuntado o `.wasm` sin tipo | Verificar `root /var/www/northwind` y el `location ~* \.wasm$` |
| Front carga pero el login da error de red | `WebApiAddress` mal | Opción B: `http://IP-VM/` · Opción A: `http://IP-VM:5103` |
| `404`/`403` en rutas API vía proxy | Falta una ruta en el regex `location ~` | Revisar las 8 rutas (nuevos endpoints deben agregarse) |
| PDF tarda o se corta | Timeout/buffering del proxy | `proxy_read_timeout 60s` y `proxy_buffering off` ya incluidos |
| `dnf install nginx` no encuentra paquete | Repos mínimos | `sudo dnf makecache` o verificar red/NAT |

---

## Comandos útiles PARTE 2

```bash
# Estado general
sudo systemctl status nginx northwind-api --no-pager
sudo docker ps

# Logs de Nginx (Rocky: /var/log/nginx/)
sudo tail -f /var/log/nginx/access.log /var/log/nginx/error.log

# Validar y recargar config tras cada cambio
sudo nginx -t && sudo systemctl reload nginx

# SELinux: comprobar permisos
ls -Zd /var/www/northwind
getsebool httpd_can_network_connect
sudo restorecon -Rv /var/www/northwind     # si se movieron archivos

# Actualizar el front tras re-publicar en Windows
rsync -av --delete publish/web/ usuario@IP-VM:/var/www/northwind/
sudo chown -R nginx:nginx /var/www/northwind
sudo systemctl reload nginx

# Probar la API igual que el navegador (misma ruta del front)
curl -i "http://localhost/SearchCustomers?search=a"    # Opción B
```
