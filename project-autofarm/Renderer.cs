using System;
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



        for (int x = 0; x < xMax; x++) output.Append('=');
        output.AppendLine();

        var human1 = entityDict["human1"];
        var human2 = entityDict["human2"];

        int index1 = human1.Position.Y * (xMax + Environment.NewLine.Length)
                        + human1.Position.X;
        
        output[index1] = human1.Icon;

        int index2 = human2.Position.Y * (xMax + Environment.NewLine.Length)
                        + human2.Position.X;
        
        output[index2] = human2.Icon;

        RenderTrees(terrain, output);

        Console.Write(output);
    }

    public static void RenderEntity(Entity entity)
    {
        Console.SetCursorPosition(entity.Position.X, entity.Position.Y);
        Console.Write(entity.Icon);
        Console.SetCursorPosition(0,0);
    }

    public static void RenderTrees(Terrain terrain, StringBuilder output)
    {     
        foreach (var tree in terrain.TreeList)

        for (int y = 0; y < tree.Texture!.GetLength(0); y++)
        {  
            for (int x = 0; x < tree.Texture.GetLength(1); x++)
            {
                string textureName = tree.Texture[y,x];
                
                if (textureName == "skip") continue;
                
                int screenX = tree.Position.X + x;
                int screenY = tree.Position.Y + y;

                int index = screenY * (Terrain.MaxSizeX + Environment.NewLine.Length) + screenX;

                output[index] = Texture.List[textureName];
            }
        }
    }
}