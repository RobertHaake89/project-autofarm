using System;

namespace ProjectAutofarm;

class General
{
    public static void MainLoop(Terrain terrain)
    {
        Console.Clear();

        while (true)
        {
            UpdateGame(terrain);

            Renderer.Screen(terrain);

            //Console.ReadKey();
            Thread.Sleep(1000);
        }
    }

    public static void UpdateGame(Terrain terrain)
    {
        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            //if(y > 0) Console.Write("\n");
            for (int x = 0; x < Terrain.MaxSizeX; x++)
            {
                terrain.Grid![x,y].Resource.GiveGrowthChance();

                terrain.Grid![x,y].TextureRefresher();
            }
        }
    }
}