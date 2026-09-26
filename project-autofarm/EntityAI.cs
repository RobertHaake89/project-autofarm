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

        int x = 0;
        int y = 0;

        for (; y < Terrain.MaxSizeY; y++)
        for (; x < Terrain.MaxSizeX; x++)
        {
            if (terrain.Grid![x,y].Resource.Type == target)
            {
                _targetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
            }
        }
    }

    public virtual void MoveTo()
    {
        Position chosenTarget = _targetMemory[0];

        if (Position.X < chosenTarget.X) _targetPosition = new Position(Position.X + 1, Position.Y);
        else if (Position.Y < chosenTarget.Y) _targetPosition = new Position(Position.X, Position.Y + 1);

        if (Position.X > chosenTarget.X) _targetPosition = new Position(Position.X - 1, Position.Y);
        else if (Position.Y > chosenTarget.Y) _targetPosition = new Position(Position.X, Position.Y - 1);

        _targetPosition = Position;
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