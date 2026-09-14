using System;

namespace ProjectAutofarm;

class Terrain
{
    public Tile[,]? Grid {get; private set;} = new Tile[MaxSizeX, MaxSizeY];
    public const int MaxSizeX = 70;
    public const int MaxSizeY = 20;

    //public (int xMax, int yMax) GetMaxSize() => (_maxSizeX, _maxSizeY);

    public void CreateMap()
    {
        CreateTiles();
    }

    private void CreateTiles()
    {
        for (int y = 0; y < MaxSizeY; y++)
        {
            for (int x = 0; x < MaxSizeX; x++)
            {
                Grid![x,y] = new Tile("bot_grass1",(x,y));
            }
        }
    }
    
    
}