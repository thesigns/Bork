using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Bork.Output;

public class Terminal
{
    private readonly RenderWindow _window;
    private readonly TerminalCell[,] _cells;
    private readonly int _cols;
    private readonly int _rows;
    private readonly uint _charSize;
    private readonly float _cellWidth;
    private readonly float _cellHeight;
    private readonly Font _font;

    public RenderWindow Window => _window;
    public bool IsOpen => _window.IsOpen;

    public Terminal(int cols, int rows, Font font, uint charSize, bool fullscreen, string title)
    {
        _cols = cols;
        _rows = rows;
        _font = font;
        _charSize = charSize == 0 ? CalculateOptimalCharSize(font, cols, rows) : charSize;

        var glyph = font.GetGlyph('▒', _charSize, false, 0);
        _cellWidth = glyph.Bounds.Width;
        _cellHeight = glyph.Bounds.Height;

        var mode = new VideoMode(new Vector2u((uint)(cols * _cellWidth), (uint)(rows * _cellHeight)));
        _window = new RenderWindow(mode, title, Styles.Close, fullscreen ? State.Fullscreen : State.Windowed);
        _window.SetFramerateLimit(60);
        _window.Closed += OnTerminalClosed;

        _cells = new TerminalCell[cols, rows];
        InitializeCells();
    }

    private static uint CalculateOptimalCharSize(Font font, int cols, int rows)
    {
        var desktopSize = VideoMode.DesktopMode.Size;
        var maxWidth = desktopSize.X - (desktopSize.X / 3);
        var maxHeight = desktopSize.Y - (desktopSize.Y / 3);

        uint lastValidSize = 1;
        for (uint size = 1; ; size++)
        {
            var glyph = font.GetGlyph('▒', size, false, 0);
            var windowWidth = cols * glyph.Bounds.Width;
            var windowHeight = rows * glyph.Bounds.Height;

            if (windowWidth > maxWidth || windowHeight > maxHeight)
                return lastValidSize;

            lastValidSize = size;
        }
    }

    private void InitializeCells()
    {
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                _cells[x, y] = new TerminalCell(_font, _charSize);
                _cells[x, y].Text.Position = new Vector2f(x * _cellWidth, y * _cellHeight);
            }
        }
    }

    private void OnTerminalClosed(object? sender, EventArgs e)
    {
        _window.Close();
    }

    public void Update()
    {
        _window.DispatchEvents();

        _window.Clear();

        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                _window.Draw(_cells[x, y].Text);
            }
        }

        _window.Display();
    }
}
