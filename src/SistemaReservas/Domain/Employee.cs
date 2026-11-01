namespace SistemaReservas.Domain;

public enum Role
{
    Administrator,
    Teacher
}

public sealed class Employee
{
    public Employee(string username, string fullName, Role role, string passwordHash)
    {
        Username = username;
        FullName = fullName;
        Role = role;
        PasswordHash = passwordHash;
    }

    public string Username { get; }
    public string FullName { get; }
    public Role Role { get; }
    public string PasswordHash { get; }

    public bool IsAdministrator => Role == Role.Administrator;
}
