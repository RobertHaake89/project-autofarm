using System.Net.NetworkInformation;
using System.Reflection.Emit;
using Microsoft.VisualBasic.FileIO;

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
            if (Profession is Profession.Farmer
                && terrain.CropTileList.All(
                crop => crop.Resource.Status is GrowthProcess.Ripe or GrowthProcess.Fallow))
                    Status = Status.Working;
            
            else if (Profession is Profession.Forester)
            {
                if (terrain.TreeList.Count(tree => tree.Status is GrowthProcess.Ripe)
                    >= Terrain.TreeCuttingThreshold)
                {
                    Mode = ProcessingMode.Harvesting;
                    Status = Status.Working;
                }
                else if (terrain.TreeList.Count < Terrain.TreeMaxQty / 3)
                {
                    Mode = ProcessingMode.Planting;
                    Status = Status.Working;
                }
                else Status = Status.Idle;
            }       
            else if (Profession is Profession.Forager)
            {
                Console.Write($"prof: {Profession} special: {Specialisation} mode: {Mode}");


                if (terrain.BushList.Count(bush => bush.Status is GrowthProcess.Ripe)
                    > Terrain.BushMaxQty / 3)
                {
                    Mode = ProcessingMode.Harvesting;
                    Specialisation = Specialisation.BlueBerries;
                    Status = Status.Working;
                }
                else if (terrain.TreeList.Sum(tree 
                    => tree.Mycel?.MushroomList.Count(mushroom 
                    => mushroom.Status is GrowthProcess.Ripe)
                    ?? 0) > terrain.TreeList.Count(mycel => mycel is not null) / 3)
                {
                    Mode = ProcessingMode.Harvesting;
                    Specialisation = Specialisation.Mushrooms;
                    Status = Status.Working;
                }
                else Status = Status.Idle;
            }

            await Task.Delay(Random.Shared.Next(3000, 6000));
        }

        if (Status is Status.Working)
        {
            if (!TargetMemory.Any())
                await ScanFor(terrain);
            
            await MoveTo(terrain);
            
            if (Profession is Profession.Farmer)
            {
                await DoFarmWork(terrain);
            } 
            if (Profession is Profession.Forester)
            {
                if (Mode is ProcessingMode.Harvesting)
                {
                    await ChopWood(terrain);
                }
                else if (Mode is ProcessingMode.Planting)
                {
                    await PlantTree(terrain,randomFactor: 50);
                }
            }
            if (Profession is Profession.Forager
            && Mode is ProcessingMode.Harvesting)
                await DoForaging(terrain);
            
                //Console.Write($"{Profession} is working!");
        }
        if (Status is Status.Exhausted)
        {
            TargetPosition = IdlePosition;
            if (Position == IdlePosition) Status = Status.Resting;

            await MoveTo(terrain);
        }
        if (Status is Status.Resting)
        {
            TargetMemory.Clear();

            await Task.Delay(
                Random.Shared.Next(8000, 12_000));

            Stamina = StaminaMax;

            Status = Status.Idle;
        }
    }

    public override async Task ScanFor(Terrain terrain)
    {
        int scanDuration = Random.Shared.Next(2000, 6000);

        for (int y = 0; y < Terrain.MaxSizeY; y++)
        {
            int start = y % 2 == 0 ? 0 : Terrain.MaxSizeX - 1;
            int end = y % 2 == 0 ? Terrain.MaxSizeX : - 1;
            int step = y % 2 == 0 ? 1 : -1;

            for (int x = start; x != end; x += step)
            {
                if (terrain.Grid![x,y].Resource.Type == TargetResource)
                {
                    if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Ripe/*  or GrowthProcess.Mature or GrowthProcess.Young */)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));                            
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Harvested)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Plowed)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                    else if (terrain.Grid![x,y].Resource.Status is GrowthProcess.Fallow)
                    {
                        TargetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
                    }
                }
            }
            TargetMemory = TargetMemory.Distinct().ToList();

            if (Profession is Profession.Forester or Profession.Forager 
                && Mode is ProcessingMode.Harvesting)
                    TargetMemory.Shuffle();

            if (Profession is Profession.Forager)
            {
                Console.Write($"{Profession} in {Mode}-mode is scanning");
                Console.Write($"and has {TargetMemory.Count} targets");
            }
                
        }
        await Task.Delay(scanDuration);     
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

        await Task.Delay(
            Status == Status.Exhausted
                ? SpeedMove * 2
                : SpeedMove);

        if (Status is not Status.Exhausted
            && Position == chosenTarget
            && TargetMemory.Any())
                TargetMemory.RemoveAt(0); 

        if (Profession is Profession.Forager)
        {
            Console.Write($"{Profession} is moving");
        }
        
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
                    await Task.Delay(SpeedWorking);

                    lock (terrain.TreeListLock)
                    {
                        terrain.TreeList.RemoveAll(tree 
                            => tree.Position.X == TargetPosition.X
                            && tree.Position.Y == TargetPosition.Y);
                    }

                    terrain.Grid![TargetPosition.X, TargetPosition.Y].Resource.Status = GrowthProcess.Fallow;
                    terrain.Grid![TargetPosition.X, TargetPosition.Y].Resource.Type = ResourceType.None;

                    Stamina--;

                    /* Status = Status.Exhausted;
                    TargetMemory.Clear(); */
                }
                if (TargetMemory.Count == 0)
                {
                    Status = Status.Exhausted;
                    /* TargetMemorySize = 0; */                     
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
        && terrain.Grid![Position.X, Position.Y].Soil
            is Soil.Fertile
        && terrain.Grid![Position.X, Position.Y].Resource.Type
            is not (ResourceType.Wheat 
                or ResourceType.SpruceWood 
                or ResourceType.OakWood 
                or ResourceType.Blueberry))
        {
            if (randomNumber == randomFactor)
            {
                await Task.Delay(SpeedWorking / 3);
                lock (terrain.TreeListLock)
                {
                    terrain.TreeList.Add(
                        new Tree(PlantType.Spruce, new Position(Position.X, Position.Y), GrowthProcess.Planted, terrain));
                }
            }

            if (!TargetMemory.Any()) Status = Status.Exhausted;
            if (terrain.TreeList.Count >= Terrain.TreeCuttingThreshold)
            {
                
                Mode = ProcessingMode.Harvesting;
                Status = Status.Exhausted;
                TargetMemory.Clear();
            }
            //else await ScanFor(terrain);
        }
    }

    public async Task DoForaging(Terrain terrain)
    {
        if (Profession is Profession.Forager
            && Mode is ProcessingMode.Harvesting 
            && Position.X == TargetPosition.X
            && Position.Y == TargetPosition.Y
            && terrain.Grid![Position.X,Position.Y].Resource.Type == TargetResource
            && terrain.Grid![Position.X, Position.Y].Resource.Status is GrowthProcess.Ripe)
        {
            Console.Write($"{Profession} is foraging");
            //Position = new Position(Position.X - 1, Position.Y);
            await Task.Delay(SpeedWorking);

            if (Specialisation is Specialisation.BlueBerries)
            {                
                    lock (terrain.BushListLock)
                    {
                        Bush bush = terrain.BushList.FirstOrDefault(bush
                            => bush.Position.X == TargetPosition.X
                            && bush.Position.Y == TargetPosition.Y
                            && bush.Status is GrowthProcess.Ripe)
                                ?? throw new InvalidOperationException("bush not found");

                        bush.Status = GrowthProcess.Mature;
                        terrain.Grid![TargetPosition.X, TargetPosition.Y].Resource.Status = GrowthProcess.Mature;

                        Stamina--;
                        /* if (terrain.BushList.Any(bush => bush.Status is GrowthProcess.Ripe))
                            Status = Status.Exhausted; */
                    }
                
            }
            if (Specialisation is Specialisation.Mushrooms)
            {
                foreach (Tree tree in terrain.TreeList)

                if (tree.Mycel is not null)
                {
                    Mushroom mushroom = tree.Mycel.MushroomList.FirstOrDefault(mushroom
                        => mushroom.Position.X == TargetPosition.X
                        && mushroom.Position.Y == TargetPosition.Y
                        && mushroom.Status is GrowthProcess.Ripe)
                            ?? throw new InvalidOperationException("mushroom not found");

                    lock (terrain.TreeListLock)
                    {
                        //tree.Mycel.MushroomList
                        mushroom.Parent.MushroomList.Remove(mushroom);
                        Stamina--;
                        /* if (terrain.TreeList.Any(tree 
                            => tree.Mycel!.MushroomList.Any(mushroom 
                            => mushroom.Status is GrowthProcess.Ripe)))
                                Status = Status.Exhausted; */
                    }
                }
            }
        }

        if (TargetMemory.Count == 0)
        {
            Status = Status.Exhausted;
            TargetMemory.Clear();                 
        }
    }
}
    
    


partial class Animal
{
    public virtual async Task CheckForDanger(Terrain terrain)
    {
        foreach (Human human in terrain.EntityDict.Values.OfType<Human>())
        {
            if (human.Position == Position
            || Math.Abs(Position.X - human.Position.X) <= 3
            && Math.Abs(Position.Y - human.Position.Y) <= 1)
            {
                int fleeDistanceX = Random.Shared.Next(4,7);
                int fleeDistanceY = Random.Shared.Next(2,4);

                if (Position.X <= human.Position.X)
                    fleeDistanceX = 
                        Position.X <= Terrain.MaxSizeX.ClampToAxis(0) / 2
                        ? fleeDistanceX
                        : -fleeDistanceX;
                else if (Position.X > human.Position.X)
                    fleeDistanceX = 
                        Position.X <= Terrain.MaxSizeX.ClampToAxis(0) / 2
                        ? -fleeDistanceX
                        : fleeDistanceX;
                else if (Position.X <= human.Position.X)
                    fleeDistanceX = 
                        Position.X > Terrain.MaxSizeX.ClampToAxis(0) / 2
                        ? -fleeDistanceX
                        : fleeDistanceX;
                else if (Position.X > human.Position.X)
                    fleeDistanceX = 
                        Position.X > Terrain.MaxSizeX.ClampToAxis(0) / 2
                        ? -fleeDistanceX
                        : fleeDistanceX;

                if (Position.Y <= human.Position.Y)
                    fleeDistanceY = 
                        Position.Y <= Terrain.MaxSizeY.ClampToAxis(1) / 2
                        ? fleeDistanceY
                        : -fleeDistanceY;
                else if (Position.Y > human.Position.Y)
                    fleeDistanceY = 
                        Position.Y <= Terrain.MaxSizeY.ClampToAxis(1) / 2
                        ? -fleeDistanceY
                        : fleeDistanceY;
                else if (Position.Y <= human.Position.Y)
                    fleeDistanceY = 
                        Position.Y > Terrain.MaxSizeY.ClampToAxis(1) / 2
                        ? -fleeDistanceY
                        : fleeDistanceY;
                else if (Position.Y > human.Position.Y)
                    fleeDistanceY = 
                        Position.Y > Terrain.MaxSizeY.ClampToAxis(1) / 2
                        ? -fleeDistanceY
                        : fleeDistanceY;

                for (int y = 0; y < fleeDistanceY; y++)
                {
                    for (int x = 0; x < fleeDistanceX; x++)
                    {
                        Position = new Position(Position.X + x, Position.Y + y);
                        await Task.Delay(SpeedMove / 2);
                    }
                }
            }    
        }
    }
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
        //await MoveInCircles();
        //await MoveTo(terrain);
        await ScamperAround(terrain);
        
    }
    
    public override async Task MoveTo(Terrain terrain)
    {
        
    }

    public async Task ScamperAround(Terrain terrain)
    {
        int scamperLengthX = Random.Shared.Next(4,9);
        int scamperLengthY = Random.Shared.Next(2,4);
        bool shouldTurnX;
        bool shouldTurnY;

        for (int y = 0; y < scamperLengthY; y++)
        {
            int randomY = Random.Shared.Next(-1, 2);

            for (int x = 0; x < scamperLengthX; x++)
            {
                int randomX = Random.Shared.Next(-2, 3);
                
                shouldTurnX =
                    Random.Shared.Next(2) == 1
                        ? true
                        : false;
                shouldTurnY =
                    Random.Shared.Next(2) == 1
                        ? true
                        : false;

                if (Position.X + randomX != LastPosition.X
                || shouldTurnX
                && Position.Y + randomY != LastPosition.Y
                || shouldTurnY)
                    Position = new Position(
                        Position.X + randomX, 
                        Position.Y + randomY);
                else continue;
                
                await Task.Delay(SpeedMove);
                await CheckForDanger(terrain);                
            };
        }
        await ScanFor(terrain);
    }

    public override async Task ScanFor(Terrain terrain)
    {
        int focusdirection;
        int focusDuration = 500;
        int pastScanThinking = Random.Shared.Next(600, 1500);
        int scanAmount = Random.Shared.Next(2,7);

        for (int x = 0; x < scanAmount; x++)
        {
            focusdirection = Random.Shared.Next(0,2);

            //await CheckForDanger(terrain);

            if (focusdirection == 0)
            {
                Icon = IconList[4];
                await Task.Delay(focusDuration);
                await CheckForDanger(terrain);
                Icon = IconList[6];
                await Task.Delay(focusDuration);
                await CheckForDanger(terrain);
            }
            else if (focusdirection == 1)
            {
                Icon = IconList[8];
                await Task.Delay(focusDuration);
                await CheckForDanger(terrain);
                Icon = IconList[2];
                await Task.Delay(focusDuration);
                await CheckForDanger(terrain);
            }

            await Task.Delay(pastScanThinking);
        }
    }

    public async Task MoveInCircles()
    {
        Position = new Position(Position.X + 1, Position.Y); // East
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y);
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y);
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y);
        await Task.Delay(200);

        Position = new Position(Position.X + 1, Position.Y + 1); // SE
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y + 1);
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y + 1);
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y + 1);
        await Task.Delay(200);

        Position = new Position(Position.X, Position.Y + 1); // South
        await Task.Delay(200);
        Position = new Position(Position.X, Position.Y + 1);
        await Task.Delay(200);
        Position = new Position(Position.X, Position.Y + 1);
        await Task.Delay(200);
        Position = new Position(Position.X, Position.Y + 1);
        await Task.Delay(200);

        Position = new Position(Position.X - 1, Position.Y + 1); // SW
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y + 1);
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y + 1);
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y + 1);
        await Task.Delay(200);

        Position = new Position(Position.X - 1, Position.Y); // West
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y);
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y);
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y);
        await Task.Delay(200);

        Position = new Position(Position.X - 1, Position.Y - 1); // NW
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y - 1);
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y - 1);
        await Task.Delay(200);
        Position = new Position(Position.X - 1, Position.Y - 1);
        await Task.Delay(200);

        Position = new Position(Position.X, Position.Y - 1); // North
        await Task.Delay(200);
        Position = new Position(Position.X, Position.Y - 1);
        await Task.Delay(200);
        Position = new Position(Position.X, Position.Y - 1);
        await Task.Delay(200);
        Position = new Position(Position.X, Position.Y - 1);
        await Task.Delay(200);

        Position = new Position(Position.X + 1, Position.Y - 1); // NE
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y - 1);
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y - 1);
        await Task.Delay(200);
        Position = new Position(Position.X + 1, Position.Y - 1);
        await Task.Delay(200);
    }

}

