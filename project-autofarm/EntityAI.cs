using System;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;

namespace ProjectAutofarm;

abstract partial class Entity : ITargetable // AI
{
    public abstract Task RunSchedule(Terrain terrain);
    public abstract void GeneralConditioner(Terrain terrain);
    public abstract void ScanFor(Terrain terrain);
    public abstract void MoveTo(Terrain terrain);
}

partial class Human : Entity
{
    public override async Task RunSchedule(Terrain terrain)
    {
        while (true)
        {
            GeneralConditioner(terrain);
            await Task.Delay(100);
        }
    }
    public override async void GeneralConditioner(Terrain terrain)
    {
        if (Status is Status.Idle)
            {
                if (Profession is not Profession.None)Status = Status.Working;
            }
            if (Status is Status.Working)
            {
                if (!TargetMemory.Any()) ScanFor(terrain);
                
                MoveTo(terrain);
                
                if (Profession is Profession.Farmer) DoFarmWork(terrain);
            
                await Task.Delay(200);

                //Console.WriteLine(Status);
                //Console.WriteLine(IdlePosition);
            }
            else if (Status is Status.Exhausted)
            {
                if (Position == IdlePosition) Status = Status.Resting;

                //TargetPosition = IdlePosition;
                MoveTo(terrain);

                await Task.Delay(400);
            }
            else if (Status is Status.Resting)
            {
                TargetMemory.Clear();
                await Task.Delay(10000);
                Status = Status.Idle;
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
                    if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Fallow)
                    {
                        Mode = ProcessingMode.Plowing;
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                        //TargetMemorySize++;
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Ripe/*  && Mode is ProcessingMode.Harvesting *//*  && TargetMemory.Count == TargetMemorySize */)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));                            
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Plowed/*  && Mode is ProcessingMode.Sowing */)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Harvested/*  && Mode is ProcessingMode.Sowing */)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                    
                }
            }

            TargetMemory = TargetMemory.Distinct().ToList();
            if (TargetMemory.Count == TargetMemorySize) Status = Status.Working;
            
        }
        /* if (!TargetMemory.Any() && Mode is ProcessingMode.Harvesting)
        {
            Status = Status.Exhausted;
            Mode = ProcessingMode.None;
            
        } */
        
    }
    public override void MoveTo(Terrain terrain)
    {
        Position chosenTarget;

        if (Status is not Status.Exhausted && TargetMemory.Count == 0) return;
        else if (Status is not Status.Exhausted && TargetMemory.Count != 0)
        {
            chosenTarget = TargetMemory[0];
            TargetPosition = chosenTarget;
        }
        
        else if (Status is Status.Exhausted)
        {
            TargetMemory.Clear();
            chosenTarget = IdlePosition;
        }
        else chosenTarget = TargetMemory[0];

        if (Position.X < chosenTarget.X && Position.Y < chosenTarget.Y) Position = new Position(Position.X + 1, Position.Y + 1);
        else if (Position.X > chosenTarget.X && Position.Y > chosenTarget.Y) Position = new Position(Position.X - 1, Position.Y - 1);
        else if (Position.X < chosenTarget.X) Position = new Position(Position.X + 1, Position.Y);
        else if (Position.Y < chosenTarget.Y) Position = new Position(Position.X, Position.Y + 1);
        else if (Position.X > chosenTarget.X) Position = new Position(Position.X - 1, Position.Y);
        else if (Position.Y > chosenTarget.Y) Position = new Position(Position.X, Position.Y - 1);

        if (Status is not Status.Exhausted
        && Position == chosenTarget
        && TargetMemory.Any()) TargetMemory.RemoveAt(0); 
    }

    public void DoFarmWork(Terrain terrain)
    {
        if (Specialisation is Specialisation.Wheat && TargetResource is ResourceType.Wheat && Position == TargetPosition)
        {
            if (terrain.Grid![Position.X, Position.Y].Resource.Type == TargetResource)
            {
                if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Ripe)
                {
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Harvested;
                    if (!TargetMemory.Any())
                    {
                        Status = Status.Exhausted;
                        //Mode = ProcessingMode.Sowing;
                       
                    }
                    //Mode = ProcessingMode.Sowing;
                    //return;
                }
                    else if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Harvested)
                {
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Sown;
                    if (!TargetMemory.Any())
                    {
                        Status = Status.Exhausted;
                        //Mode = ProcessingMode.Sowing;
                       
                    }
                }
                    else if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Fallow)
                {
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Plowed;
                    //if (!TargetMemory.Any()) Mode = ProcessingMode.Sowing;
                }
                    else if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Plowed)
                {
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Sown;
                    if (!TargetMemory.Any())
                    {
                        Status = Status.Exhausted;
                        TargetMemorySize = 0;
                        //Mode = ProcessingMode.Sowing;
                       
                    }
                }
                //Console.WriteLine(Stamina);      
            }
        }
        
    }
}
