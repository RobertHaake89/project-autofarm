using System;

namespace ProjectAutofarm;

class General
{
    public static async Task MainLoop(Terrain terrain, Dictionary<string, Entity> entityDict)
    {
        Console.Clear();
        var tasks = new List <Task>();
        
        foreach (Entity entity in entityDict.Values)
        {
            tasks.Add(entity.RunSchedule(terrain));
        }

        while (true)
        {
            await UpdateGame(terrain, entityDict);

            Renderer.Screen(terrain, entityDict);

            //Console.ReadKey();
            await Task.Delay(60);
        }
        //await Task.WhenAll(tasks);
    }

    public static async Task UpdateGame(Terrain terrain, Dictionary<string, Entity> entityDict)
    {
        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            for (int x = 0; x < Terrain.MaxSizeX; x++)
            {
                terrain.Grid![x,y].Resource.GiveGrowthChance();

                terrain.Grid![x,y].UpdateAcre();
            }
        }
        //entityDict["human1"].RunSchedule(terrain);
        //Thread.Sleep(2000);

    }
}