using Bork.Input;
using Bork.Output;
using Bork.World;
using SFML.Graphics;
using SFML.System;

namespace Bork.Core;

public class Game
{
    private Terminal _output;
    private KeyboardInput _input;
    private Prompt _prompt;
    private World.World _world;

    private readonly Clock _progressClock = new();
    private float _lastProgressTime;
    private float _lastInputTime;
    private int _turnCount;

    private const float ProgressInterval = 1.0f;
    private const float ProgressPauseAfterInput = 5.0f;

    private readonly List<string> _outputLines = new();
    private int _scrollOffset;

    public bool ShouldQuit => !_output.IsOpen;

    public Game(Terminal output, KeyboardInput input)
    {
        _output = output;
        _input = input;
        _prompt = new Prompt();
        _prompt.OnSubmit += OnPromptSubmit;

        _world = CreateWorld();

        var initialDescription = _world.Player.CurrentArea?.GetFullDescription();
        if (initialDescription != null)
            AddOutput(initialDescription);

        UpdateHints();
    }

    private World.World CreateWorld()
    {
        var world = new World.World();

        var entrance = new Area("Cave Entrance",
            "You stand at the mouth of a dark cave. Cool air drifts from within.");
        var tunnel = new Area("Narrow Tunnel",
            "The passage narrows here. The walls are damp with moisture.");
        var chamber = new Area("Underground Chamber",
            "A vast chamber opens before you. Stalactites hang from the ceiling.");

        entrance.AddChild(new Exit("north", tunnel));
        tunnel.AddChild(new Exit("south", entrance));
        tunnel.AddChild(new Exit("east", chamber));
        chamber.AddChild(new Exit("west", tunnel));

        world.AddChild(entrance);
        world.AddChild(tunnel);
        world.AddChild(chamber);

        entrance.AddChild(world.Player);

        return world;
    }

    private void OnPromptSubmit(string command)
    {
        _lastInputTime = _progressClock.ElapsedTime.AsSeconds();

        AddOutput($">> {command}");

        var responses = _world.ProcessCommand(command);

        if (responses.Count == 0)
        {
            AddOutput("I don't understand that command.");
        }
        else
        {
            foreach (var response in responses)
            {
                AddOutput(response);
            }
        }

        UpdateHints();
    }

    private void UpdateHints()
    {
        _prompt.ClearHints();
        foreach (var hint in _world.GetCommandHints())
        {
            _prompt.AddHint(hint);
        }
    }

    private void AddOutput(string text)
    {
        foreach (var line in text.Split('\n'))
        {
            _outputLines.Add(line);
        }

        var maxVisibleLines = _output.Rows - 1;
        if (_outputLines.Count > maxVisibleLines)
        {
            _scrollOffset = _outputLines.Count - maxVisibleLines;
        }
    }

    public void Update()
    {
        _input.Update();

        // Detect any key press to pause progress
        foreach (var key in _input.KeyPulse.Keys)
        {
            if (_input.KeyPulse[key])
            {
                _lastInputTime = _progressClock.ElapsedTime.AsSeconds();
                break;
            }
        }

        _prompt.Update(_input, _output.Cols - Prompt.PrefixLength);

        float now = _progressClock.ElapsedTime.AsSeconds();
        float timeSinceInput = now - _lastInputTime;
        float timeSinceProgress = now - _lastProgressTime;

        if (timeSinceProgress >= ProgressInterval && timeSinceInput >= ProgressPauseAfterInput)
        {
            _lastProgressTime = now;
            _turnCount++;

            var responses = _world.ProcessProgress(_turnCount);
            foreach (var response in responses)
            {
                AddOutput(response);
            }
        }
    }

    public void Render()
    {
        _output.Clear();

        _output.SetColor(new Color(200, 200, 200));
        _output.Locate(0, 0);

        int maxVisibleLines = _output.Rows - 1;
        int startLine = _scrollOffset;
        int endLine = Math.Min(startLine + maxVisibleLines, _outputLines.Count);

        for (int i = startLine; i < endLine; i++)
        {
            _output.Print(_outputLines[i]);
            _output.Print("\n");
        }

        _prompt.Render(_output);
        _output.Update();
    }
}
