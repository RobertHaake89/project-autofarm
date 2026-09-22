using System;

namespace ProjectAutofarm;

interface ITargetable
{
    public RessourceType Type {get; set;}
}

interface IHarvestable
{
    public RessourceType Type {get; set;}
    public int Yield {get; set;}
    public GrowthProcess Status {get; set;}

    public void GiveGrowthChance()
    {
        
    }
}