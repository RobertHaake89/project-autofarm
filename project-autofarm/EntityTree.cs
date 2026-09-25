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
    private List<Position> _targetMemory {get; set;} = new List<Position>();

    public Entity(string name, char icon, Position position)
    {
        Name = name;
        Icon = icon;
        SpawnPoint = position;
        Position = position;
    }
}

partial class Human : Entity
{
    public const int StaminaMax = 100;
    public int Stamina {get => field; set
        {
            if (field < 0) field = 0;
            else if (field > StaminaMax) field = StaminaMax;
        }}
    public bool IsExhausted {get => field; set
        {
            if (Stamina == 0) field = true;
            else if (Stamina == StaminaMax) field = false;
        }}
    public Profession Profession {get; set;} = Profession.None;

    public Human(string name, char icon, Position position, Profession profession) : base(name, icon, position)
    {
        Profession = profession;
        Position = position;
    }
}