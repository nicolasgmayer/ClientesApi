ClientesAPI – Web API en ASP.NET Core (Minimal API)
ClientesAPI es una Web API desarrollada con ASP.NET Core (.NET 8) utilizando el enfoque Minimal API.
Su propósito es exponer operaciones CRUD básicas sobre un conjunto de clientes almacenados en un archivo JSON local, sin utilizar base de datos.

La API está diseñada para ser simple, ligera y fácil de integrar con aplicaciones externas, como la aplicación de escritorio ClientImport.

Características principales
-Implementada con ASP.NET Core Minimal API
-CRUD completo sobre clientes
-Almacenamiento en archivo JSON
-Sin base de datos ni autenticación
-Endpoints REST estándar
-Compatible con Swagger/OpenAPI
-Validación básica de datos
-Integración directa con la aplicación WinForms ClientImport

Endpoints disponibles:

-GET /clientes
Devuelve la lista completa de clientes almacenados en el archivo JSON.

-GET /clientes/{dni}
Devuelve un cliente específico por su DNI.

Respuestas:

-200 OK si existe
-404 Not Found si no existe

POST /clientes:

-Crea un nuevo cliente.
-Requiere un objeto JSON con los datos del cliente.

Respuestas:

-201 Created si se crea correctamente
-400 Bad Request si el DNI ya existe o los datos son inválidos

DELETE /clientes/{dni}

-Elimina un cliente por DNI.

Respuestas:

-204 No Content si se elimina
-404 Not Found si no existe

Almacenamiento de datos:
La API utiliza un archivo JSON para persistir los datos.

El archivo se encuentra en:

<data>\clientes_store.json
donde <data> es una carpeta ubicada junto al ejecutable de la API.

Ejemplo típico:

ClientesAPI\bin\Debug\net8.0\data\clientes_store.json
El archivo se crea automáticamente si no existe.


Requisitos:

.NET 8 o superior
Windows, Linux o macOS
Permisos de lectura/escritura en la carpeta del ejecutable

He dejado un archivo llamado "clientes_store.json" en el proyecto que viene desde la aplicación de escritorio ClientImport para realizar pruebas. Se debe colocar dentro de la carpeta "data" creada en el proyecto.
