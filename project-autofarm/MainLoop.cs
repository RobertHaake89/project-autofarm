using System;

namespace ProjectAutofarm;

class General
{
    public static async Task MainLoop(Terrain terrain, Dictionary<string, Entity> entityDict)
    {
        Console.Clear();
        //var tasks = new List <Task>();
        
        foreach (Entity entity in entityDict.Values)
        {
            _ = entity.RunSchedule(terrain);
        }

        while (true)
        {
            await UpdateGame(terrain);

            Renderer.Screen(terrain, entityDict);

            //Console.ReadKey();
            await Task.Delay(100); // 60
        }
        //await Task.WhenAll(tasks);
    }

    public static async Task UpdateGame(Terrain terrain)
    {
        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            for (int x = 0; x < Terrain.MaxSizeX; x++)
            {
                terrain.Grid![x,y].Resource.GiveGrowthChance();

                terrain.Grid![x,y].UpdateAcre();
            }
        }

    }
}