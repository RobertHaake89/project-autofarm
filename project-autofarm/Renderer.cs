using System;
using System.Text;

namespace ProjectAutofarm;

class Renderer
{
    public static void Screen(Terrain terrain, Dictionary<string, Entity> entityDict)
    {
        Console.SetCursorPosition(0, 0);

        StringBuilder output = new StringBuilder();

        for (int x = 0; x < Terrain.MaxSizeX; x++) output.Append('=');
        output.AppendLine();

        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            for (int x = 0; x < Terrain.MaxSizeX; x++)
            {
                output.Append(terrain.Grid![x,y].Texture.Icon);
            }
            output.AppendLine();
        }

        for (int x = 0; x < Terrain.MaxSizeX; x++) output.Append('=');
        output.AppendLine();

        Console.Write(output);

        RenderEntity(entityDict["human1"]);
        RenderEntity (entityDict["human2"]);

        RenderTrees(terrain);
    }

    public static void RenderEntity(Entity entity)
    {
        Console.SetCursorPosition(entity.Position.X, entity.Position.Y);
        Console.Write(entity.Icon);
        Console.SetCursorPosition(0,0);
    }

    public static void RenderTrees(Terrain terrain)
    {
        foreach (var tree in terrain.TreeList)

        for (int y = 0; y < tree.Texture!.GetLength(0); y++)
        {  
            for (int x = 0; x < tree.Texture.GetLength(1); x++)
            {
                if (terrain.Grid![tree.Position.X + tree.Texture.GetLength(1), tree.Position.Y + tree.Texture.GetLength(0)].Resource.Type is not ResourceType.Wheat)
                {
                    Console.SetCursorPosition(tree.Position.X + x, tree.Position.Y + y);

                    string textureName = tree.Texture[y,x];
                    
                    if (textureName != "skip")
                    {
                        Console.Write(Texture.List[textureName]);
                    }
                }
            }
        }

    }
}