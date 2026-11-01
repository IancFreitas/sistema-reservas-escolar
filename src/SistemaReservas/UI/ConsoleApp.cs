using SistemaReservas.Data;
using SistemaReservas.Domain;
using SistemaReservas.Services;

namespace SistemaReservas.UI;

/// <summary>
/// Interface de console. Só cuida de mostrar menus e ler respostas;
/// todas as regras de negócio ficam nos serviços.
/// </summary>
public sealed class ConsoleApp
{
    private const int MaxLoginAttempts = 3;

    private static readonly (EquipmentType Type, string Label)[] EquipmentTypes =
    {
        (EquipmentType.Notebook, "Notebooks"),
        (EquipmentType.Projector, "Projetores"),
        (EquipmentType.Tablet, "Tablets")
    };

    private readonly AuthService _auth;
    private readonly ReservationService _reservations;
    private readonly EmployeeService _employees;

    public ConsoleApp(SchoolDatabase database)
    {
        _auth = new AuthService(database);
        _reservations = new ReservationService(database);
        _employees = new EmployeeService(database);
    }

    public void Run()
    {
        Console.WriteLine("=== Sistema de Reservas | Colégio Vencer Sempre ===");

        Employee? user = Authenticate();
        if (user is null)
        {
            ShowError("Número máximo de tentativas atingido. Procure um administrador.");
            return;
        }

        RunSession(user);
    }

    private Employee? Authenticate()
    {
        for (int attempt = 1; attempt <= MaxLoginAttempts; attempt++)
        {
            string username = ConsoleInput.ReadText("Usuário: ");
            string password = ConsoleInput.ReadPassword("Senha: ");

            Employee? employee = _auth.Login(username, password);
            if (employee is not null)
            {
                return employee;
            }

            ShowError($"Usuário ou senha inválidos ({attempt}/{MaxLoginAttempts}).");
        }

        return null;
    }

    private void RunSession(Employee user)
    {
        Console.WriteLine($"\nBem-vindo(a), {user.FullName}!");

        var options = new List<(string Label, Action Action)>
        {
            ("Reservar sala", () => ReserveRoom(user)),
            ("Reservar equipamento", () => ReserveEquipment(user)),
            ("Devolver item", () => ReturnItem(user)),
            ("Minhas reservas", () => ShowReservations(user))
        };

        if (user.IsAdministrator)
        {
            options.Add(("Gerenciar funcionários", () => ManageEmployees(user)));
        }

        while (true)
        {
            int choice = ConsoleInput.ReadMenuOption("Menu principal", options.Select(o => o.Label).ToList(), "Sair");
            if (choice == 0)
            {
                Console.WriteLine("Até logo!");
                return;
            }

            options[choice - 1].Action();
        }
    }

    private void ReserveRoom(Employee user)
    {
        IReadOnlyList<Room> rooms = _reservations.GetAvailableRooms();
        if (rooms.Count == 0)
        {
            ShowError("Todas as salas estão ocupadas no momento.");
            return;
        }

        Room? room = ConsoleInput.Choose("Qual sala deseja reservar?", rooms, r => r.Description);
        if (room is not null)
        {
            Show(_reservations.Reserve(room, user));
        }
    }

    private void ReserveEquipment(Employee user)
    {
        int typeChoice = ConsoleInput.ReadMenuOption(
            "Qual tipo de equipamento?", EquipmentTypes.Select(t => t.Label).ToList());
        if (typeChoice == 0)
        {
            return;
        }

        (EquipmentType type, string label) = EquipmentTypes[typeChoice - 1];
        IReadOnlyList<Equipment> available = _reservations.GetAvailableEquipment(type);
        if (available.Count == 0)
        {
            ShowError($"Não há {label.ToLowerInvariant()} disponíveis no momento.");
            return;
        }

        Equipment? equipment = ConsoleInput.Choose($"{label} disponíveis", available, e => e.Description);
        if (equipment is not null)
        {
            Show(_reservations.Reserve(equipment, user));
        }
    }

    private void ReturnItem(Employee user)
    {
        IReadOnlyList<Resource> returnable = _reservations.GetReturnableBy(user);
        if (returnable.Count == 0)
        {
            ShowError("Não há nenhum item para devolver.");
            return;
        }

        Resource? resource = ConsoleInput.Choose(
            "Qual item deseja devolver?", returnable, r => $"{r.Description}, reservado por {r.ReservedBy}");
        if (resource is not null)
        {
            Show(_reservations.Return(resource, user));
        }
    }

    private void ShowReservations(Employee user)
    {
        IReadOnlyList<Resource> reservations = _reservations.GetReservationsOf(user);
        Console.WriteLine();
        if (reservations.Count == 0)
        {
            Console.WriteLine("Você não tem nenhuma reserva ativa.");
            return;
        }

        Console.WriteLine("Suas reservas ativas:");
        foreach (Resource resource in reservations)
        {
            Console.WriteLine($"  - {resource.Description}");
        }
    }

    private void ManageEmployees(Employee admin)
    {
        while (true)
        {
            int choice = ConsoleInput.ReadMenuOption(
                "Gerenciar funcionários", new[] { "Listar", "Cadastrar", "Remover" });

            switch (choice)
            {
                case 0:
                    return;
                case 1:
                    ListEmployees();
                    break;
                case 2:
                    RegisterEmployee(admin);
                    break;
                case 3:
                    string username = ConsoleInput.ReadText("Usuário a remover: ");
                    Show(_employees.Remove(admin, username));
                    break;
            }
        }
    }

    private void ListEmployees()
    {
        Console.WriteLine();
        foreach (Employee employee in _employees.GetAll())
        {
            string role = employee.IsAdministrator ? "Administrador" : "Professor";
            Console.WriteLine($"  {employee.Username,-15} {employee.FullName,-30} {role}");
        }
    }

    private void RegisterEmployee(Employee admin)
    {
        string username = ConsoleInput.ReadText("Usuário: ");
        string fullName = ConsoleInput.ReadText("Nome completo: ");
        string password = ConsoleInput.ReadPassword(
            $"Senha (mínimo {EmployeeService.MinimumPasswordLength} caracteres): ");
        int roleChoice = ConsoleInput.ReadInt("Função (1 - Professor, 2 - Administrador): ", 1, 2);
        Role role = roleChoice == 2 ? Role.Administrator : Role.Teacher;

        Show(_employees.Register(admin, username, fullName, password, role));
    }

    private static void Show(OperationResult result)
    {
        if (result.Success)
        {
            WriteColored(result.Message, ConsoleColor.Green);
        }
        else
        {
            ShowError(result.Message);
        }
    }

    private static void ShowError(string message) => WriteColored(message, ConsoleColor.Red);

    private static void WriteColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
