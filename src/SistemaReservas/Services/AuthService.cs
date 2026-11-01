using SistemaReservas.Data;
using SistemaReservas.Domain;
using SistemaReservas.Security;

namespace SistemaReservas.Services;

public sealed class AuthService
{
    private readonly SchoolDatabase _database;

    public AuthService(SchoolDatabase database)
    {
        _database = database;
    }

    /// <returns>O funcionário autenticado, ou <c>null</c> se usuário ou senha estiverem errados.</returns>
    public Employee? Login(string username, string password)
    {
        Employee? employee = _database.FindEmployee(username);
        return employee is not null && PasswordHasher.Verify(password, employee.PasswordHash)
            ? employee
            : null;
    }
}
