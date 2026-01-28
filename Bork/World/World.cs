namespace Bork.World;

public class World : Component
{
    public Player Player { get; }

    public World()
    {
        Name = "World";
        Description = "The game world.";
        Player = new Player();
    }

    public List<string> ProcessCommand(string rawCommand)
    {
        var responses = new List<string>();
        ProcessCommandRecursive(this, rawCommand, responses);
        return responses;
    }

    private void ProcessCommandRecursive(Component component, string rawCommand, List<string> responses)
    {
        var response = component.OnCommand(rawCommand);
        if (response != null)
            responses.Add(response);

        foreach (var child in component.Children.ToList())
        {
            ProcessCommandRecursive(child, rawCommand, responses);
        }
    }

    public List<string> ProcessProgress(int elapsedTurns)
    {
        var responses = new List<string>();
        ProcessProgressRecursive(this, elapsedTurns, responses);
        return responses;
    }

    private void ProcessProgressRecursive(Component component, int elapsedTurns, List<string> responses)
    {
        var response = component.OnProgress(elapsedTurns);
        if (response != null)
            responses.Add(response);

        foreach (var child in component.Children.ToList())
        {
            ProcessProgressRecursive(child, elapsedTurns, responses);
        }
    }

    public IEnumerable<string> GetCommandHints()
    {
        var hints = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "look", "l", "help", "quit"
        };

        if (Player.CurrentArea != null)
        {
            foreach (var exit in Player.CurrentArea.FindChildren<Exit>())
            {
                hints.Add(exit.Direction);
                hints.Add($"go {exit.Direction}");
            }

            foreach (var item in Player.CurrentArea.Children.Where(c => c.GetType() != typeof(Player) && c.GetType() != typeof(Exit)))
            {
                hints.Add($"look at {item.Name}");
                hints.Add($"examine {item.Name}");
            }
        }

        return hints;
    }
}
