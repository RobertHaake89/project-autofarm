using System;

namespace ProjectAutofarm;

abstract class Ressources
{
    public enum RessourceType
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

    public RessourceType Type {get; set;} = RessourceType.None;
    public string? Name => Type.ToString();
    public int Yield {get; set;} = 1;
    public GrowthProcess Status {get; set;}

    public Ressources(RessourceType type, GrowthProcess status)
    {
        Type = type;
        Status = status;
    }

    public virtual void GiveGrowthChance()
    {
        int factor = Type switch
        {
            RessourceType.Wheat => 10,
            RessourceType.Mushrooms => 20,
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

class Wheat : Ressources
{
    public Wheat(GrowthProcess status) : base(RessourceType.Wheat, status)
    {
        
    }
}


