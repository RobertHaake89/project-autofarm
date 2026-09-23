using System;
using System.Security.Cryptography.X509Certificates;

namespace ProjectAutofarm;

public enum ResourceType
    {
        None = 0,
        Wheat,
        Mushrooms
    }
    public enum GrowthProcess
    {
        None = 0,
        Sown,
        Young,
        Mature,
        Ripe,
        Harvested
    }
class Resource : ITargetable, IHarvestable
{
    //public Tile Tile {get;}
    public ResourceType Type {get; set;} = ResourceType.None;
    public string? Name => Type.ToString();
    public Position Position {get; set;}
    public int Yield {get; set;} = 1;
    public GrowthProcess Status {get; set;}

    public Resource(ResourceType type, GrowthProcess status, (int x, int y) position)
    {
        Type = type;
        Status = status;
        Position = new Position(position.x, position.y);
    }

    public virtual void GiveGrowthChance()
    {
        int factor = Type switch
        {
            ResourceType.Wheat => 10,
            ResourceType.Mushrooms => 20,
            _ => 10
        };

        int RandomNumber = Random.Shared.Next(0,factor + 1);

        if (Status == GrowthProcess.Ripe) return;
        else if (RandomNumber == factor)
        {
            if (Status == GrowthProcess.Sown) Status = GrowthProcess.Young;
            else if (Status == GrowthProcess.Young) Status = GrowthProcess.Mature;
            else if (Status == GrowthProcess.Mature) Status = GrowthProcess.Ripe;
        }
    }
}

/* class Wheat : Ressource
{
    public Wheat(GrowthProcess status) : base(RessourceType.Wheat, status)
    {
        
    }
} */


