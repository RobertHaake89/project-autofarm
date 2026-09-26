using System;

namespace ProjectAutofarm;

class General
{
    public static void MainLoop(Terrain terrain, Dictionary<string, Entity> entityDict)
    {
        Console.Clear();

        while (true)
        {
            UpdateGame(terrain, entityDict);

            Renderer.Screen(terrain, entityDict);

            //Console.ReadKey();
            Thread.Sleep(1000);
        }
    }

    public static void UpdateGame(Terrain terrain, Dictionary<string, Entity> entityDict)
    {
        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            for (int x = 0; x < Terrain.MaxSizeX; x++)
            {
                terrain.Grid![x,y].Resource.GiveGrowthChance();

                terrain.Grid![x,y].TextureRefresher();
            }
        }
        entityDict["human1"].RunSchedule(terrain);
        //Thread.Sleep(2000);

    }
}