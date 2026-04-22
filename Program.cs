using System.Text.Json;
using System.Text.RegularExpressions;
using ClientesApi.Models;

// Configuración inicial de la aplicación y registro de servicios necesarios para exponer la API y generar documentación Swagger.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Habilitación de Swagger únicamente en entorno de desarrollo para facilitar pruebas y documentación interactiva.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Definición de la ruta donde se almacenará el archivo JSON que contiene los clientes.
// Se utiliza una carpeta "data" dentro del directorio base de la aplicación.
var dataFolder = Path.Combine(AppContext.BaseDirectory, "data");
Directory.CreateDirectory(dataFolder);
var jsonPath = Path.Combine(dataFolder, "clientes_store.json");

// Función auxiliar para leer el archivo JSON y deserializarlo en una lista de clientes.
// Si el archivo no existe, se devuelve una lista vacía para evitar errores.
List<Cliente> LeerClientes()
{
    if (!File.Exists(jsonPath))
        return new List<Cliente>();

    var json = File.ReadAllText(jsonPath);
    return JsonSerializer.Deserialize<List<Cliente>>(json) ?? new List<Cliente>();
}

#region Funciones de Validación y Persistencia

// Función auxiliar para persistir la lista de clientes en el archivo JSON.
// Se utiliza formato indentado para facilitar la lectura del archivo.
void GuardarClientes(List<Cliente> clientes)
{
    var json = JsonSerializer.Serialize(clientes, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(jsonPath, json);
}

// Función de validación que verifica que los datos del cliente cumplan con los requisitos mínimos.
// Si algún campo no es válido, se devuelve false junto con un mensaje descriptivo del error.
bool EsClienteValido(Cliente c, out string error)
{
    error = "";

    // Validación del formato del DNI: debe contener exactamente 8 números seguidos de una letra.
    if (!Regex.IsMatch(c.Dni, @"^[0-9]{8}[A-Za-z]$"))
    {
        error = "El DNI debe tener 8 números seguidos de una letra.";
        return false;
    }

    // Validación básica del formato de email mediante expresión regular.
    if (!Regex.IsMatch(c.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
    {
        error = "Email inválido.";
        return false;
    }

    // Validación del teléfono: debe contener únicamente dígitos y tener una longitud razonable.
    if (!Regex.IsMatch(c.Telefono, @"^\d{7,15}$"))
    {
        error = "El teléfono debe tener entre 7 y 15 dígitos.";
        return false;
    }

    // Validación de la fecha de nacimiento: no se permite una fecha futura.
    if (c.FechaNacimiento > DateTime.Now)
    {
        error = "La fecha de nacimiento no puede ser futura.";
        return false;
    }

    return true;
}
#endregion

#region Endpoints
// ENDPOINTS

// Endpoint GET /clientes
// Devuelve la lista completa de clientes almacenados en el archivo JSON.
app.MapGet("/clientes", () =>
{
    return Results.Ok(LeerClientes());
});

// Endpoint GET /clientes/{dni}
// Busca un cliente por su DNI y devuelve 404 si no existe.
app.MapGet("/clientes/{dni}", (string dni) =>
{
    var clientes = LeerClientes();
    var cliente = clientes.FirstOrDefault(c => c.Dni == dni);

    return cliente is null
        ? Results.NotFound()
        : Results.Ok(cliente);
});

// Endpoint POST /clientes/lote
// Permite agregar múltiples clientes en una sola operación.
// Si alguno de los clientes es inválido o está duplicado, se devuelve un 400 sin guardar cambios.
app.MapPost("/clientes/lote", (List<Cliente> nuevos) =>
{
    var clientes = LeerClientes();

    var errores = new List<string>();
    var agregados = new List<Cliente>();

    foreach (var nuevo in nuevos)
    {
        // Verificación de duplicados por DNI.
        if (clientes.Any(c => c.Dni == nuevo.Dni))
        {
            errores.Add($"DNI duplicado: {nuevo.Dni}");
            continue;
        }

        // Validación de los datos del cliente.
        if (!EsClienteValido(nuevo, out var error))
        {
            errores.Add($"Cliente {nuevo.Dni} inválido: {error}");
            continue;
        }

        // Acumulación de clientes válidos para su posterior guardado.
        agregados.Add(nuevo);
    }

    // Si se detectaron errores, no se realizan modificaciones y se devuelve un 400 con el detalle.
    if (errores.Any())
    {
        return Results.BadRequest(new
        {
            mensaje = "Algunos clientes no son válidos",
            errores
        });
    }

    // Si todos los clientes son válidos, se agregan y se persisten en el archivo JSON.
    clientes.AddRange(agregados);
    GuardarClientes(clientes);

    return Results.Created("/clientes/lote", new
    {
        mensaje = "Clientes agregados correctamente",
        cantidad = agregados.Count
    });
});

// Endpoint DELETE /clientes/{dni}
// Elimina un cliente por DNI y devuelve 204 si la operación es exitosa.
app.MapDelete("/clientes/{dni}", (string dni) =>
{
    var clientes = LeerClientes();
    var cliente = clientes.FirstOrDefault(c => c.Dni == dni);

    if (cliente is null)
        return Results.NotFound();

    clientes.Remove(cliente);
    GuardarClientes(clientes);

    return Results.NoContent();
});
#endregion

// Inicio de la aplicación.
app.Run();
