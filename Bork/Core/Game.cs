using Bork.Input;
using Bork.Output;
using SFML.Graphics;

namespace Bork.Core;

public class Game
{
    private Terminal _output;
    private KeyboardInput _input;
    private Prompt _prompt;

    public bool ShouldQuit => !_output.IsOpen;

    public Game(Terminal output, KeyboardInput input)
    {
        _output = output;
        _input = input;
        _prompt = new Prompt();
        _prompt.OnSubmit += OnPromptSubmit;

        // Test hints
        _prompt.AddHint("look");
        _prompt.AddHint("look around");
        _prompt.AddHint("go north");
        _prompt.AddHint("go south");
        _prompt.AddHint("go east");
        _prompt.AddHint("go west");
        _prompt.AddHint("take");
        _prompt.AddHint("inventory");
        _prompt.AddHint("help");
    }

    private void OnPromptSubmit(string command)
    {
        Console.WriteLine($"Command: {command}");
    }

    public void Update()
    {
        _input.Update();
        _prompt.Update(_input, _output.Cols - Prompt.PrefixLength);
    }

    public void Render()
    {
        _prompt.Render(_output);
        _output.Update();
    }
}