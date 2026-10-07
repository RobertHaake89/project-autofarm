using System;

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
        Chatting
    }
    public enum Profession
    {
        None = 0,
        Farmer,
        Forester,
        Forager
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
        Sowing,
        Harvesting
    }
abstract partial class Entity : ITargetable
{
    public string Name {get; set;} = "none";
    public char Icon { get => Position == SpawnPoint ? ' ' : field;
    set ;}
    public Gender Gender {get; init;}
    public Position Position {get => field; set
        {
            field = new Position(
                Math.Clamp(value.X, 0, Terrain.MaxSizeX),
                Math.Clamp(value.Y, 0, Terrain.MaxSizeY));
        }}
    public Position SpawnPoint {get; init;}
    public Position IdlePosition;
    public Position TargetPosition;
    public Status Status {get; set;} = Status.Idle;

    public Entity(string name, char icon, Gender gender, Position position)
    {
        Name = name;
        Icon = icon;
        Gender = gender;
        SpawnPoint = position;
        Position = position;
        IdlePosition = SpawnPoint; //new Position(Random.Shared.Next(0, Terrain.MaxSizeX), Random.Shared.Next(0, Random.Shared.Next(0, Terrain.MaxSizeY)));
        TargetPosition = position;
    }
}

partial class Human : Entity
{
    public Profession Profession {get; set;}
    public Specialisation Specialisation {get; set;} = Specialisation.Wheat;
    public ResourceType TargetResource => Specialisation switch
            {
                Specialisation.Wheat => ResourceType.Wheat,
                Specialisation.SpruceWood => ResourceType.SpruceWood,
                Specialisation.OakWood => ResourceType.OakWood,
                Specialisation.Mushrooms => ResourceType.Mushrooms,
                _ => ResourceType.None
            };
    public ProcessingMode Mode {get; set;} = ProcessingMode.None;
    public int TargetMemorySize {get => field; set => field = Math.Max(field,value);}
    public List<Position> TargetMemory {get; set;} = new List<Position>();

    public Human(string name, char icon, Gender gender,Profession profession, Specialisation specialisation, Position position) : base(name, icon, gender, position)
    {
        Profession = profession;
        Specialisation = specialisation;
    }
}

