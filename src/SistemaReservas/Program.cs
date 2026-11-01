using SistemaReservas.Data;
using SistemaReservas.UI;

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
