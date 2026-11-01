namespace SistemaReservas.Domain;

public sealed class Room : Resource
{
    public Room(int id, string name, int number)
        : base(id, name)
    {
        Number = number;
    }

    public int Number { get; }

    public override string Description => $"{Name} (sala {Number})";
}
