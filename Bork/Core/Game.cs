using Bork.Input;
using Bork.Output;
using SFML.Graphics;

namespace Bork.Core;

public class Game
{
    private Terminal _output;
    private KeyboardInput _input;
    
    public bool ShouldQuit => !_output.IsOpen;
    
    public Game(Terminal output, KeyboardInput input)
    {
        _output = output;
        _input = input;
        
        _output.SetColor(Color.Green);
        _output.Clear();
        _output.SetColor(Color.White);
        _output.Locate(10, 5);
        _output.Print("Hello Bork!\nLinia 2");
        
    }
    
    
    public void Update()
    {
        _input.Update();
    }

    public void Render()
    {
        _output.Update();
    }
}