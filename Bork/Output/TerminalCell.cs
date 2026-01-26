using SFML.Graphics;

namespace Bork.Output;

public struct TerminalCell
{
    public Text Text { get; }

    public TerminalCell(Font font, uint charSize)
    {
        Text = new Text(font, " ", charSize);
    }

    public void Set(char character, Color color)
    {
        Text.DisplayedString = character.ToString();
        Text.FillColor = color;
    }

    public void SetCharacter(char character)
    {
        Text.DisplayedString = character.ToString();
    }

    public void SetColor(Color color)
    {
        Text.FillColor = color;
    }
}
