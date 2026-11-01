namespace SistemaReservas.Domain;

/// <summary>
/// Resultado de uma operação de negócio, com a mensagem que deve ser mostrada ao usuário.
/// </summary>
public sealed record OperationResult(bool Success, string Message)
{
    public static OperationResult Ok(string message) => new(true, message);

    public static OperationResult Fail(string message) => new(false, message);
}
