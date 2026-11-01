using SistemaReservas.Data;
using SistemaReservas.Domain;
using SistemaReservas.Services;

namespace SistemaReservas.Tests;

public class ReservationServiceTests
{
    private readonly SchoolDatabase _database = SchoolDatabase.CreateWithSampleData();
    private readonly ReservationService _service;
    private readonly Employee _admin;
    private readonly Employee _teacher;

    public ReservationServiceTests()
    {
        _service = new ReservationService(_database);
        _admin = _database.FindEmployee("admin")!;
        _teacher = _database.FindEmployee("professor")!;
    }

    [Fact]
    public void Reserve_MarksResourceAsReservedByEmployee()
    {
        Room room = _database.Rooms[0];

        OperationResult result = _service.Reserve(room, _teacher);

        Assert.True(result.Success);
        Assert.Equal("professor", room.ReservedBy);
        Assert.DoesNotContain(room, _service.GetAvailableRooms());
    }

    [Fact]
    public void Reserve_FailsWhenResourceIsAlreadyReserved()
    {
        Room room = _database.Rooms[0];
        _service.Reserve(room, _admin);

        OperationResult result = _service.Reserve(room, _teacher);

        Assert.False(result.Success);
        Assert.Equal("admin", room.ReservedBy);
    }

    [Fact]
    public void GetAvailableEquipment_ReturnsOnlyFreeItemsOfRequestedType()
    {
        Equipment notebook = _database.Equipment.First(e => e.Type == EquipmentType.Notebook);
        _service.Reserve(notebook, _teacher);

        IReadOnlyList<Equipment> available = _service.GetAvailableEquipment(EquipmentType.Notebook);

        Assert.Equal(2, available.Count);
        Assert.All(available, e => Assert.Equal(EquipmentType.Notebook, e.Type));
        Assert.DoesNotContain(notebook, available);
    }

    [Fact]
    public void Return_ReleasesResourceSoItCanBeReservedAgain()
    {
        Equipment tablet = _database.Equipment.First(e => e.Type == EquipmentType.Tablet);
        _service.Reserve(tablet, _teacher);

        OperationResult result = _service.Return(tablet, _teacher);

        Assert.True(result.Success);
        Assert.False(tablet.IsReserved);
        Assert.Contains(tablet, _service.GetAvailableEquipment(EquipmentType.Tablet));
    }

    [Fact]
    public void Return_FailsWhenResourceIsNotReserved()
    {
        OperationResult result = _service.Return(_database.Rooms[0], _teacher);

        Assert.False(result.Success);
    }

    [Fact]
    public void Return_TeacherCannotReturnSomeoneElsesReservation()
    {
        Room room = _database.Rooms[0];
        _service.Reserve(room, _admin);

        OperationResult result = _service.Return(room, _teacher);

        Assert.False(result.Success);
        Assert.True(room.IsReserved);
    }

    [Fact]
    public void Return_AdministratorCanReturnAnyReservation()
    {
        Room room = _database.Rooms[0];
        _service.Reserve(room, _teacher);

        OperationResult result = _service.Return(room, _admin);

        Assert.True(result.Success);
        Assert.False(room.IsReserved);
    }

    [Fact]
    public void GetReturnableBy_TeacherSeesOnlyOwnReservations()
    {
        _service.Reserve(_database.Rooms[0], _admin);
        _service.Reserve(_database.Rooms[1], _teacher);

        IReadOnlyList<Resource> teacherItems = _service.GetReturnableBy(_teacher);
        IReadOnlyList<Resource> adminItems = _service.GetReturnableBy(_admin);

        Assert.Single(teacherItems);
        Assert.Equal(2, adminItems.Count);
    }
}
