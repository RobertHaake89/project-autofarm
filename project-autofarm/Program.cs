using System;

namespace ProjectAutofarm;

class Program
{
    public static void Main()
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
        var human1 = new Human("Hans", '♟', new Position(12,4), Profession.Farmer);

        General.MainLoop(terrain, human1);
    }
}