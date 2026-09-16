using System;
using System.Security.AccessControl;

namespace ProjectAutofarm;

class Structure
{
    public string Name {get; set;}

    public static void CreateFarmHouse()
    {
        Console.SetCursorPosition(6,6);
        Console.Write("""
         ▰▱▰▱▰▱
        ▱▰▱▰▱▰|
        ╋━⊞━▯━━━╋/
        ║║║║║║║║║║ 
             ▤
        """);
    }

    /* private static void CreateFarmHouse()
    {
        string[,] farmHouseArray =
        {
            {"empty","struct_roof1", "struct_roof2", "struct_roof1", "struct_roof2","struct_roof1", "struct_roof2", "empty"},
            {"struct_roof2", "struct_roof1", "struct_roof2","struct_roof1", "struct_roof2", "struct_roof1", "struct_beam_diagonal", "empty"},
            {"struct_beam_vert1","struct_beam_horz1","struct_window1","struct_door1","struct_beam_horz1","struct_beam_horz1", "struct_beam_vert1", "struct_beam_diagonal"},
            {"struct_floor1", "struct_floor1", "struct_floor1", "struct_floor1", "struct_floor1", "struct_floor1","empty", "empty"},
            {"empty", "empty", "empty", "empty", "empty", "struct_stairs1", "empty","empty"}
        };
    } */
}