using System;

namespace ProjectAutofarm;

class Display
{
    public static void TestScreen(Terrain terrain)
    {
        //Console.Clear();
        /* Console.WriteLine(Terrain.MaxSizeX);
        Console.WriteLine(Terrain.MaxSizeY); */

        for (int i = 0; i < Terrain.MaxSizeX; i++) Console.Write("=");
        Console.Write("\n");

        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            if(y > 0) Console.Write("\n");
            for (int x = 0; x < Terrain.MaxSizeX; x++)
            {
                
                Console.Write(terrain.Grid![x,y].Texture.Icon);
            }
        }

        Console.Write("\n");
        for (int i = 0; i < Terrain.MaxSizeX; i++) Console.Write("=");
    }
}