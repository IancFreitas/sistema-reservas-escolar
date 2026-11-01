using SistemaReservas.Data;
using SistemaReservas.Security;
using SistemaReservas.Services;

namespace SistemaReservas.Tests;

public class AuthenticationTests
{
    private readonly AuthService _auth = new(SchoolDatabase.CreateWithSampleData());

    [Fact]
    public void Login_SucceedsWithCorrectCredentials()
    {
        Assert.NotNull(_auth.Login("admin", "admin123"));
    }

    [Theory]
    [InlineData("admin", "senha-errada")]
    [InlineData("nao-existe", "admin123")]
    [InlineData("", "")]
    public void Login_FailsWithWrongCredentials(string username, string password)
    {
        Assert.Null(_auth.Login(username, password));
    }

    [Fact]
    public void PasswordHasher_NeverStoresPlainTextAndUsesRandomSalt()
    {
        string first = PasswordHasher.Hash("minha-senha");
        string second = PasswordHasher.Hash("minha-senha");

        Assert.DoesNotContain("minha-senha", first);
        Assert.NotEqual(first, second);
        Assert.True(PasswordHasher.Verify("minha-senha", first));
        Assert.True(PasswordHasher.Verify("minha-senha", second));
        Assert.False(PasswordHasher.Verify("outra-senha", first));
    }

    [Fact]
    public void PasswordHasher_RejectsMalformedHash()
    {
        Assert.False(PasswordHasher.Verify("qualquer", "formato-invalido"));
    }
}
