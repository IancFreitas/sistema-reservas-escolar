using System.Text;
using SistemaReservas.Data;
using SistemaReservas.UI;

// Garante acentuação correta também no console antigo do Windows.
Console.OutputEncoding = Encoding.UTF8;

try
{
    var app = new ConsoleApp(SchoolDatabase.CreateWithSampleData());
    app.Run();
}
catch (EndOfStreamException)
{
    // A entrada foi fechada (Ctrl+Z / Ctrl+D): encerra sem mostrar stack trace.
    Console.WriteLine();
}
