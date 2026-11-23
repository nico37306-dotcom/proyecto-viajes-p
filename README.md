# Terra Andina · Agencia de Viajes

MVP monolítico en Blazor Web App .NET 10 para una agencia de turismo en Bolivia. El catálogo se renderiza en el servidor y los formularios privados usan Interactive Server. ASP.NET Core Identity, EF Core Code First y SQL Server comparten el mismo proyecto `Agencia.Web`.

## Ejecutar en Windows

Se necesita el SDK de .NET 10 y acceso a una instancia SQL Server. La configuración inicial usa SQL Server LocalDB `(localdb)\MSSQLLocalDB`. Si usas SQL Server Express, guarda la cadena de tu instancia con User Secrets, por ejemplo:

```powershell
dotnet user-secrets set 'ConnectionStrings:DefaultConnection' 'Server=.\SQLEXPRESS;Database=AgenciaAndina;Integrated Security=True;TrustServerCertificate=True' --project Agencia.Web.csproj
```

Crea la cuenta que administrará la agencia. Usa un correo que todavía no esté registrado y una contraseña propia que cumpla la política de Identity.

```powershell
dotnet user-secrets set 'Agency:AdminEmail' 'administracion@ejemplo.com' --project Agencia.Web.csproj
dotnet user-secrets set 'Agency:AdminPassword' '<tu-contraseña-segura>' --project Agencia.Web.csproj
dotnet dev-certs https --trust
```

Abre [https://localhost:7055](https://localhost:7055). Con `Agency:InitializeDatabase=true`, el primer inicio aplica las migraciones y crea los roles y departamentos. La cuenta de conexión debe poder crear o migrar la base de datos. En el perfil de desarrollo, `Agency:DemoCatalog=true` carga tours, fechas relativas al primer inicio, una promoción y un paquete. La cuenta administradora se crea sin una contraseña predeterminada y el arranque no concede privilegios a un usuario registrado anteriormente.

Con `Agency:DemoCatalog=true` también puedes crear una cuenta de viajero de prueba (`viajero.demo@example.test`). Configura su contraseña antes de iniciar la aplicación:

```powershell
dotnet user-secrets set 'Auth:DemoUserPassword' '<tu-contraseña-de-prueba>' --project Agencia.Web.csproj
dotnet run --project Agencia.Web.csproj --launch-profile https
```

También puedes usar la variable de entorno `Auth__DemoUserPassword`. Si falta, la aplicación registra una advertencia y continúa sin crear esa cuenta. Si el correo ya existe, no cambia su contraseña ni sus permisos. No guardes la contraseña en `appsettings.json`.

Para generar el CSS de Tailwind con el tema del proyecto:

```powershell
npm install
npm run css
dotnet build Agencia.Web.csproj
```

La hoja base permite ejecutar el proyecto también sin npm. Las fotografías de demostración son imágenes remotas de Wikimedia Commons con sus licencias en `/creditos`. Puedes reemplazarlas desde el catálogo por URL HTTPS o por rutas bajo `wwwroot/images`.

## Desplegar en Render

El proyecto se despliega como **Web Service** con runtime **Docker**, usando el `Dockerfile` de la raíz. Render termina HTTPS y la aplicación escucha HTTP en el puerto `PORT` (10000 por defecto). Blazor Interactive Server requiere conexiones WebSocket, admitidas por Render.

La base configurada para desarrollo es SQL Server LocalDB de Windows; no funciona dentro del contenedor Linux. Antes de desplegar, prepara un **SQL Server accesible desde Render** y configura estas variables en *Environment* del servicio:

| Variable | Valor |
| --- | --- |
| `ConnectionStrings__DefaultConnection` | `Server=tcp:<host>,1433;Database=AgenciaAndina;User ID=<usuario>;Password=<contraseña>;Encrypt=True;TrustServerCertificate=False` |
| `Agency__InitializeDatabase` | `true` para aplicar migraciones al iniciar y crear roles; la cuenta SQL necesita permisos para ello |
| `Agency__DemoCatalog` | `false` en producción |
| `Agency__AdminEmail` | Correo de una cuenta administradora nueva |
| `Agency__AdminPassword` | Contraseña segura para esa cuenta |

Conecta el repositorio en Render, crea un Web Service con Docker y despliega. `Auth__DemoUserPassword` solo se necesita si activas `Agency__DemoCatalog` y quieres la cuenta de viajero demo. La base administrada PostgreSQL de Render no es compatible con las migraciones SQL Server actuales; cambiarla requiere adaptar el proveedor y generar migraciones nuevas.

El sistema de archivos del servicio es temporal. Para conservar las claves de autenticación entre despliegues, en un plan que admita disco persistente monta uno en `/var/data` y configura `Agency__DataProtectionKeysPath=/var/data/keys`. Sin claves persistentes, las sesiones de los usuarios se invalidan al reiniciar el contenedor.

## Flujos de prueba

1. Registra un cliente desde `/Account/Register`. Elige una salida en `/salidas`, añade pasajeros y solicita la reserva. El código de demostración `ALTIPLANO` aplica 10 % si sigue vigente.
2. Accede como administrador a `/panel`. En Ventas, revisa la reserva, apruébala y déjala pendiente de pago. En Contabilidad registra y confirma un anticipo y el saldo. Si corresponde, adjunta un PDF, JPG o PNG de hasta 2 MB. El cliente puede descargar el comprobante desde su propia reserva.
3. En Operaciones asigna guía y vehículo y registra el checklist. Cuando llegue la fecha de fin y las solicitudes estén resueltas, cierra la salida. Después Ventas puede finalizar la reserva y el cliente puede dejar una reseña.
4. Para una organización, crea su ficha y vincula una cuenta con el rol `Organizacion`. Desde su portal envía una solicitud. Usa Alianzas para enviar una oferta. Acepta la propuesta desde el portal y convierte la cotización en una salida privada.
5. Para probar permisos, da de alta un empleado con su departamento y asocia una cuenta con rol `Empleado`. El panel solo lista sus módulos y los servicios validan el permiso y, cuando aplica, la propiedad del recurso.

## Alcance de esta versión

- Catálogo, formularios comerciales y portal de viajeros en español e inglés. Los paneles internos y parte de la configuración avanzada de Identity se presentan en español o mantienen los textos de la plantilla.
- Las reservas retienen cupos al enviarse. La cancelación libera por transacción los lugares retenidos. Las versiones de fila de SQL Server protegen reservas, salidas y cotizaciones frente a escrituras concurrentes.
- El servicio de precios aplica el mayor descuento elegible al precio del tour. Los extras tienen precio por reserva. BOB y USD son tarifas explícitas. Los paquetes se comercializan por consulta y los viajes derivados de cotización se registran como salidas privadas.
- Los pagos de la reserva usan su moneda. Efectivo, transferencia y QR se registran manualmente. La confirmación de tarjeta queda pendiente de integrar un proveedor mediante `IPaymentGateway`. Los reembolsos registran una operación que se realizó externamente.
- Los recibos se almacenan en SQL Server y se descargan con autorización del propietario o Contabilidad. NIT y razón social se guardan para facturación externa. El proyecto usa configuración externa para secretos.
- La gestión de fichas de catálogo, empleados, proveedores, guías y demás datos maestros usa los módulos CRUD de `AdminService`. Los procesos de reservas, pagos, cotizaciones y operaciones usan servicios específicos.

Para configurar una instalación sin datos de ejemplo, establece `Agency:DemoCatalog=false`. Tras inicializar la base, puedes retirar el secreto de la contraseña inicial. Si la infraestructura aplica las migraciones por separado, desactiva `Agency:InitializeDatabase` después de inicializar roles y departamentos. Usa `ConnectionStrings__DefaultConnection`, `Agency__AdminEmail` y `Agency__AdminPassword` si gestionas la configuración con variables de entorno.
