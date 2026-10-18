using System;
using System.Runtime;

namespace ProjectAutofarm;


public enum Gender
    {
        None = 0,
        Male,
        Female
    }
    public enum Status
    {
        Idle = 0,
        Working,
        Exhausted,
        Resting,
        Sleeping,
        Chatting,
        Scampering
    }
    public enum Profession
    {
        None = 0,
        Farmer,
        Forester,
        Forager,
        Hunter
    }

    public enum Specialisation
    {
        None = 0,
        Wheat,
        SpruceWood,
        OakWood,
        Mushrooms
    }

    public enum ProcessingMode
    {
        None = 0,
        Plowing,
        Planting,
        Harvesting,
    }
abstract partial class Entity : ITargetable
{
    public string Name {get; set;} = "none";
    public char Icon { get => Position == SpawnPoint ? ' ' : field;
    set ;}
    public Gender Gender {get; init;}
    public int SpeedMove {get => 
    Status is Status.Exhausted 
        ? (int)(field * 1.5) 
        : field; set;}
    public virtual Position Position {get => field; set
        {
            field = new Position(
                Math.Clamp(value.X, 0, Terrain.MaxSizeX),
                Math.Clamp(value.Y, 0, Terrain.MaxSizeY));
        }}
    public Position SpawnPoint {get; init;}
    public Position IdlePosition;
    public virtual ResourceType TargetResource {get; set;}
    public Position TargetPosition;
    public virtual List<Position> TargetMemory {get; set;} = new List<Position>();
    public Status Status {get; set;} = Status.Idle;
    public ProcessingMode Mode {get; set;} = ProcessingMode.None;

    public Entity(string name, char icon, Gender gender, int speedMove, Position position)
    {
        Name = name;
        Icon = icon;
        Gender = gender;
        SpeedMove = speedMove;
        SpawnPoint = position;
        Position = position;
        IdlePosition = SpawnPoint;
        TargetPosition = position;
    }
}

partial class Human : Entity
{
    public Profession Profession {get; set;}
    public Specialisation Specialisation {get; set;}
    public override ResourceType TargetResource {get => field = Mode == ProcessingMode.Planting 
        ? ResourceType.HumusSoil
        : Specialisation switch
            {
                Specialisation.Wheat => ResourceType.Wheat,
                Specialisation.SpruceWood => ResourceType.SpruceWood,
                Specialisation.OakWood => ResourceType.OakWood,
                Specialisation.Mushrooms => ResourceType.Mushrooms,
                _ => ResourceType.None
            }; set;}
    public int TargetMemorySize {get => field; set => field = Math.Max(field,value);}
    public override List<Position> TargetMemory {get; set;} = new List<Position>();
    public int SpeedWorking {get;
    set => field = Profession switch
    {
        Profession.Farmer => 100,
        Profession.Forester => 10000,
        Profession.Forager => 4000,
        _ => 500
    };}

    public Human(string name, char icon, Gender gender, int speedMove, Profession profession, Specialisation specialisation, Position position)
        : base(name, icon, gender, speedMove, position)
    {
        Profession = profession;
        Specialisation = specialisation;
    }
}

abstract partial class Animal : Entity
{
    public abstract override Position Position {get; set;}
            
    public bool IsAlive = true;
    
    public Animal(string name, char icon, Gender gender, int speedMove, Position position) : base(name, icon, gender, speedMove, position)
    {
        
    }

    public void SetIcon(Position value)
    {
        if (value.X < Position.X && value.Y < Position.Y) Icon = '૪'; // diagonal NW
        else if (value.X < Position.X && value.Y > Position.Y) Icon = 'ꙮ'; // diagonal SW
        else if (value.X > Position.X && value.Y < Position.Y) Icon = '૪'; // diagonal NE
        else if (value.X > Position.X && value.Y > Position.Y) Icon = 'ꙮ'; // diagonal SE

        else if (value.X < Position.X) Icon = 'ᘚ'; // Left
        else if (value.X > Position.X) Icon = 'ᘛ'; // Right
        else if (value.Y < Position.Y) Icon = '૪'; // Top
        else if (value.Y > Position.Y) Icon = 'ꙮ'; // Bottom
    }
}

partial class Squirrel : Animal
{
    public override Position Position {get; set
    {
        SetIcon(value);
        field = value;
    }}

    public Squirrel(string name, char icon, int speedMove, Position position) : base(name, icon, Gender.Male, speedMove, position)
    {
        TargetResource = ResourceType.Wheat;
    }

    public async Task MoveInCircles()
    {
        Position = new Position(Position.X + 1, Position.Y); // East
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y);
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y);
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y);
        await Task.Delay(200);

        Position = new Position(Position.X + 1, Position.Y + 1); // SE
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y + 1);
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y + 1);
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y + 1);
        await Task.Delay(200);

        Position = new Position(Position.X, Position.Y + 1); // South
        await Task.Delay(200);
        Position = new Position(Position.X, Position.Y + 1);
        await Task.Delay(200);
        Position = new Position(Position.X, Position.Y + 1);
        await Task.Delay(200);
        Position = new Position(Position.X, Position.Y + 1);
        await Task.Delay(200);

        Position = new Position(Position.X - 1, Position.Y + 1); // SW
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y + 1);
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y + 1);
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y + 1);
        await Task.Delay(200);

        Position = new Position(Position.X - 1, Position.Y); // West
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y);
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y);
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y);
        await Task.Delay(200);

        Position = new Position(Position.X - 1, Position.Y - 1); // NW
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y - 1);
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y - 1);
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y - 1);
        await Task.Delay(200);

        Position = new Position(Position.X, Position.Y - 1); // North
        await Task.Delay(200);
        Position = new Position(Position.X, Position.Y - 1);
        await Task.Delay(200);
        Position = new Position(Position.X, Position.Y - 1);
        await Task.Delay(200);
        Position = new Position(Position.X, Position.Y - 1);
        await Task.Delay(200);

        Position = new Position(Position.X + 1, Position.Y - 1); // NE
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y - 1);
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y - 1);
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y - 1);
        await Task.Delay(200);
    }
}