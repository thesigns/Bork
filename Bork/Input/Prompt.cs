using Bork.Output;
using SFML.Graphics;
using SFML.Window;

namespace Bork.Input;

public class Prompt
{
    private const string Prefix = ">> ";
    public static int PrefixLength => Prefix.Length;

    private string _text = "";
    private int _cursorPosition = 0;

    private readonly List<string> _history = new();
    private int _historyIndex = -1;
    private string _savedText = "";

    public string Text => _text;
    public event Action<string>? OnSubmit;

    public void Update(KeyboardInput input, int maxLength)
    {
        // Navigation keys
        if (input.KeyPulse.GetValueOrDefault(Keyboard.Key.Left) && _cursorPosition > 0)
            _cursorPosition--;

        if (input.KeyPulse.GetValueOrDefault(Keyboard.Key.Right) && _cursorPosition < _text.Length)
            _cursorPosition++;

        if (input.KeyPulse.GetValueOrDefault(Keyboard.Key.Home))
            _cursorPosition = 0;

        if (input.KeyPulse.GetValueOrDefault(Keyboard.Key.End))
            _cursorPosition = _text.Length;

        // History navigation
        if (input.KeyPulse.GetValueOrDefault(Keyboard.Key.Up) && _history.Count > 0)
        {
            if (_historyIndex == -1)
            {
                _savedText = _text;
                _historyIndex = _history.Count - 1;
            }
            else if (_historyIndex > 0)
            {
                _historyIndex--;
            }
            _text = _history[_historyIndex];
            _cursorPosition = _text.Length;
        }

        if (input.KeyPulse.GetValueOrDefault(Keyboard.Key.Down))
        {
            if (_historyIndex >= 0)
            {
                _historyIndex++;
                if (_historyIndex >= _history.Count)
                {
                    _historyIndex = -1;
                    _text = _savedText;
                }
                else
                {
                    _text = _history[_historyIndex];
                }
                _cursorPosition = _text.Length;
            }
        }

        // Deletion keys
        if (input.KeyPulse.GetValueOrDefault(Keyboard.Key.Backspace) && _cursorPosition > 0)
        {
            _text = _text.Remove(_cursorPosition - 1, 1);
            _cursorPosition--;
        }

        if (input.KeyPulse.GetValueOrDefault(Keyboard.Key.Delete) && _cursorPosition < _text.Length)
        {
            _text = _text.Remove(_cursorPosition, 1);
        }

        // Submit
        if (input.KeyPulse.GetValueOrDefault(Keyboard.Key.Enter))
        {
            if (_text.Length > 0)
            {
                _history.Add(_text);
                OnSubmit?.Invoke(_text);
            }
            _text = "";
            _cursorPosition = 0;
            _historyIndex = -1;
            _savedText = "";
            return;
        }

        // Character input (only if not at max length)
        if (_text.Length >= maxLength)
            return;

        bool shift = input.KeyHeld.GetValueOrDefault(Keyboard.Key.LShift) ||
                     input.KeyHeld.GetValueOrDefault(Keyboard.Key.RShift);

        foreach (var key in Enum.GetValues<Keyboard.Key>())
        {
            if (input.KeyPulse.GetValueOrDefault(key))
            {
                char? c = KeyToChar(key, shift);
                if (c.HasValue)
                {
                    _text = _text.Insert(_cursorPosition, c.Value.ToString());
                    _cursorPosition++;
                }
            }
        }
    }

    public void Render(Terminal terminal)
    {
        int row = terminal.Rows - 1;

        // Clear the prompt line
        terminal.Locate(0, row);
        terminal.SetColor(Color.Black);
        terminal.Print(new string(' ', terminal.Cols));

        // Draw prefix
        terminal.Locate(0, row);
        terminal.SetColor(Color.Green);
        terminal.Print(Prefix);

        // Draw text
        terminal.SetColor(Color.White);
        terminal.Print(_text);

        // Position cursor
        terminal.Locate(Prefix.Length + _cursorPosition, row);
    }

    private static char? KeyToChar(Keyboard.Key key, bool shift)
    {
        // Letters A-Z
        if (key >= Keyboard.Key.A && key <= Keyboard.Key.Z)
        {
            char c = (char)('a' + (key - Keyboard.Key.A));
            return shift ? char.ToUpper(c) : c;
        }

        // Numbers 0-9 (top row)
        if (key >= Keyboard.Key.Num0 && key <= Keyboard.Key.Num9)
        {
            if (shift)
            {
                return (key - Keyboard.Key.Num0) switch
                {
                    1 => '!',
                    2 => '@',
                    3 => '#',
                    4 => '$',
                    5 => '%',
                    6 => '^',
                    7 => '&',
                    8 => '*',
                    9 => '(',
                    0 => ')',
                    _ => null
                };
            }
            return (char)('0' + (key - Keyboard.Key.Num0));
        }

        // Numpad 0-9
        if (key >= Keyboard.Key.Numpad0 && key <= Keyboard.Key.Numpad9)
        {
            return (char)('0' + (key - Keyboard.Key.Numpad0));
        }

        // Special characters
        return key switch
        {
            Keyboard.Key.Space => ' ',
            Keyboard.Key.Period => shift ? '>' : '.',
            Keyboard.Key.Comma => shift ? '<' : ',',
            Keyboard.Key.Semicolon => shift ? ':' : ';',
            Keyboard.Key.Apostrophe => shift ? '"' : '\'',
            Keyboard.Key.Slash => shift ? '?' : '/',
            Keyboard.Key.Backslash => shift ? '|' : '\\',
            Keyboard.Key.Equal => shift ? '+' : '=',
            Keyboard.Key.Hyphen => shift ? '_' : '-',
            Keyboard.Key.LBracket => shift ? '{' : '[',
            Keyboard.Key.RBracket => shift ? '}' : ']',
            Keyboard.Key.Grave => shift ? '~' : '`',
            _ => null
        };
    }
}
