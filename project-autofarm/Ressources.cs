using System;
using System.Security.Cryptography.X509Certificates;

namespace ProjectAutofarm;

public enum ResourceType
    {
        None = 0,
        Fertile,
        Wheat,
        SpruceWood,
        OakWood,
        Mushrooms
    }
    public enum GrowthProcess
    {
        Fallow = 0,
        Plowed,
        Planted,
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
            ResourceType.Wheat => 25,
            ResourceType.Mushrooms => 60,
            ResourceType.SpruceWood => 30,
            ResourceType.OakWood => 30,
            _ => 10
        };

        int RandomNumber = Random.Shared.Next(0,factor + 1);

        if (Status == GrowthProcess.Ripe) return;
        else if (RandomNumber == factor)
        {
            if (Status == GrowthProcess.Planted) Status = GrowthProcess.Young;
            else if (Status == GrowthProcess.Young) Status = GrowthProcess.Mature;
            else if (Status == GrowthProcess.Mature) Status = GrowthProcess.Ripe;
        }
    }
}



