using System;
using System.Security.Cryptography.X509Certificates;

namespace ProjectAutofarm;

class Terrain
{
    public Tile[,]? Grid {get; private set;} = new Tile[MaxSizeX, MaxSizeY];
    public const int MaxSizeX = 90;
    public const int MaxSizeY = MaxSizeX * (16/9) / 4;

    //public (int xMax, int yMax) GetMaxSize() => (_maxSizeX, _maxSizeY);

    public void CreateMap() // xMin, yMin, xMax, yMax, radius?
    {
        CreateTiles();
        CreateDirtFoundation();
        CreateGrassPatch(60,0,100,100,radius:8);
        CreateGrassPatch(40,50,80,100,radius:5);
        CreateGrassPatch(50,0,100,40,radius:5);
        CreateFoliage(0,0,100,100,factor:150);
        CreateSingleGrass(0,0,100,100,factor:200);
        CreateStones(0,0,100,100,factor:100);
        CreateField(70,35,90,80);

        int quantityPine = Random.Shared.Next(4, 8);
        for (int i = 0; i < quantityPine; i++) CreatePineTree(xMin:30, xMax:80, yMin: 0, yMax: 3);
        
        
        CreateFarmHouse(5,0);
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
    
    private void CreateGrassPatch(int xMin, int yMin, int xMax, int yMax, int radius)
    {
        var OffsetMin = (X:MaxSizeX * xMin/100, Y:MaxSizeY * yMin/100); // parameters for min/max size
        var OffsetMax = (X:MaxSizeX * xMax/100, Y:MaxSizeY * yMax/100);

        for (int incY = OffsetMin.Y; incY < OffsetMax.Y; incY++)
        {
            
            for (int incX = OffsetMin.X - GetSimpleNoise(-2,2); incX < OffsetMax.X - GetSimpleNoise(0,3); incX++)
            {
                PatchGenerator(ref incX, ref incY, in OffsetMin, in OffsetMax, out int dx, out int dy);
                
                if (dx * dx + dy * dy <= Math.Pow(radius,2)) Grid![incX,incY].Texture.TextureName = "bot_grass1";
            }
        }
    }

    private void CreateField(int xMin, int yMin, int xMax, int yMax)
    {
        var OffsetMin = (X:MaxSizeX * xMin/100, Y:MaxSizeY * yMin/100);
        var OffsetMax = (X:MaxSizeX * xMax/100, Y:MaxSizeY * yMax/100);

        for (int incY = OffsetMin.Y; incY < OffsetMax.Y; incY++)
        {
            for (int incX = OffsetMin.X; incX < OffsetMax.X; incX++)
            {
                Grid![incX,incY].Ressource = new Wheat(Ressources.GrowthProcess.Sown);
            }
        }
    }

    private static void PatchGenerator(ref int incrementX, ref int incrementY, in (int x, int y) offsetMin, in (int x, int y) offsetMax, out int dx, out int dy)
    {
        int midPointX = (offsetMin.x + offsetMax.x) / 2;
        int midPointY = (offsetMin.y + offsetMax.y) / 2;

        dx = (incrementX - midPointX)/3;
        dy = incrementY - midPointY;
    }

    private void CreateFoliage(int xMin, int yMin, int xMax, int yMax, int factor)
    {
        var OffsetMin = (X:MaxSizeX * xMin/100, Y:MaxSizeY * yMin/100);
        var OffsetMax = (X:MaxSizeX * xMax/100, Y:MaxSizeY * yMax/100);

        string[] foliageArray = {"plant_flower1", "plant_flower2", "plant_flower3", "plant_bush1"};

        for (int incY = OffsetMin.Y; incY < OffsetMax.Y; incY++)
        {
            for (int incX = OffsetMin.X; incX < OffsetMax.X; incX++)
            {
                if (1 == GetSimpleNoise(1,factor) && Grid![incX,incY].Texture.TextureName == "bot_grass1")
                Grid![incX,incY].Texture.TextureName = foliageArray[Random.Shared.Next(0,foliageArray.Length)];
            }
        }
    }

    private void CreateStones(int xMin, int yMin, int xMax, int yMax, int factor)
    {
        var OffsetMin = (X:MaxSizeX * xMin/100, Y:MaxSizeY * yMin/100);
        var OffsetMax = (X:MaxSizeX * xMax/100, Y:MaxSizeY * yMax/100);

        string[] objectArray = {"obj_stone1", "obj_stone2", "obj_stone3", "obj_stone4", "obj_stone5"};

        for (int incY = OffsetMin.Y; incY < OffsetMax.Y; incY++)
        {
            for (int incX = OffsetMin.X; incX < OffsetMax.X; incX++)
            {
                if (1 == GetSimpleNoise(1,factor) && Grid![incX,incY].Texture.TextureName == "bot_dirt1")
                Grid![incX,incY].Texture.TextureName = objectArray[Random.Shared.Next(0,objectArray.Length)];
            }
        }
    }

    private void CreateSingleGrass(int xMin, int yMin, int xMax, int yMax, int factor)
    {
        var OffsetMin = (X:MaxSizeX * xMin/100, Y:MaxSizeY * yMin/100);
        var OffsetMax = (X:MaxSizeX * xMax/100, Y:MaxSizeY * yMax/100);

        string[] grassArray = {"obj_grass1", "obj_grass2", "obj_grass3"};

        for (int incY = OffsetMin.Y; incY < OffsetMax.Y; incY++)
        {
            for (int incX = OffsetMin.X; incX < OffsetMax.X; incX++)
            {
                if (1 == GetSimpleNoise(1,factor) && Grid![incX,incY].Texture.TextureName == "bot_dirt1")
                Grid![incX,incY].Texture.TextureName = grassArray[Random.Shared.Next(0,grassArray.Length)];
            }
        }
    }

    private void CreatePineTree(int xMin, int xMax, int yMin, int yMax)
    {
        string[,] PineArray =
        {
            {"skip", "skip", "skip", "tree_pine_top", "skip", "skip", "skip"},
            {"skip", "skip", "tree_pine_side_left", "tree_pine_centre", "tree_pine_side_right", "skip", "skip"},
            {"skip", "tree_pine_side_left", "tree_pine_centre", "tree_pine_centre", "tree_pine_centre", "tree_pine_side_right", "skip"},
            {"skip", "skip", "skip","tree_pine_stem", "skip", "skip", "skip"},
        };

        int posX = Random.Shared.Next(xMin, xMax + 1);
        int posY = Random.Shared.Next(yMin, yMax + 1);
        

        /* int distX = randomCoordX; // twisted coords, needs rework!
        int distY = randomCoordY; */

        for (int incY = 0; incY < PineArray.GetLength(0); incY++)
        {
            for (int incX = 0; incX < PineArray.GetLength(1); incX++)
            {
                if (PineArray[incY,incX] != "skip")
                Grid![incX + posX,incY + posY].Texture.TextureName = PineArray[incY,incX];

                /*   ⋀
                    /^\
                   /^^^\
                   /^^^\
                     █ */
            }
        }
    }
    
    private void CreateFarmHouse(int posX, int posY)
    {
        string[,] farmHouseArray =
        {
            {"empty","struct_roof1", "struct_roof2", "struct_roof1", "struct_roof2","struct_roof1", "struct_roof2", "empty","empty"},
            {"struct_roof2", "struct_roof1", "struct_roof2","struct_roof1", "struct_roof2", "struct_roof1", "struct_beam_diagonal", "struct_beam_vert2","empty"},
            {"struct_beam_vert1","struct_beam_horz1","struct_window1","struct_door1", "empty","struct_beam_horz1", "struct_beam_vert1", "struct_beam_diagonal", "empty"},
            {"struct_floor1", "struct_floor1", "struct_floor1", "struct_floor1", "struct_floor1", "struct_floor1", "struct_floor1","empty", "empty"},
        };

        int distX = posX; // twisted coords, needs rework!
        int distY = posY;

        for (int incY = 0; incY < farmHouseArray.GetLength(0); incY++)
        {
            for (int incX = 0; incX < farmHouseArray.GetLength(1); incX++)
            {
                Grid![incX + distX,incY + distY].Texture.TextureName = farmHouseArray[incY,incX];
            }
        }
    }

    private static int GetSimpleNoise(int min, int max) => Random.Shared.Next(min, max + 1);
}