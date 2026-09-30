using System;
using System.Formats.Asn1;

namespace ProjectAutofarm;

abstract partial class Entity : ITargetable // AI
{
    public abstract Task RunSchedule(Terrain terrain);
    public abstract void ScanFor(Terrain terrain);
    public abstract void MoveTo(Terrain terrain);
}

abstract partial class Human : Entity
{
    public override void MoveTo(Terrain terrain)
    {
        Position chosenTarget;
        //Position currentPosition;

        if (!IsExhausted && TargetMemory.Count == 0) return;
        else if (!IsExhausted && TargetMemory.Count != 0) chosenTarget = TargetMemory[0];
        else if (IsExhausted)
        {
            TargetMemory.Clear();
            chosenTarget = SpawnPoint;
        }
        
        else chosenTarget = TargetMemory[0];

        if (Position.X < chosenTarget.X && Position.Y < chosenTarget.Y) Position = new Position(Position.X + 1, Position.Y + 1);
        else if (Position.X > chosenTarget.X && Position.Y > chosenTarget.Y) Position = new Position(Position.X - 1, Position.Y - 1);
        else if (Position.X < chosenTarget.X) Position = new Position(Position.X + 1, Position.Y);
        else if (Position.Y < chosenTarget.Y) Position = new Position(Position.X, Position.Y + 1);
        else if (Position.X > chosenTarget.X) Position = new Position(Position.X - 1, Position.Y);
        else if (Position.Y > chosenTarget.Y) Position = new Position(Position.X, Position.Y - 1);

        TargetPosition = chosenTarget;
        if (Position == chosenTarget) TargetMemory.RemoveAt(0);
    }
}

partial class Farmer : Human
{
    public override async Task RunSchedule(Terrain terrain)
    {
        while (true)
        {
            if (!IsExhausted)
            {
                ScanFor(terrain);
                await Task.Delay(10);
                
                MoveTo(terrain);
                await Task.Delay(300);
                DoWork(terrain);
                await Task.Delay(80);
            }
            else if (IsExhausted)
            {
                TargetPosition = SpawnPoint;
                MoveTo(terrain);
            }
            //await Task.Delay(400);
        }
    }

    public override void ScanFor(Terrain terrain)
    {
        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            int start = y % 2 == 0 ? 0 : Terrain.MaxSizeX - 1;
            int end = y % 2 == 0 ? Terrain.MaxSizeX : - 1;
            int step = y % 2 == 0 ? 1: -1;

            for (int x = start; x != end; x += step)
            {
                if (terrain.Grid![x,y].Resource.Type == TargetResource)
                {
                    if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Ripe)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));                            
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Fallow)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Plowed)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                    else if (/* !TargetMemory.Any() &&  */terrain.Grid![x,y].Resource.Status is GrowthProcess.Harvested)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                }
            }
        }
    }

    public void DoWork(Terrain terrain)
    {
        if (TargetResource is ResourceType.Wheat && Position == TargetPosition)
        {
            if (terrain.Grid![Position.X, Position.Y].Resource.Type == TargetResource)
            {
                if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Ripe)
                {
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Harvested;
                    return;
                }
                    else if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Harvested)
                {
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Sown;
                }
                    else if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Fallow)
                {
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Plowed;
                }
                    else if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Plowed)
                {
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Sown;
                }

                //Stamina--;
                //Console.WriteLine(Stamina);      
            }
        }
    }
}

