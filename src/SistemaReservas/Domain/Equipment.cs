namespace SistemaReservas.Domain;

public enum EquipmentType
{
    Notebook,
    Projector,
    Tablet
}

public sealed class Equipment : Resource
{
    public Equipment(int id, EquipmentType type, string model, string serialNumber, int voltage)
        : base(id, model)
    {
        Type = type;
        SerialNumber = serialNumber;
        Voltage = voltage;
    }

    public EquipmentType Type { get; }
    public string SerialNumber { get; }
    public int Voltage { get; }

    public override string Description => $"{Name} (série {SerialNumber}, {Voltage}V)";
}
