using System;

namespace ProjectAutofarm;

interface ITargetable
{
    public Position Position {get; set;}
}

interface IHarvestable
{
    public ResourceType Type {get; set;}
    public int Yield {get; set;}
    public GrowthProcess Status {get; set;}

    public void GiveGrowthChance()
    {
        
    }
}