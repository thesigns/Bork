using Bork.Output;
using SFML.Graphics;

namespace Bork;

internal static class Program
{
    private static void Main()
    {

        var font = new Font("Assets/Fonts/CascadiaCode-Regular.ttf");

        
        var terminal = new Terminal(80, 25, font, 0, false, "Bork");

        while (terminal.IsOpen)
        {
            terminal.Update();
        }
        

    }
}