using System;
using System.Diagnostics.Tracing;
using System.Reflection.Metadata;
using System.Xml.Serialization;

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
    Sleeping,
    Chatting
}

abstract class Entity
{
    public string Name {get; set;} = "none";
    public const int StaminaMax = 100;
    public int Stamina {get; set;} = StaminaMax;
    public Position Position {get; set;}
    public List<Position> TargetMemory {get; set;} = new List<Position>();
    private Profession _profession {get; set;} = Profession.None;
    //private RessourceType _targetRessource {get; set;} = RessourceType.None;
    private Status Status {get; set;} = Status.Idle;

    public Entity(Position position, Profession profession)
    {
        Position = position;
        _profession = profession;
    }

    public void ScanFor(Terrain terrain, ITargetable target)
    {   
        //new List<(int, int)>();
        
        int x = 0;
        int y = 0;

        for (; y < terrain.Grid!.GetLength(0); y++)
        for (; x < terrain.Grid.GetLength(1); x++)
        {
            if (terrain.Grid[x,y].Ressource.Type == target.Type)
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