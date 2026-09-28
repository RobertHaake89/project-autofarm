using System;
using System.Formats.Asn1;

namespace ProjectAutofarm;

abstract partial class Entity : ITargetable // AI
{
    public abstract void RunSchedule(Terrain terrain);
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

        //Position = currentPosition;

        TargetPosition = chosenTarget;
        if (Position == chosenTarget) TargetMemory.RemoveAt(0);
    }
}

partial class Farmer : Human
{
    public override void RunSchedule(Terrain terrain)
    {
        if (!IsExhausted)
        {
            //Console.WriteLine("seems not exhausted");
            Thread.Sleep(300);
            if(TargetMemory.Any() == false && !IsExhausted) ScanFor(terrain);
            MoveTo(terrain);
            DoWork(terrain);
            //if(TargetMemory.Any() == false && !IsExhausted) Reseed(terrain);
        }
        else if (IsExhausted)
        {
            //Console.WriteLine("seems exhausted");
            Thread.Sleep(300);
            TargetPosition = SpawnPoint;
            MoveTo(terrain);
        }
    }

    public override void ScanFor(Terrain terrain)
    {
        //ResourceType target = TargetResource;
        //Console.WriteLine($"Target: {target}");
        //Thread.Sleep(500);

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
                else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Harvested)
                {
                    TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                }
            }
        }
    }
        
        //Console.WriteLine("IS SCANNED");
    }

    public void DoWork(Terrain terrain)
    {
        if (Position == TargetPosition)
        {
            //Console.WriteLine("is on target");
            //Thread.Sleep(300);
            if (terrain.Grid![Position.X, Position.Y].Resource.Type == TargetResource)
                if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Harvested)
                {
                    //Console.WriteLine("is sowing");
                    //Thread.Sleep(300);
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Sown;
                }
                else if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Ripe)
                {
                    //Console.WriteLine("is Harvesting");
                    //Thread.Sleep(300);
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Harvested;
                }
                
                Stamina--;
                Console.WriteLine(Stamina);

                    
                
            
                    
            }
            
        }
    }

    /* public void Reseed(Terrain terrain)
    {
        if (Position == TargetPosition)
        {
            if (terrain.Grid![Position.X, Position.Y].Resource.Type == TargetResource
            && (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Harvested))
            {
                terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Sown;
                Stamina--;
            }
        }
    } */
