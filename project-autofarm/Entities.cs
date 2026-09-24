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
    public Position SpawnPoint {get; set;}
    public Position Position {get; set;}
    public ITargetable TargetType {get; set;}
    public Position TargetPosition {get; set;}
    public Status Status {get; set;} = Status.Idle;
    private List<Position> _targetMemory {get; set;} = new List<Position>();

    public Entity(string name, char icon, Position position)
    {
        Name = name;
        Icon = icon;
        SpawnPoint = position;
        Position = position;
    }

    public virtual void ScanFor(Terrain terrain, ITargetable target)
    {   

        int x = 0;
        int y = 0;

        for (; y < Terrain.MaxSizeY; y++)
        for (; x < Terrain.MaxSizeX; x++)
        {
            if (terrain.Grid![x,y].Position == target.Position)
            {
                _targetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
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
}

class Human : Entity
{
    public const int StaminaMax = 100;
    public int Stamina {get; set;} = StaminaMax;
    private bool _isExhausted {get; set;} = false;
    public Profession Profession {get; set;} = Profession.None;

    public Human(string name, char icon, Position position, Profession profession) : base(name, icon, position)
    {
        Profession = profession;
        Position = position;
    }

    public void RunSchedule(Terrain terrain, ITargetable target)
    {
        ScanFor(terrain, target);
        Move(target.Position);
        ExecuteWork();
    }

    /* public override void ScanFor(Terrain terrain, in ITargetable target)
    {
        if (target == typeof(Resource))
        {
            TargetType = Profession switch
            {
                Profession.Farmer => ResourceType.Wheat,
                Profession.Forager => ResourceType.Mushrooms,
                _ => ResourceType.None
            }
        }
        

        base.ScanFor(terrain, TargetType);
    } */

    public void ExecuteWork()
    {
        
    }
}