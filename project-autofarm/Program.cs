using System;

namespace ProjectAutofarm;

class Program
{
    public static void Main()
    {
        Console.ForegroundColor = ConsoleColor.Green;

        Console.WriteLine("\n\tPROJECT AUTOFARM\n");

        var map = new Map();
        //map.CreateMap();

        Console.WriteLine(map.Grid![0,0].Type);
        Console.ReadKey();

        //pos1.x = 2;

        //Console.WriteLine(pos1 == pos2);

        Display.TestScreen(map);
    }
}