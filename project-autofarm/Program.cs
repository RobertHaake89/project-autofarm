using System;

namespace ProjectAutofarm;

class Program
{
    public static void Main()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;

        Console.WriteLine("\n\tPROJECT AUTOFARM\n");

        var terrain = new Terrain();
        terrain.CreateMap();

        Texture.LoadTextures(Texture.List,"Ressources/Textures.sc");

        Console.WriteLine($"Textures loaded: {Texture.List.Count}");
        //Console.WriteLine($"bot_dirt1 texture");

        

        Console.WriteLine(terrain.Grid![0,0].Texture);
        Console.ReadKey();

        Display.TestScreen(terrain);
    }
}