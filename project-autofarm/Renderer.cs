using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace ProjectAutofarm;

class Renderer
{
    public static void Screen(Terrain terrain, Dictionary<string, Entity> entityDict)
    {
        Console.SetCursorPosition(0, 0);

        int xMax = Terrain.MaxSizeX;
        int yMax = Terrain.MaxSizeY;

        StringBuilder output = new StringBuilder();

        for (int x = 0; x < xMax; x++) output.Append('=');
        output.AppendLine();

        for (int y = 0; y < yMax; y++)
        {
            for (int x = 0; x < xMax; x++)
            {
                output.Append(terrain.Grid![x,y].Texture.Icon);
            }
            output.AppendLine();
        }

        for (int x = 0; x < xMax; x++)
            output.Append('=');

        output.AppendLine();

        var human1 = entityDict["human1"];
        var human2 = entityDict["human2"];
        //var human3 = entityDict["human3"];

        RenderMushrooms(terrain, output);

        RenderEntity(human1, output);
        RenderEntity(human2, output);
        //RenderEntity(human3, output);

        RenderTrees(terrain, output);

        Console.Write(output);
    }

    public static void RenderEntity(Entity entity)
    {
        Console.SetCursorPosition(entity.Position.X, entity.Position.Y);
        Console.Write(entity.Icon);
        Console.SetCursorPosition(0,0);
    }

    public static void RenderEntity(Entity entity, StringBuilder output)
    {
        int index = entity.Position.Y * (Terrain.MaxSizeX + Environment.NewLine.Length)
                        + entity.Position.X;
        
        output[index] = entity.Icon;
    }

    public static void RenderTrees(Terrain terrain, StringBuilder output)
    {     
        foreach (var tree in terrain.TreeList.ToList())
        {
            int startX = tree.Position.X - tree.Texture!.GetLength(1) / 2;
            int startY = tree.Position.Y - (tree.Texture.GetLength(0) - 1);

            for (int y = 0; y < tree.Texture!.GetLength(0); y++)
            {  
                for (int x = 0; x < tree.Texture.GetLength(1); x++)
                {
                    string textureName = tree.Texture[y,x];
                    
                    if (textureName == "skip")
                        continue;
                    
                    int screenX = startX + x;
                    int screenY = startY + y;

                    int index = screenY * (Terrain.MaxSizeX + Environment.NewLine.Length) + screenX;

                    output[index] = Texture.List[textureName];
                }
            }
        }
    }

    public static void RenderMushrooms(Terrain terrain, StringBuilder output)
    {
        foreach (var tree in terrain.TreeList.ToList())
        {
            char icon;
            if (tree.Mycel is not null)
            {
                foreach(var mushroom in tree.Mycel.MushroomList)
                {
                    icon = mushroom.Icon;
                    int index = mushroom.Position.Y * (Terrain.MaxSizeX + Environment.NewLine.Length) + mushroom.Position.X;

                    output[index] = icon;
                }



                    
            }
        }
    }
}