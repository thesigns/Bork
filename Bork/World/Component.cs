namespace Bork.World;

public abstract class Component
{
    public Component? Parent { get; private set; }
    private readonly List<Component> _children = new();
    public IReadOnlyList<Component> Children => _children;

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public void AddChild(Component child)
    {
        child.Parent?.RemoveChild(child);
        child.Parent = this;
        _children.Add(child);
    }

    public void RemoveChild(Component child)
    {
        if (_children.Remove(child))
            child.Parent = null;
    }

    public void MoveTo(Component newParent)
    {
        newParent.AddChild(this);
    }

    public virtual string? OnCommand(string rawCommand) => null;

    public virtual string? OnProgress(int elapsedTurns) => null;

    public IEnumerable<Component> Siblings =>
        Parent?.Children.Where(c => c != this) ?? Enumerable.Empty<Component>();

    public Component? FindSibling(string name) =>
        Siblings.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public Component? FindChild(string name) =>
        _children.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public T? FindChild<T>() where T : Component =>
        _children.OfType<T>().FirstOrDefault();

    public IEnumerable<T> FindChildren<T>() where T : Component =>
        _children.OfType<T>();

    public Component Root
    {
        get
        {
            var current = this;
            while (current.Parent != null)
                current = current.Parent;
            return current;
        }
    }

    public T? FindInTree<T>() where T : Component =>
        Root.FindDescendant<T>();

    public T? FindDescendant<T>() where T : Component
    {
        if (this is T t) return t;
        foreach (var child in _children)
        {
            var found = child.FindDescendant<T>();
            if (found != null) return found;
        }
        return null;
    }

    public IEnumerable<T> FindDescendants<T>() where T : Component
    {
        if (this is T t) yield return t;
        foreach (var child in _children)
        foreach (var desc in child.FindDescendants<T>())
            yield return desc;
    }
}
