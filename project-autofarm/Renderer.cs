using System;

namespace ProjectAutofarm;

class Display
{
    public static void TestScreen(Terrain terrain)
    {
        /* Console.WriteLine(Terrain.MaxSizeX);
        Console.WriteLine(Terrain.MaxSizeY); */

        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            Console.WriteLine("");
            for (int x = 0; x < Terrain.MaxSizeX; x++)
            {
                
                Console.Write(terrain.Grid[x,y].Texture.Icon);
            }
        }
    }
}