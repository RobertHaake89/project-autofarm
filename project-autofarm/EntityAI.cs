using System;

namespace ProjectAutofarm;

partial class Entity : ITargetable // AI
{
    public virtual void RunSchedule(Terrain terrain)
    {
        
    }
    public void MoveTo()
    {
        
    }
}

partial class Human : Entity
{
    public override void RunSchedule(Terrain terrain)
    {
        if (!IsExhausted)
        {
            if(_targetMemory.Any() == false) ScanFor(terrain);
            MoveTo();
            ExecuteWork(terrain);
        }
    }

    public virtual void ScanFor(Terrain terrain)
    {
        ResourceType target = _targetResource;
        //Console.WriteLine($"Target: {target}");
        //Thread.Sleep(500);

        for (int y = 0; y < Terrain.MaxSizeY; y++)
        for (int x = 0; x < Terrain.MaxSizeX; x++)
        {
            if (terrain.Grid![x,y].Resource.Type == target
            && terrain.Grid![x,y].Resource.Status is GrowthProcess.Ripe)
            {
                //Console.WriteLine($"FOUND RIPE TARGET at {x},{y}");
                Thread.Sleep(500);
                _targetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
            }
        }
        //Console.WriteLine("IS SCANNED");
    }

    public virtual void MoveTo()
    {
        //Console.WriteLine("IS MovingStart");
        if (_targetMemory.Count == 0) return;
        Position chosenTarget = _targetMemory[0];

        if (Position.X < chosenTarget.X && Position.Y < chosenTarget.Y) _targetPosition = new Position(Position.X + 1, Position.Y + 1);
        else if (Position.X > chosenTarget.X && Position.Y > chosenTarget.Y) _targetPosition = new Position(Position.X - 1, Position.Y - 1);
        else if (Position.X < chosenTarget.X) _targetPosition = new Position(Position.X + 1, Position.Y);
        else if (Position.Y < chosenTarget.Y) _targetPosition = new Position(Position.X, Position.Y + 1);
        else if (Position.X > chosenTarget.X) _targetPosition = new Position(Position.X - 1, Position.Y);
        else if (Position.Y > chosenTarget.Y) _targetPosition = new Position(Position.X, Position.Y - 1);

        Position = _targetPosition;

        if (Position == chosenTarget) _targetMemory.RemoveAt(0);

        //Console.WriteLine("IS MovingEnd");
    }

    public void ExecuteWork(Terrain terrain)
    {
        if (Position == _targetPosition)
        {
            if (terrain.Grid![Position.X, Position.Y].Resource.Type == _targetResource)
            {
                
            }
        }
    }
}