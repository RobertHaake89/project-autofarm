using System.Dynamic;

namespace ProjectAutofarm;

class General
{
    public static readonly object EntityListLock = new();
    public static event EventHandler? OnCropsAreRipe;
    private static int _isUpdating = 0;
    public static async Task MainLoop(Terrain terrain)
    {
        Console.Clear();
        
        lock (EntityListLock)
        {
            foreach (Entity entity in terrain.EntityDict.Values)
            {
                _ = entity.RunSchedule(terrain);
            }
        }
        

        while (true)
        {
            UpdateTiles(terrain);

            await UpdateGameAsync(terrain);
            Renderer.Screen(terrain);
            
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

        if (terrain.CropTileList.Count > 0
        && terrain.CropTileList.All(
        crop => crop.Resource.Status is GrowthProcess.Ripe))
        {
            lock (EntityListLock)
            {
                foreach(Human human in terrain.EntityDict.Values.OfType<Human>())
                {
                    if (human.Profession is Profession.Farmer
                    && human.Status is Status.Idle)
                        OnCropsAreRipe?.Invoke(terrain, EventArgs.Empty);
                }
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
            }

            foreach (Bush bush in terrain.BushList.ToList())
            {
                await bush.GiveGrowthChance(terrain);
                await bush.SelfSeeding(terrain, randomFactor: 1000); // 1000
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