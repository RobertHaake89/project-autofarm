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
    private Position _position;
    public Position Position {get => _position; set
        {
            _position = new Position(
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

class Human : Entity
{
    public const int StaminaMax = 100;
    public int Stamina {get; set
        {
            if (Stamina < 0) Stamina = 0;
            else if (Stamina > StaminaMax) Stamina = StaminaMax;
        }} = StaminaMax;
    private bool IsExhausted {get; set
        {
            if (Stamina == 0) IsExhausted = true;
            else if (Stamina == StaminaMax) IsExhausted = false;
        }} = false;
    public Profession Profession {get; set;} = Profession.None;

    public Human(string name, char icon, Position position, Profession profession) : base(name, icon, position)
    {
        Profession = profession;
        Position = position;
    }

    public void RunSchedule(Terrain terrain, ITargetable target)
    {
        if (!IsExhausted)
        {
            ScanFor(terrain, target);
            Move(target.Position);
            ExecuteWork();
        }
    }

    /* public override void ScanFor(Terrain terrain, out ITargetable target)
    {
        if (target is Resource resource)
        {
            TargetType = Profession switch
            {
                Profession.Farmer => ResourceType.Wheat,
                Profession.Forager => ResourceType.Mushrooms,
                _ => ResourceType.None
            }
        }
        else if (target is Entity entity)
        {
            
        }
        

        base.ScanFor(terrain, TargetType);
    } */

    public void ExecuteWork()
    {
        
    }
}