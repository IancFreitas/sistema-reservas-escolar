using SistemaReservas.Data;
using SistemaReservas.Domain;

namespace SistemaReservas.Services;

public sealed class ReservationService
{
    private readonly SchoolDatabase _database;

    public ReservationService(SchoolDatabase database)
    {
        _database = database;
    }

    public IReadOnlyList<Room> GetAvailableRooms() =>
        _database.Rooms.Where(r => !r.IsReserved).ToList();

    public IReadOnlyList<Equipment> GetAvailableEquipment(EquipmentType type) =>
        _database.Equipment.Where(e => e.Type == type && !e.IsReserved).ToList();

    /// <summary>Itens que o funcionário pode devolver: os dele, ou todos se for administrador.</summary>
    public IReadOnlyList<Resource> GetReturnableBy(Employee employee) =>
        AllResources()
            .Where(r => r.IsReserved && (employee.IsAdministrator || IsReservedBy(r, employee)))
            .ToList();

    public IReadOnlyList<Resource> GetReservationsOf(Employee employee) =>
        AllResources().Where(r => IsReservedBy(r, employee)).ToList();

    public OperationResult Reserve(Resource resource, Employee employee)
    {
        if (resource.IsReserved)
        {
            return OperationResult.Fail($"{resource.Name} já está reservado(a) por {resource.ReservedBy}.");
        }

        resource.Reserve(employee.Username);
        return OperationResult.Ok($"{resource.Name} reservado(a) com sucesso.");
    }

    public OperationResult Return(Resource resource, Employee employee)
    {
        if (!resource.IsReserved)
        {
            return OperationResult.Fail($"{resource.Name} não está reservado(a).");
        }

        if (!employee.IsAdministrator && !IsReservedBy(resource, employee))
        {
            return OperationResult.Fail("Só quem fez a reserva ou um administrador pode devolver este item.");
        }

        resource.Release();
        return OperationResult.Ok($"{resource.Name} devolvido(a) com sucesso.");
    }

    private IEnumerable<Resource> AllResources() =>
        _database.Rooms.Cast<Resource>().Concat(_database.Equipment);

    private static bool IsReservedBy(Resource resource, Employee employee) =>
        string.Equals(resource.ReservedBy, employee.Username, StringComparison.OrdinalIgnoreCase);
}
