using System;
using System.Data;

namespace ProjectAutofarm;

class Display
{
    public static void TestScreen(Map map)
    {
        foreach (Tile tile in map.Grid)
        {
            Console.Write(tile.Type);
        }
    }
}