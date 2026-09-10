using System;

namespace ProjectAutofarm;

class Map
{
    public Tile[,]? Grid {get; set => CreateMap();} = new Tile[_maxSizeX,_maxSizeY];
    private const int _maxSizeX = 20;
    private const int _maxSizeY = 20;

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
                Grid![x,y] = new Tile("grass");
            }
        }
    }
        
    
}