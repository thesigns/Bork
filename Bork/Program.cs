using Bork.Core;
using Bork.Input;
using Bork.Output;
using SFML.Graphics;

namespace Bork;

internal static class Program
{
    private static void Main()
    {

        var font = new Font("Assets/Fonts/CascadiaCode-Regular.ttf");
        
        var output = new Terminal(80, 25, font, 0, false, "Bork");
        var input = new KeyboardInput();
        var game = new Game(output, input);

        while (!game.ShouldQuit)
        {
            game.Update();
            game.Render();
        }

    }
}