using System;
using System.Net;
using System.Threading.Tasks.Dataflow;
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
            for (int incX = OffsetMin.X; incX < OffsetMax.X; incX++)
            {
                //NoiseGenerator(ref incY, in OffsetMin.Y, in OffsetMax.Y);
                NoiseGenerator(incX, incY, in OffsetMin, in OffsetMax);
                Console.WriteLine($"incX {incX}");
                
                
                Grid![incX,incY].Texture.TextureName = "bot_grass1";

                if (incX >= OffsetMax.X) incX = OffsetMin.X;
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

    private static void NoiseGenerator(int incrementX, int incrementY, in (int x, int y) offsetMin, in (int x, int y) offsetMax)
    {
        //if (incrementX < offsetMin.x || incrementX > offsetMax.x) return;
        
        int midIncrementX = (offsetMin.x + offsetMax.x) / 2;
        int midIncrementY = (offsetMin.y + offsetMax.y) / 2;
        //int subIncrementX = offsetMin.x + (incrementX - offsetMin.x);
        //int subIncrementY = offsetMin.y + (incrementY - offsetMin.y);

        Console.WriteLine($"IncrementX {incrementX}");
        Console.WriteLine($"IncrementY {incrementY}");
        Console.WriteLine($"midIncrementX {midIncrementX}");
        Console.WriteLine($"midIncrementY {midIncrementY}\n");
        //Console.WriteLine($"subIncrementX {subIncrementX}");
        //Console.WriteLine($"subIncrementY {subIncrementY}\n");
        //Console.ReadKey();

        if (incrementX < midIncrementX && incrementY < midIncrementY) incrementX = incrementX + (midIncrementX - incrementY * 3);
        else if (incrementX > midIncrementX && incrementY < midIncrementY)
        {
            incrementX = incrementX - (midIncrementX - midIncrementY * 3);
        } 
        
        //else if (incrementX == incrementX) return;
        //else if (incrementX > midIncrementX + midIncrementY * 2) return; //incrementX = incrementX + (midIncrementX - subIncrementY*2);

        int randomizer = Random.Shared.Next(0,6);



        /* if (randomizer == 0) increment -= 2;
        if (randomizer < 2) increment--;

        else if (randomizer > 4) increment++;
        else if (randomizer == 6) increment += 2;  */
    }

    /* private static void NoiseGenerator(in (int x, int y) position, out int randomMin, out int randomMax)
    {
        int randomFactor = Random.Shared.Next(1,5);
        randomMin = position.x * randomFactor/10;
        randomMax = position.y * randomFactor/10;

        Console.Write($"{randomMin} ");
        Console.WriteLine(randomMax);
    } */
}