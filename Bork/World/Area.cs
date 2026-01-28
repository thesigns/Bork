using System.Text;

namespace Bork.World;

public class Area : Component
{
    public Area(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string GetFullDescription()
    {
        var sb = new StringBuilder();

        sb.AppendLine(Name);
        sb.AppendLine(new string('-', Name.Length));
        sb.AppendLine(Description);

        var visibleItems = Children
            .Where(c => c.GetType() != typeof(Player) && c.GetType() != typeof(Exit))
            .ToList();

        if (visibleItems.Count > 0)
        {
            sb.AppendLine();
            sb.Append("You can see: ");
            sb.AppendLine(string.Join(", ", visibleItems.Select(c => c.Name)));
        }

        var exits = FindChildren<Exit>().ToList();
        if (exits.Count > 0)
        {
            sb.AppendLine();
            sb.Append("Exits: ");
            sb.AppendLine(string.Join(", ", exits.Select(e => e.Direction)));
        }

        return sb.ToString().TrimEnd();
    }
}
