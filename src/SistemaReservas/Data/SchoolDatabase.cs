using SistemaReservas.Domain;
using SistemaReservas.Security;

namespace SistemaReservas.Data;

/// <summary>
/// Armazenamento em memória do colégio. Concentra todos os dados em um só lugar
/// para que, no futuro, possa ser trocado por um banco de dados real sem mexer nos serviços.
/// </summary>
public sealed class SchoolDatabase
{
    public List<Room> Rooms { get; } = new();
    public List<Equipment> Equipment { get; } = new();
    public List<Employee> Employees { get; } = new();

    public Employee? FindEmployee(string username) =>
        Employees.FirstOrDefault(e => string.Equals(e.Username, username, StringComparison.OrdinalIgnoreCase));

    /// <summary>Cria a base com os dados de demonstração descritos no README.</summary>
    public static SchoolDatabase CreateWithSampleData()
    {
        var database = new SchoolDatabase();

        database.Rooms.AddRange(new[]
        {
            new Room(1, "Sala de Vídeo", 43),
            new Room(2, "Sala de Teatro", 52),
            new Room(3, "Sala de Apresentação", 12)
        });

        database.Equipment.AddRange(new[]
        {
            new Equipment(1, EquipmentType.Notebook, "Sony Vaio i456", "01325546", 110),
            new Equipment(2, EquipmentType.Notebook, "Positivo Dual", "032465", 110),
            new Equipment(3, EquipmentType.Notebook, "Dell Inspiron i9", "456121", 220),
            new Equipment(4, EquipmentType.Tablet, "Samsung A16", "123564", 5),
            new Equipment(5, EquipmentType.Tablet, "Positivo FD5", "498498", 5),
            new Equipment(6, EquipmentType.Tablet, "iPad 10", "465186", 5),
            new Equipment(7, EquipmentType.Projector, "Epson 410", "781245", 220),
            new Equipment(8, EquipmentType.Projector, "Sony Light 12", "903417", 220),
            new Equipment(9, EquipmentType.Projector, "HP FullCinema", "2362198", 220)
        });

        database.Employees.AddRange(new[]
        {
            new Employee("admin", "Administrador do Sistema", Role.Administrator, PasswordHasher.Hash("admin123")),
            new Employee("professor", "Professor de Exemplo", Role.Teacher, PasswordHasher.Hash("prof123"))
        });

        return database;
    }
}
