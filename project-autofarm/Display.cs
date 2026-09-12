using System;

namespace ProjectAutofarm;

class Display
{
    public static void TestScreen(Terrain terrain) // 20 is magic number, connect to terrain
    {
        for (int y = 0; y < 20; y++)
        {
            if (y % 10 == 0) Console.WriteLine("");
            for (int x = 0; x < 20; x++)
            {
                
                Console.Write(terrain.Grid[x,y].Texture.ToString());
            }
        }
    }
}