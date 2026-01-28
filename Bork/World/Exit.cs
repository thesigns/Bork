namespace Bork.World;

public class Exit : Component
{
    public string Direction { get; }
    public Area Target { get; }

    private static readonly Dictionary<string, string> DirectionAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = "north",
        ["s"] = "south",
        ["e"] = "east",
        ["w"] = "west",
        ["u"] = "up",
        ["d"] = "down",
        ["ne"] = "northeast",
        ["nw"] = "northwest",
        ["se"] = "southeast",
        ["sw"] = "southwest"
    };

    public Exit(string direction, Area target)
    {
        Direction = direction.ToLowerInvariant();
        Target = target;
        Name = $"Exit {Direction}";
        Description = $"An exit leading {Direction}.";
    }

    public override string? OnCommand(string rawCommand)
    {
        var cmd = rawCommand.Trim().ToLowerInvariant();

        string? normalizedDir = null;

        if (cmd.StartsWith("go ") || cmd.StartsWith("walk "))
        {
            normalizedDir = cmd.Substring(cmd.IndexOf(' ') + 1).Trim();
        }
        else if (cmd == Direction || DirectionAliases.GetValueOrDefault(cmd) == Direction)
        {
            normalizedDir = Direction;
        }

        if (normalizedDir != null)
        {
            if (DirectionAliases.TryGetValue(normalizedDir, out var expanded))
                normalizedDir = expanded;

            if (normalizedDir == Direction)
            {
                return MovePlayer();
            }
        }

        return null;
    }

    private string? MovePlayer()
    {
        var player = Parent?.FindChild<Player>();
        if (player == null)
            return null;

        player.MoveTo(Target);

        return $"You go {Direction}.\n\n{Target.GetFullDescription()}";
    }
}
