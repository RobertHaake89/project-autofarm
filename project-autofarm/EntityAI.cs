using System;

namespace ProjectAutofarm;

partial class Entity // AI
{
    public void ScanFor(Terrain terrain, ITargetable target)
    {   

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

    public virtual void Move(Position targetPos)
    {
        if (Position.X < targetPos.X) new Position(Position.X + 1, Position.X);
        else if (Position.Y < targetPos.Y) new Position(Position.Y + 1, Position.Y);

        if (Position.X > targetPos.X) new Position(Position.X - 1, Position.X);
        else if (Position.Y > targetPos.Y) new Position(Position.Y - 1, Position.Y);
    }
}