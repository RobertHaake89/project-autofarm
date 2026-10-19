namespace ProjectAutofarm;

abstract partial class Entity : ITargetable // AI
{
    public abstract Task RunSchedule(Terrain terrain);
    public abstract Task GeneralConditioner(Terrain terrain);
    public abstract Task ScanFor(Terrain terrain);
    public abstract Task MoveTo(Terrain terrain);
}

partial class Human : Entity
{
    public override async Task RunSchedule(Terrain terrain)
    {
        while (true)
        {
            await GeneralConditioner(terrain);
        }
    }
    public override async Task GeneralConditioner(Terrain terrain)
    {
        if (Status is Status.Idle)
        {
            if (Profession is not Profession.None)
                Status = Status.Working;

            if (Profession is Profession.Forester
                && terrain.TreeList.Count( tree => tree.Status is GrowthProcess.Ripe)
                >= Terrain.TreeCuttingThreshold)
                Status = Status.Working;
        }

        if (Status is Status.Working)
        {
            if (!TargetMemory.Any())
                await ScanFor(terrain);
            
            await MoveTo(terrain);
            await Task.Delay(SpeedMove);
            
            if (Profession is Profession.Farmer)
            {
                await DoFarmWork(terrain);
                await Task.Delay(SpeedWorking);
            } 
            if (Profession is Profession.Forester)
            {
                if (terrain.TreeList.Count >= Terrain.TreeCuttingThreshold)
                {
                    Mode = ProcessingMode.Harvesting;
                    await ChopWood(terrain);
                    await Task.Delay(SpeedWorking);
                }
                else if (terrain.TreeList.Count < Terrain.TreeMaxQty)
                {
                    Mode = ProcessingMode.Planting;
                    await PlantTree(terrain,randomFactor: 45);
                    await Task.Delay(SpeedWorking);
                }
            }
                
        }
        if (Status is Status.Exhausted)
        {
            TargetPosition = IdlePosition;
            if (Position == IdlePosition) Status = Status.Resting;

            await MoveTo(terrain);

            await Task.Delay(SpeedMove * 2);
        }
        if (Status is Status.Resting)
        {
            TargetMemory.Clear();
            await Task.Delay(10000);

            Status = Status.Idle;
        }
    }



    public override async Task ScanFor(Terrain terrain)
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
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Ripe or GrowthProcess.Mature or GrowthProcess.Young)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));                            
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Plowed)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Harvested)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                }
            }
            TargetMemory = TargetMemory.Distinct().ToList();

            if (Profession is Profession.Forester && Mode is ProcessingMode.Harvesting) TargetMemory.Shuffle();
        }        
    }

    public override async Task MoveTo(Terrain terrain)
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
        && TargetMemory.Any())
            TargetMemory.RemoveAt(0); 
    }

    public async Task DoFarmWork(Terrain terrain)
    {
        if (Specialisation is Specialisation.Wheat && TargetResource is ResourceType.Wheat && Position == TargetPosition)
        {
            if (terrain.Grid![Position.X, Position.Y].Resource.Type == TargetResource)
            {
                if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Ripe)
                {
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Harvested;
                }
                    else if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Harvested)
                {
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Planted;
                    if (!TargetMemory.Any())
                    {
                        Status = Status.Exhausted;                       
                    }
                }
                    else if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Fallow)
                {
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Plowed;
                }
                    else if (terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Plowed)
                {
                    terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Planted;
                    if (!TargetMemory.Any())
                    {
                        Status = Status.Exhausted;
                        TargetMemorySize = 0;                     
                    }
                }
                //Console.WriteLine(Stamina);      
            }
        }
    }

    public async Task ChopWood(Terrain terrain)
    {
        if (Mode is ProcessingMode.Harvesting
            && Specialisation is Specialisation.SpruceWood or Specialisation.OakWood
            && TargetResource is ResourceType.SpruceWood or ResourceType.OakWood
            && Position.X == TargetPosition.X
            && Position.Y == TargetPosition.Y)
        {
            if (terrain.Grid![TargetPosition.X, TargetPosition.Y].Resource.Type == TargetResource)
            {
                if (terrain.Grid![TargetPosition.X, TargetPosition.Y].Resource.Status is GrowthProcess.Ripe)
                {
                    Position = new Position(Position.X - 1, Position.Y);
                    await Task.Delay(10_000); // 10000
                    terrain.TreeList.RemoveAll(tree 
                    => tree.Position.X == TargetPosition.X
                    && tree.Position.Y == TargetPosition.Y);

                    terrain.Grid![TargetPosition.X, TargetPosition.Y].Resource.Status = GrowthProcess.Fallow;
                    terrain.Grid![TargetPosition.X, TargetPosition.Y].Resource.Type = ResourceType.None;

                    Status = Status.Exhausted;
                    TargetMemory.Clear();
                }
                if (TargetMemory.Count == 0)
                {
                    Status = Status.Exhausted;
                    TargetMemorySize = 0;                     
                }
            }
        }
    }
    public async Task PlantTree(Terrain terrain, int randomFactor)
    {
        if (Status is Status.Exhausted)
            return;

        int randomNumber = RandomNumberGenerator.GetInt32(randomFactor + 1); //Random.Shared.Next(randomFactor + 1);

        if (Position.Y > 3
        &&terrain.Grid![Position.X, Position.Y].Soil
            is Soil.Fertile
        && terrain.Grid![Position.X, Position.Y].Resource.Type
            is not ResourceType.Wheat)
        {
            if (randomNumber == randomFactor)
            {
                terrain.TreeList.Add(new Tree(PlantType.Spruce, new Position(Position.X, Position.Y), GrowthProcess.Planted, terrain));
            }

            if (!TargetMemory.Any()) Status = Status.Exhausted;
            if (terrain.TreeList.Count >= Terrain.TreeCuttingThreshold)
            {
                
                Mode = ProcessingMode.Harvesting;
                Status = Status.Exhausted;
                TargetMemory.Clear();
            }
            else await ScanFor(terrain);
            
        }
    }
}

partial class Animal
{
    
}
partial class Squirrel
{
    public override async Task RunSchedule(Terrain terrain)
    {
        while (true)
        {
            await GeneralConditioner(terrain);
        }
    }
    public override async Task GeneralConditioner(Terrain terrain)
    {
        //await ScanFor(terrain);
        await MoveInCircles();
        //await MoveTo(terrain);
    }
    public override async Task ScanFor(Terrain terrain)
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
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Ripe or GrowthProcess.Mature or GrowthProcess.Young)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));                            
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Plowed)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Harvested)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                }
            }
            TargetMemory = TargetMemory.Distinct().ToList();

            //if (Profession is Profession.Forester && Mode is ProcessingMode.Harvesting) TargetMemory.Shuffle();
        }        
    }
    public override async Task MoveTo(Terrain terrain)
    {
        
    }
}
