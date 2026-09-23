using System;

namespace ProjectAutofarm;

enum Profession
{
    None = 0,
    Farmer,
    Forager
}

enum Status
{
    Idle = 0,
    Working,
    Resting,
    Sleeping,
    Chatting
}

abstract class Entity : ITargetable
{
    public string Name {get; set;} = "none";
    public char Icon {get; set;}
    public Position Position {get; set;}
    public Status Status {get; set;} = Status.Idle;
    public List<Position> TargetMemory {get; set;} = new List<Position>();

    public Entity(Position position)
    {
        Position = position;
    }

    public void ScanFor(Terrain terrain, ITargetable target)
    {           
        int x = 0;
        int y = 0;

        for (; y < Terrain.MaxSizeY; y++)
        for (; x < Terrain.MaxSizeX; x++)
        {
            if (terrain.Grid![x,y].Position == target.Position)
            {
                TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
            }
        }
    }

    public virtual void Move(Position targetPos)
    {
        if (Position.X < targetPos.X) new Position(Position.X + 1, Position.X);
        else if (Position.Y < targetPos.Y) new Position(Position.Y + 1, Position.Y);

        if (Position.X > targetPos.X) new Position(Position.X - 1, Position.X);
        else if (Position.Y > targetPos.Y) new Position(Position.Y - 1, Position.Y);
    }

    public void Work()
    {
        
    }
}

class Human : Entity
{
    public const int StaminaMax = 100;
    public int Stamina {get; set;} = StaminaMax;
    public Profession Profession {get; set;} = Profession.None;

    public Human(Position position, Profession profession) : base(position)
    {
        Profession = profession;
        Position = position;
    }
}