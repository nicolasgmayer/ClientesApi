namespace ClientesApi.Models
{
    public record Cliente(
        string Dni,
        string Nombre,
        string Apellidos,
        DateTime FechaNacimiento,
        string Telefono,
        string Email
    );
}
