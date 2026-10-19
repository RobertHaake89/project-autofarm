namespace ProjectAutofarm;

class General
{
    private static int _isUpdating = 0;
    public static async Task MainLoop(Terrain terrain, Dictionary<string, Entity> entityDict)
    {
        Console.Clear();
        
        foreach (Entity entity in entityDict.Values)
        {
            _ = entity.RunSchedule(terrain);
        }

        while (true)
        {
            UpdateTiles(terrain);
            await UpdateGameAsync(terrain);
            Renderer.Screen(terrain, entityDict);
            
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
                await tree.GiveGrowthChance(terrain);
                await tree.SelfSeeding(terrain, randomFactor: 1000); // 1000
                await tree.CreateMycel(terrain, randomFactor: 200);

                if (tree.Mycel is not null)
                    foreach (Mushroom mushroom in tree.Mycel.MushroomList)
                        await mushroom.GiveGrowthChance(randomFactor: 100);

                if (tree.Mycel is not null)
                {
                    await tree.Mycel.AddMushrooms(terrain, randomFactor: 100);
                }

                //terrain.Grid![tree.Position.X, tree.Position.Y].Resource.Type = tree.Resource;
                //terrain.Grid![tree.Position.X, tree.Position.Y].Resource.Status = tree.Status;
            }
        }
        finally
        {
            Volatile.Write(ref _isUpdating, 0);
            terrain.TreeList.Sort((a,b) =>
            {
                int yComparison = a.Position.Y.CompareTo(b.Position.Y);

                if (yComparison != 0)
                    return yComparison;

                return a.Position.X.CompareTo(b.Position.X);
            });
                
        }
    }
}