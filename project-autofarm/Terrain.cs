using System;

namespace ProjectAutofarm;

class Terrain
{
    public Tile[,]? Grid {get; private set;} = new Tile[_maxSizeX,_maxSizeY];
    private const int _maxSizeX = 20;
    private const int _maxSizeY = 20;

    //public (int xMax, int yMax) GetMaxSize() => (_maxSizeX, _maxSizeY);

    public void CreateMap()
    {
        CreateBottom();
    }

    private void CreateBottom()
    {
        for (int y = 0; y < _maxSizeX; y++)
        {
            for (int x = 0; x < _maxSizeY; x++)
            {
                Grid![x,y] = new Tile((x,y));
            }
        }
    }
        
    
}