namespace Bork.World;

public class Player : Component
{
    public Player()
    {
        Name = "Player";
        Description = "That's you.";
    }

    public Area? CurrentArea => Parent as Area;

    public override string? OnCommand(string rawCommand)
    {
        var cmd = rawCommand.Trim().ToLowerInvariant();

        if (cmd == "look" || cmd == "l")
        {
            return CurrentArea?.GetFullDescription() ?? "You are nowhere.";
        }

        if (cmd.StartsWith("look at "))
        {
            var target = cmd.Substring(8).Trim();
            return ExamineTarget(target);
        }

        if (cmd.StartsWith("examine "))
        {
            var target = cmd.Substring(8).Trim();
            return ExamineTarget(target);
        }

        return null;
    }

    private string ExamineTarget(string targetName)
    {
        var target = Siblings
            .FirstOrDefault(c => c.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));

        if (target != null)
            return target.Description;

        return $"You don't see any {targetName} here.";
    }
}
