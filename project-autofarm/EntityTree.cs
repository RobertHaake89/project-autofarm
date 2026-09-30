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
    Exhausted,
    Resting,
    Sleeping,
    Chatting
}

abstract partial class Entity : ITargetable
{
    public string Name {get; set;} = "none";
    public char Icon {get; set;} = ' ';
    public Position SpawnPoint {get; set;}
    public Position Position {get => field; set
        {
            field = new Position(
                Math.Clamp(value.X, 0, Terrain.MaxSizeX),
                Math.Clamp(value.Y, 0, Terrain.MaxSizeY));
        }}
    public Status Status {get; set;} = Status.Idle;

    public Entity(string name, char icon, Position position)
    {
        Name = name;
        Icon = icon;
        SpawnPoint = position;
        Position = position;
    }
}

abstract partial class Human : Entity
{
    public const int StaminaMax = 50;
    public int Stamina {get => field; set
        {
            if (field < 0) field = 0;
            else if (field > StaminaMax) field = StaminaMax;
        }} = StaminaMax;
    /* public bool IsExhausted {get => field; set
        {
            if (Stamina == 0) field = true;
            else if (Stamina == StaminaMax) field = false;
        }} = false; */
    public Profession Profession {get; set;} = Profession.None;
    public ResourceType TargetResource => Profession switch
            {
                Profession.Farmer => ResourceType.Wheat,
                Profession.Forager => ResourceType.Mushrooms,
                Profession.None => ResourceType.None,
                _ => ResourceType.None
            };
    public List<Position> TargetMemory {get; set;} = new List<Position>();
    public Position TargetPosition;
    public Position IdlePosition;

    public Human(string name, char icon, Position position, Position idlePosition, Profession profession) : base(name, icon, position)
    {
        Profession = profession;
        Position = position;
        TargetPosition = position;
        IdlePosition = idlePosition;
    }
}

partial class Farmer : Human
{
    public Farmer(string name, char icon, Position position, Position idlePosition, Profession profession) : base(name, icon, position, idlePosition, profession)
    {
        Profession = profession;
        Position = position;
        TargetPosition = position;
        IdlePosition = idlePosition;

    }
}