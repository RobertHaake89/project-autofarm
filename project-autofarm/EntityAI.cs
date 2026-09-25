using System;

namespace ProjectAutofarm;

partial class Entity : ITargetable // AI
{
    public void MoveTo()
    {
        
    }
}

partial class Human : Entity
{
    public void RunSchedule(Terrain terrain)
    {
        if (!IsExhausted)
        {
            if(_targetMemory.Any() == false) ScanFor(terrain);
            MoveTo();
            ExecuteWork();
        }
    }

    public virtual void ScanFor(Terrain terrain)
    {
        Resource target = _targetResource;

        int x = 0;
        int y = 0;

        for (; y < Terrain.MaxSizeY; y++)
        for (; x < Terrain.MaxSizeX; x++)
        {
            if (terrain.Grid![x,y].Position == target.Position)
            {
                _targetMemory.Add(new Position(terrain.Grid[x,y].Position.X, terrain.Grid[x,y].Position.Y));
            }
        }
    }

    public virtual Position MoveTo()
    {
        Position chosenTarget = _targetMemory[0];

        if (Position.X < chosenTarget.X) return new Position(Position.X + 1, Position.Y);
        else if (Position.Y < chosenTarget.Y) return new Position(Position.X, Position.Y + 1);

        if (Position.X > chosenTarget.X) return new Position(Position.X - 1, Position.Y);
        else if (Position.Y > chosenTarget.Y) return new Position(Position.X, Position.Y - 1);

        return Position;
    }

    public void ExecuteWork()
    {
        
    }
}