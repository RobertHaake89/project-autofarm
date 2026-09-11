using System;

namespace ProjectAutofarm;

class Terrain
{
    public Tile[,]? Grid {get; private set;} = new Tile[_maxSizeX,_maxSizeY];
    public const int MaxSizeX = 20;
    public const int MaxSizeY = 20;

    //public (int xMax, int yMax) GetMaxSize() => (_maxSizeX, _maxSizeY);

    public void CreateMap()
    {
        CreateTiles();
    }

    private void CreateTiles()
    {
        for (int y = 0; y < _maxSizeX; y++)
        {
            for (int x = 0; x < _maxSizeY; x++)
            {
                Grid![x,y] = new Tile("bot_dirt1",(x,y));
            }
        }
    }
        
    
}