using System;
using System.Xml.Serialization;
using Microsoft.VisualBasic;

namespace ProjectAutofarm;

class Terrain
{
    public Tile[,]? Grid {get; private set;} = new Tile[MaxSizeX, MaxSizeY];
    public const int MaxSizeX = 90;
    public const int MaxSizeY = 20;

    //public (int xMax, int yMax) GetMaxSize() => (_maxSizeX, _maxSizeY);

    public void CreateMap()
    {
        CreateTiles();
        CreateDirtFoundation();
        CreateGrassPattern();
        CreateField();
    }

    private void CreateTiles()
    {
        for (int y = 0; y < MaxSizeY; y++)
        {
            for (int x = 0; x < MaxSizeX; x++)
            {
                Grid![x,y] = new Tile((x,y),"empty");
            }
        }
    }

    private void CreateDirtFoundation()
    {
        for (int y = 0; y < MaxSizeY; y++)
        {
            for (int x = 0; x < MaxSizeX; x++)
            {
                Grid![x,y].Texture.TextureName = "bot_dirt1";
            }
        }
    }
    
    private void CreateGrassPattern()
    {
        var OffsetMin = (X:MaxSizeX * 30/100, Y:MaxSizeY * 20/100);
        var OffsetMax = (X:MaxSizeX * 95/100, Y:MaxSizeY * 90/100);

        for (int incY = OffsetMin.Y; incY < OffsetMax.Y; incY++)
        {
            NoiseGenerator(in OffsetMin, out OffsetMin.Y, out OffsetMax.Y);

            for (int incX = OffsetMin.X; incX < OffsetMax.X; incX++)
            {
                NoiseGenerator(in OffsetMin, out OffsetMin.X, out OffsetMax.X);

                Grid![incX,incY].Texture.TextureName = "bot_grass1";
            }
        }
    }

    private void CreateField()
    {
        var OffsetMin = (X:MaxSizeX * 70/100, Y:MaxSizeY * 35/100);
        var OffsetMax = (X:MaxSizeX * 90/100, Y:MaxSizeY * 80/100);

        for (int incY = OffsetMin.Y; incY < OffsetMax.Y; incY++)
        {
            for (int incX = OffsetMin.X; incX < OffsetMax.X; incX++)
            {
                Grid![incX,incY].Texture.TextureName = "field_blank";
            }
        }
    }

    private static void NoiseGenerator(in (int x, int y) position, out int randomMin, out int randomMax)
    {
        int randomFactor = Random.Shared.Next(1,5);
        randomMin = position.x * randomFactor/10;
        randomMax = position.y * randomFactor/10;

        Console.Write($"{randomMin} ");
        Console.WriteLine(randomMax);
    }
}