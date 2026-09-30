using System;

namespace ProjectAutofarm;

class Program
{
    public static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Clear();

        Console.WriteLine("\n\t\t\tPROJECT AUTOFARM\n");

        Texture.LoadTextures(Texture.List,"Ressources/Textures.sc");

        var terrain = new Terrain();
        terrain.CreateMap();

        // ♟ ♙ 
        Entity human1 = new Farmer("Hans", '♟', position: new Position(12,4),idlePosition: new Position(10,13), Profession.Farmer);
        Entity human2 = new Farmer("Jürgen", '♙', position: new Position(14,4), idlePosition: new Position(14,4), Profession.Forager);

        Dictionary<string, Entity> entityList = new Dictionary<string, Entity>()
        {
            {"human1", human1},
            {"human2", human2}
        };
        
        await General.MainLoop(terrain, entityList);
    }
}