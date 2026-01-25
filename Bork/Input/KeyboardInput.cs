using SFML.System;
using SFML.Window;

namespace Bork.Input;

public class KeyboardInput
{
    public Dictionary<Keyboard.Key, TimeSpan> KeyLastPressTime { get; } = new();
    public Dictionary<Keyboard.Key, bool> KeyPulse { get; } = new();
    public Dictionary<Keyboard.Key, bool> KeyHeld { get; } = new();

    private readonly Dictionary<Keyboard.Key, TimeSpan> _keyPressTime = new();
    private readonly Dictionary<Keyboard.Key, TimeSpan> _keyLastRepeatTime = new();
    private readonly Dictionary<Keyboard.Key, bool> _previousKeyState = new();
    private readonly Clock _clock = new();

    private static readonly TimeSpan InitialRepeatDelay = TimeSpan.FromMilliseconds(500);
    private static readonly TimeSpan RepeatInterval = TimeSpan.FromMilliseconds(100);

    public void Update()
    {
        var now = TimeSpan.FromSeconds(_clock.ElapsedTime.AsSeconds());

        foreach (Keyboard.Key key in Enum.GetValues<Keyboard.Key>())
        {
            UpdateKey(key, now);
        }
    }

    private void UpdateKey(Keyboard.Key key, TimeSpan now)
    {
        KeyPulse[key] = false;
        bool isDown = Keyboard.IsKeyPressed(key);
        bool wasDown = _previousKeyState.GetValueOrDefault(key, false);
        bool justPressed = isDown && !wasDown;

        _previousKeyState[key] = isDown;
        KeyHeld[key] = isDown;

        if (justPressed)
        {
            KeyLastPressTime[key] = now;
            _keyPressTime[key] = now;
            _keyLastRepeatTime[key] = now;
            KeyPulse[key] = true;
            return;
        }

        if (!isDown)
            return;

        if (!_keyPressTime.TryGetValue(key, out var pressTime))
            return;

        var heldTime = now - pressTime;

        if (heldTime < InitialRepeatDelay)
            return;

        if (!_keyLastRepeatTime.TryGetValue(key, out var lastRepeat) || now - lastRepeat >= RepeatInterval)
        {
            _keyLastRepeatTime[key] = now;
            KeyPulse[key] = true;
        }
    }
}