using System.Text;

namespace SistemaReservas.UI;

/// <summary>
/// Leitura de dados do console com validação, para que uma entrada inválida
/// nunca derrube o programa.
/// </summary>
internal static class ConsoleInput
{
    /// <returns>O número da opção escolhida (1..N), ou 0 para a opção de saída.</returns>
    public static int ReadMenuOption(string title, IReadOnlyList<string> options, string exitLabel = "Voltar")
    {
        Console.WriteLine();
        Console.WriteLine($"--- {title} ---");
        for (int i = 0; i < options.Count; i++)
        {
            Console.WriteLine($"  {i + 1} - {options[i]}");
        }
        Console.WriteLine($"  0 - {exitLabel}");

        return ReadInt("Escolha uma opção: ", 0, options.Count);
    }

    /// <returns>O item escolhido, ou <c>null</c> se o usuário escolher voltar.</returns>
    public static T? Choose<T>(string title, IReadOnlyList<T> items, Func<T, string> describe)
        where T : class
    {
        int choice = ReadMenuOption(title, items.Select(describe).ToList());
        return choice == 0 ? null : items[choice - 1];
    }

    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            string input = ReadText(prompt);
            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Opção inválida. Digite um número entre {min} e {max}.");
        }
    }

    public static string ReadText(string prompt)
    {
        Console.Write(prompt);
        string? line = Console.ReadLine();
        if (line is null)
        {
            throw new EndOfStreamException("A entrada do console foi encerrada.");
        }

        return line.Trim();
    }

    /// <summary>Lê uma senha mostrando <c>*</c> no lugar dos caracteres digitados.</summary>
    public static string ReadPassword(string prompt)
    {
        if (Console.IsInputRedirected)
        {
            return ReadText(prompt);
        }

        Console.Write(prompt);
        var password = new StringBuilder();
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return password.ToString();
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Length--;
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write('*');
            }
        }
    }
}
