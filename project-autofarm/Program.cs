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
        Entity human1 = new Human("Hans", '♟', Gender.Male,speedMove: 100, Profession.Farmer, Specialisation.Wheat, position: new Position(8,3));
        Entity human2 = new Human("Jürgen", '♟', Gender.Male,speedMove: 100, Profession.Forester, Specialisation.SpruceWood, position: new Position(8,3));
        //Entity human3 = new Human("Gerda", '♙', Gender.Female, speedMove: 100, Profession.Forager, Specialisation.Mushrooms, position: new Position(8,3));

        Dictionary<string, Entity> entityList = new Dictionary<string, Entity>()
        {
            {"human1", human1},
            {"human2", human2},
            //{"human3", human3}
        };
        
        await General.MainLoop(terrain, entityList);
    }
}