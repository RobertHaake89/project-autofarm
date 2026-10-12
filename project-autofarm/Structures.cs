using System;
using System.Security.AccessControl;

namespace ProjectAutofarm;

class Structure
{
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
}