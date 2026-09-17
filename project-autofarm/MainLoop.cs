using System;

namespace ProjectAutofarm;

class General
{
    public static void MainLoop(Terrain terrain)
    {
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
        /* for (int i = 0; i < Terrain.MaxSizeX; i++) Console.Write("=");
        Console.Write("\n"); */

        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            if(y > 0) Console.Write("\n");
            for (int x = 0; x < Terrain.MaxSizeX; x++)
            {
                terrain.Grid![x,y].Ressource.GiveGrowthChance();
                /* Console.Write(terrain.Grid![x,y].Ressource.Type.ToString());
                Console.WriteLine(terrain.Grid![x,y].Ressource.Status.ToString()); */

                terrain.Grid![x,y].TextureRefresher();
                //if (terrain.Grid![x,y].Ressource.Status != Ressources.GrowthProcess.Harvested) terrain.Grid![x,y].Ressource.GiveGrowthChance(10);
                //Console.Write(terrain.Grid![x,y].Texture.TextureName);
                //Console.Write(terrain.Grid![x,y].Texture.Icon);
            }
        }

        /* Console.Write("\n");
        for (int i = 0; i < Terrain.MaxSizeX; i++) Console.Write("="); */
    }

}