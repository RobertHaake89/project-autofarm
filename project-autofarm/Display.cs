using System;

namespace ProjectAutofarm;

class Display
{
    public static void TestScreen(Terrain terrain)
    {
        for (int y = 0; y < terrain.MaxSizeX; y++)
        {
            for (int x = 0; x < terrain.GetMaxSize().yMax; x++)
            {
                if (x == terrain.GetMaxSize().xMax) Console.WriteLine();
                Console.Write(terrain.Grid[x,y].Texture);
            }
        }
    }
}