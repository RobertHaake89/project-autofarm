namespace ProjectAutofarm;

abstract class Structure
{
    public Position Position {get; set;}
    public Position Interaction {get; set;}

    public Structure(Position position, Position interaction)
    {
        Position = position;
        Interaction = interaction;
    }
}

class FarmHouse : Structure
{
    public FarmHouse(Position position, Position interaction) 
        :base(position, new Position(position.X + 5, position.Y + 3))
    {
        Interaction = interaction;
    }
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