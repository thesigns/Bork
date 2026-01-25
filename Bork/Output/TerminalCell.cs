using SFML.Graphics;

namespace Bork.Output;

public struct TerminalCell
{
    public Text Text { get; }

    public TerminalCell(Font font, uint charSize)
    {
        Text = new Text(font, "#", charSize);
    }
}
