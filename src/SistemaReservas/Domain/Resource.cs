namespace SistemaReservas.Domain;

/// <summary>
/// Qualquer item que pode ser reservado por um funcionário (sala ou equipamento).
/// </summary>
public abstract class Resource
{
    protected Resource(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }
    public string Name { get; }

    /// <summary>Usuário que está com o item reservado, ou <c>null</c> se ele estiver livre.</summary>
    public string? ReservedBy { get; private set; }

    public bool IsReserved => ReservedBy is not null;

    public abstract string Description { get; }

    internal void Reserve(string username) => ReservedBy = username;

    internal void Release() => ReservedBy = null;
}
