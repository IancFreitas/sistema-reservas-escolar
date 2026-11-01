using SistemaReservas.Data;
using SistemaReservas.Domain;
using SistemaReservas.Services;

namespace SistemaReservas.Tests;

public class EmployeeServiceTests
{
    private readonly SchoolDatabase _database = SchoolDatabase.CreateWithSampleData();
    private readonly EmployeeService _service;
    private readonly AuthService _auth;
    private readonly Employee _admin;
    private readonly Employee _teacher;

    public EmployeeServiceTests()
    {
        _service = new EmployeeService(_database);
        _auth = new AuthService(_database);
        _admin = _database.FindEmployee("admin")!;
        _teacher = _database.FindEmployee("professor")!;
    }

    [Fact]
    public void Register_AdministratorCreatesEmployeeWhoCanLogIn()
    {
        OperationResult result = _service.Register(_admin, "maria", "Maria Souza", "segredo1", Role.Teacher);

        Assert.True(result.Success);
        Employee? loggedIn = _auth.Login("maria", "segredo1");
        Assert.NotNull(loggedIn);
        Assert.Equal(Role.Teacher, loggedIn!.Role);
    }

    [Fact]
    public void Register_FailsForTeacher()
    {
        OperationResult result = _service.Register(_teacher, "maria", "Maria Souza", "segredo1", Role.Teacher);

        Assert.False(result.Success);
        Assert.Null(_database.FindEmployee("maria"));
    }

    [Theory]
    [InlineData("professor")]
    [InlineData("PROFESSOR")]
    public void Register_FailsForDuplicatedUsername(string username)
    {
        int countBefore = _database.Employees.Count;

        OperationResult result = _service.Register(_admin, username, "Outro Nome", "segredo1", Role.Teacher);

        Assert.False(result.Success);
        Assert.Equal(countBefore, _database.Employees.Count);
    }

    [Theory]
    [InlineData("", "Nome", "segredo1")]
    [InlineData("usuario", "  ", "segredo1")]
    [InlineData("usuario", "Nome", "123")]
    public void Register_FailsForInvalidData(string username, string fullName, string password)
    {
        OperationResult result = _service.Register(_admin, username, fullName, password, Role.Teacher);

        Assert.False(result.Success);
    }

    [Fact]
    public void Remove_AdministratorRemovesExistingEmployee()
    {
        OperationResult result = _service.Remove(_admin, "professor");

        Assert.True(result.Success);
        Assert.Null(_database.FindEmployee("professor"));
    }

    [Fact]
    public void Remove_FailsForUnknownUser()
    {
        OperationResult result = _service.Remove(_admin, "nao-existe");

        Assert.False(result.Success);
    }

    [Fact]
    public void Remove_AdministratorCannotRemoveThemselves()
    {
        OperationResult result = _service.Remove(_admin, "admin");

        Assert.False(result.Success);
        Assert.NotNull(_database.FindEmployee("admin"));
    }
}
