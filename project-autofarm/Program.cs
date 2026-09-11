using System;

namespace ProjectAutofarm;

class Program
{
    public static void Main()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;

        Console.WriteLine("\n\tPROJECT AUTOFARM\n");

        Textures.LoadTextures(Textures.List,"Ressources/Textures.sc");

        Console.WriteLine($"Textures loaded: {Textures.List.Count}");
        Console.WriteLine($"bot_dirt1 texture: {Textures.GetTexture("bot_grass1")}");

        var map = new Terrain();
        map.CreateMap();

        Console.WriteLine(map.Grid![0,0].Texture);
        Console.ReadKey();

        Display.TestScreen(map);
    }
}