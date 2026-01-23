using System;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace Potato.Trading.Cli.UI;

public class Dashboard
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await AnsiConsole.Status()
            .StartAsync("Trading Simulation Running...", async ctx =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Update UI components
                    var table = new Table();
                    table.AddColumn("Time");
                    table.AddColumn("Symbol");
                    table.AddColumn("Price");
                    table.AddColumn("Position");
                    table.AddColumn("PnL");

                    table.AddRow(DateTime.Now.ToString("HH:mm:ss"), "2330", "600", "0", "0");
                    
                    AnsiConsole.Clear();
                    AnsiConsole.Write(new Rule("[yellow]Potato Trading Simulator[/]"));
                    AnsiConsole.Write(table);
                    
                    await Task.Delay(1000, cancellationToken);
                }
            });
    }
}
