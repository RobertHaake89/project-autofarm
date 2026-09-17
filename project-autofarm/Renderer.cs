using System;

namespace ProjectAutofarm;

class Renderer
{
    public static void Screen(Terrain terrain)
    {
        //Console.Clear();
        Console.SetCursorPosition(0, 0);
        /* Console.WriteLine(Terrain.MaxSizeX);
        Console.WriteLine(Terrain.MaxSizeY); */

        for (int i = 0; i < Terrain.MaxSizeX; i++) Console.Write("=");
        Console.Write("\n");

        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            if(y > 0) Console.Write("\n");
            for (int x = 0; x < Terrain.MaxSizeX; x++)
            {
                //terrain.Grid![x,y].TextureRefresher();
                Console.Write(terrain.Grid![x,y].Texture.Icon);
            }
        }

        Console.Write("\n");
        for (int i = 0; i < Terrain.MaxSizeX; i++) Console.Write("=");
    }
}