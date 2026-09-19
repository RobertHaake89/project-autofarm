using System;
using System.Text;

namespace ProjectAutofarm;

class Renderer
{
    public static void Screen(Terrain terrain)
    {
        Console.SetCursorPosition(0, 0);

        StringBuilder output = new StringBuilder();

        for (int x = 0; x < Terrain.MaxSizeX; x++) output.Append('=');
        output.AppendLine();

        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            for (int x = 0; x < Terrain.MaxSizeX; x++)
            {
                output.Append(terrain.Grid![x,y].Texture.Icon);
            }
            output.AppendLine();
        }

        for (int x = 0; x < Terrain.MaxSizeX; x++) output.Append('=');
        output.AppendLine();

        Console.Write(output);
    }
}