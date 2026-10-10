using System;
using System.Text;

namespace ProjectAutofarm;

class General
{
    private static int _isUpdating = 0;
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
            UpdateTiles(terrain);
            await UpdateGameAsync(terrain);
            Renderer.Screen(terrain, entityDict);

            Console.Write($"{entityDict["human2"].Position}, {entityDict["human2"].Status}");
            

            //Console.ReadKey();
            await Task.Delay(100); // 60
        }
    }

    public static void UpdateTiles(Terrain terrain)
    {
        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            for (int x = 0; x < Terrain.MaxSizeX; x++)
            {
                terrain.Grid![x,y].Resource.GiveGrowthChance();

                terrain.Grid![x,y].UpdateAcre();
                //terrain.Grid![x,y].UpdateAcre();

            }
        }

    }

    public static async Task UpdateGameAsync(Terrain terrain)
    {
        if (Interlocked.Exchange(ref _isUpdating, 1) == 1)
            return;

        try
        {
            foreach (Tree tree in terrain.TreeList.ToList())
            {
                await tree.GiveGrowthChance();
                await tree.SelfSeeding(terrain, randomFactor: 900);
            }
                
            
            //await Task.Delay(1000);
        }
        finally
        {
            Volatile.Write(ref _isUpdating, 0);
            terrain.TreeList.Sort((a,b) => a.Position.Y.CompareTo(a.Position.Y));
        }
    }
}