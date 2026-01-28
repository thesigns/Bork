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
    private readonly Clock _cursorClock;
    private readonly RectangleShape _cursorRect;
    private const float CursorBlinkRate = 0.5f;

    private int _cursorCol;
    private int _cursorRow;
    private Color _currentColor;

    public bool CursorVisible { get; set; } = true;
    public float CursorSize { get; set; } = 0.2f;

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

        _cursorCol = 0;
        _cursorRow = 0;
        _currentColor = new Color(160, 160, 160);

        _cursorClock = new Clock();
        _cursorRect = new RectangleShape();
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

        if (CursorVisible && CursorSize > 0)
        {
            float elapsed = _cursorClock.ElapsedTime.AsSeconds();
            bool cursorOn = (elapsed % 1.0f) < CursorBlinkRate;

            if (cursorOn)
            {
                float cursorHeight = CursorSize * _cellHeight;
                _cursorRect.Position = new Vector2f(
                    _cursorCol * _cellWidth,
                    (_cursorRow + 1) * _cellHeight - cursorHeight);
                _cursorRect.Size = new Vector2f(_cellWidth, cursorHeight);
                _cursorRect.FillColor = _cells[_cursorCol, _cursorRow].Text.FillColor;
                _window.Draw(_cursorRect);
            }
        }

        _window.Display();
    }

    public void SetColor(Color color)
    {
        _currentColor = color;
    }

    public void Locate(int col, int row)
    {
        _cursorCol = Math.Clamp(col, 0, _cols - 1);
        _cursorRow = Math.Clamp(row, 0, _rows - 1);
    }

    public void Clear()
    {
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                _cells[x, y].Set(' ', _currentColor);
            }
        }
        _cursorCol = 0;
        _cursorRow = 0;
    }

    public void Print(string text)
    {
        foreach (char c in text)
        {
            if (c == '\n')
            {
                _cursorCol = 0;
                _cursorRow++;
                if (_cursorRow >= _rows)
                    _cursorRow = _rows - 1;
            }
            else
            {
                if (_cursorRow < _rows && _cursorCol < _cols)
                {
                    _cells[_cursorCol, _cursorRow].Set(c, _currentColor);
                }
                _cursorCol++;
                if (_cursorCol >= _cols)
                {
                    _cursorCol = 0;
                    _cursorRow++;
                    if (_cursorRow >= _rows)
                        _cursorRow = _rows - 1;
                }
            }
        }
    }
}
