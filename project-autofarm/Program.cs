using System;

namespace ProjectAutofarm;

class Program
{
    public static void Main()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Clear();

        Console.WriteLine("\n\tPROJECT AUTOFARM\n");

        Texture.LoadTextures(Texture.List,"Ressources/Textures.sc");

        var terrain = new Terrain();
        terrain.CreateMap();

        /* Console.WriteLine($"Textures loaded: {Texture.List.Count}");
        Console.WriteLine($"texture: {terrain.Grid[0,0].Texture.Icon}"); */

        //Console.WriteLine(terrain.Grid![0,0].Texture);
        //Console.ReadKey();

        Display.TestScreen(terrain);
    }
}