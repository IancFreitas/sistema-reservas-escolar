using SistemaReservas.Data;
using SistemaReservas.Domain;
using SistemaReservas.Security;

namespace SistemaReservas.Services;

public sealed class EmployeeService
{
    public const int MinimumPasswordLength = 6;

    private readonly SchoolDatabase _database;

    public EmployeeService(SchoolDatabase database)
    {
        _database = database;
    }

    public IReadOnlyList<Employee> GetAll() =>
        _database.Employees.OrderBy(e => e.FullName).ToList();

    public OperationResult Register(Employee requester, string username, string fullName, string password, Role role)
    {
        if (!requester.IsAdministrator)
        {
            return OperationResult.Fail("Apenas administradores podem cadastrar funcionários.");
        }

        username = username.Trim();
        fullName = fullName.Trim();

        if (username.Length == 0 || fullName.Length == 0)
        {
            return OperationResult.Fail("Usuário e nome são obrigatórios.");
        }

        if (password.Length < MinimumPasswordLength)
        {
            return OperationResult.Fail($"A senha precisa ter pelo menos {MinimumPasswordLength} caracteres.");
        }

        if (_database.FindEmployee(username) is not null)
        {
            return OperationResult.Fail($"Já existe um funcionário com o usuário \"{username}\".");
        }

        _database.Employees.Add(new Employee(username, fullName, role, PasswordHasher.Hash(password)));
        return OperationResult.Ok($"Funcionário \"{fullName}\" cadastrado com sucesso.");
    }

    public OperationResult Remove(Employee requester, string username)
    {
        if (!requester.IsAdministrator)
        {
            return OperationResult.Fail("Apenas administradores podem remover funcionários.");
        }

        Employee? employee = _database.FindEmployee(username.Trim());
        if (employee is null)
        {
            return OperationResult.Fail($"Nenhum funcionário encontrado com o usuário \"{username}\".");
        }

        if (employee == requester)
        {
            return OperationResult.Fail("Você não pode remover o seu próprio usuário.");
        }

        _database.Employees.Remove(employee);
        return OperationResult.Ok($"Funcionário \"{employee.FullName}\" removido com sucesso.");
    }
}
