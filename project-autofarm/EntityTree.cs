using System.Runtime.CompilerServices;

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
        Mushrooms,
        BlueBerries
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
                value.X.ClampToAxis(0),
                value.Y.ClampToAxis(1));
        }}
    public Position SpawnPoint {get; init;}
    public Position IdlePosition;
    public virtual ResourceType TargetResource {get; set;}
    public Position TargetPosition;
    public virtual List<Position> TargetMemory {get; set;} = new List<Position>();
    public Status Status {get; set;} = Status.Working;
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
    public Specialisation Specialisation
    {
        get;
         set;}
    public override ResourceType TargetResource {get 
        => field = Mode == ProcessingMode.Planting 
            ? ResourceType.Humus
            : Specialisation switch
                {
                    Specialisation.Wheat => ResourceType.Wheat,
                    Specialisation.SpruceWood => ResourceType.SpruceWood,
                    Specialisation.OakWood => ResourceType.OakWood,
                    Specialisation.Mushrooms => ResourceType.Mushrooms,
                    Specialisation.BlueBerries => ResourceType.Blueberry,
                    _ => ResourceType.None
                }; set;}
    public int TargetMemorySize {get; set => field = Math.Max(field,value);}
    public override List<Position> TargetMemory {get; set;} = [];
    public int SpeedWorking {get;
    set => field = Profession switch
    {
        Profession.Farmer => 100,
        Profession.Forester => 10000,
        Profession.Forager => 4000,
        _ => 500
    };}
    public int StaminaMax {get; set;}
    public int Stamina {get; set 
        {
            field = value > 0 ? value : 0;
            if (field == 0)
                Status = Status.Exhausted;
                TargetMemory.Clear();
                TargetMemorySize = 0;
        }}

    public Human(string name, char icon, Gender gender, int speedMove, Profession profession, Specialisation specialisation, Position position)
        : base(name, icon, gender, speedMove, position)
    {
        Profession = profession;
        Specialisation = specialisation;
        StaminaMax = Profession switch
        {
            Profession.Farmer => 30,
            Profession.Forester => 1,
            Profession.Forager => 10,
            _ => 10
        };
        Stamina = StaminaMax;
    }
}

abstract partial class Animal : Entity
{
    public abstract override Position Position {get; set;}
    public bool IsAlive = true;
    
    public Animal(string name, char icon, Gender gender, int speedMove, Position position) : base(name, icon, gender, speedMove, position)
    {
        
    }

    public virtual void SetIcon(Position value)
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
    public char[] IconList {get; set;} = new char[9];
    public override Position Position {get; set
    {
        SetIcon(value);
        LastPosition = value;

        value = new Position(
            value.X.ClampToAxis(0), 
            value.Y.ClampToAxis(1));
        
        field = value;
    }}
    public Position LastPosition {get; set;}

    public Squirrel(string name, int speedMove, Position position) : base(name, 'ꙮ', Gender.Male, speedMove, position)
    {
        TargetResource = ResourceType.Wheat;

        IconList[8] = '૪';
        IconList[6] = 'ᘛ';
        IconList[2] = 'ꙮ';
        IconList[4] = 'ᘚ';
    }

    public override void SetIcon(Position value)
    {
        if (value.X < Position.X && value.Y < Position.Y) Icon = IconList[8]; // diagonal NW
        else if (value.X < Position.X && value.Y > Position.Y) Icon = IconList[2]; // diagonal SW
        else if (value.X > Position.X && value.Y < Position.Y) Icon = IconList[8]; // diagonal NE
        else if (value.X > Position.X && value.Y > Position.Y) Icon = IconList[2]; // diagonal SE

        else if (value.X < Position.X) Icon = IconList[4]; // Left
        else if (value.X > Position.X) Icon = IconList[6]; // Right
        else if (value.Y < Position.Y) Icon = IconList[8]; // Top
        else if (value.Y > Position.Y) Icon = IconList[2]; // Bottom
    }
    
}