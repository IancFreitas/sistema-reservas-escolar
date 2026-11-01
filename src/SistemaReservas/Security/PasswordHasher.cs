using System.Security.Cryptography;

namespace SistemaReservas.Security;

/// <summary>
/// Gera e verifica hashes de senha com PBKDF2 (SHA-256 + salt aleatório),
/// para que nenhuma senha fique guardada em texto puro.
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    /// <returns>Texto no formato <c>iterações.salt.hash</c>, com salt e hash em Base64.</returns>
    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public static bool Verify(string password, string storedHash)
    {
        string[] parts = storedHash.Split('.', 3);
        if (parts.Length != 3 || !int.TryParse(parts[0], out int iterations))
        {
            return false;
        }

        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] expected = Convert.FromBase64String(parts[2]);
        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expected.Length);

        // Comparação em tempo constante para não vazar informação por timing.
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
