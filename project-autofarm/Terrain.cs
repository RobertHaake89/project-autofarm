using System;

namespace ProjectAutofarm;

partial class Terrain // Data and Parameters
{
    public Tile[,]? Grid {get; private set;} = new Tile[MaxSizeX, MaxSizeY];
    public const int MaxSizeX = 90;
    public const int MaxSizeY = MaxSizeX * (16/9) / 4;

    public const int TreeMaxQty = SpruceMaxQty + OakMaxQty;
    public const int SpruceMaxQty = 20;
    public const int OakMaxQty = 1;
    public const int TreeCuttingThreshold = TreeMaxQty * 7 / 10;
    public const int MycelMaxQty = 3;
    public const int BlueberryBushMaxQty = 3;
    public List<Tree> TreeList {get; set;} = [];
    public readonly object TreeListLock = new();
    public List<Bush> BushList {get; set;} = [];
    public readonly object BushListLock = new();
    public List<Tile> CropTileList {get; set;} = [];
    //public readonly object CropTilesLock = new();
    
    public List<Structure> StructureList {get; set;} = [];
    public Dictionary<string, Entity> EntityDict = [];

    public void CreateMap() // xMin, yMin, xMax, yMax, radius?
    {
        CreateTiles();

        CreateDirtFoundation();
        CreateSemiAridSoil();
        CreateGrassPatch(xMin: 50,yMin: 0,xMax: 100,yMax: 40,radius:6);
        CreateGrassPatch(xMin: 60, yMin: 0,xMax: 100,yMax: 100,radius:10);
        CreateGrassPatch(xMin: 35,yMin: 50,xMax: 80,yMax: 100,radius:6);
        CreateFoliage(0,0,100,100,factor:80);
        CreateGrassShafts(0,0,100,100,factor:200);
        CreateStones(0,0,100,100,factor:100);
        CreateField(xMin: 67,yMin: 35,xMax: 87,yMax: 80);
        
        CreateFarmHouse(5,0);
    }

}

partial class Terrain // Creation Methods
{
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
                Grid![x.ClampToAxis(0),y.ClampToAxis(1)].Soil = Soil.Barren;
                Grid![x,y].Texture.TextureName = "bot_dirt1";
            }
        }
    }

    private void CreateSemiAridSoil()
    {
        for (int y = 0; y < MaxSizeY; y++)
        {
            for (int x = MaxSizeX / 3; x < MaxSizeX; x++)
            {
                Grid![x.ClampToAxis(0),y.ClampToAxis(1)].Soil = Soil.SemiArid;
            }
        }
    }

    private void CreateGrassPatch(int xMin, int yMin, int xMax, int yMax, int radius)
    {
        var OffsetMin = (X:MaxSizeX * xMin/100, Y:MaxSizeY * yMin/100);
        var OffsetMax = (X:MaxSizeX * xMax/100, Y:MaxSizeY * yMax/100);

        for (int incY = OffsetMin.Y; incY < OffsetMax.Y; incY++)
        {
            for (int incX = OffsetMin.X - GetSimpleNoise(-2,2); incX < OffsetMax.X - GetSimpleNoise(0,3); incX++)
            {
                PatchGenerator(ref incX, ref incY, in OffsetMin, in OffsetMax, out int dx, out int dy);
                
                if (dx * dx + dy * dy <= Math.Pow(radius, 2))
                {
                    Grid![
                        incX.ClampToAxis(0),
                        incY.ClampToAxis(1)].
                            Soil = Soil.Fertile;
                    Grid![
                        incX.ClampToAxis(0),
                        incY.ClampToAxis(1)].
                            Resource.Type = ResourceType.Humus;
                    Grid![incX,incY].Texture.TextureName = "bot_grass1";
                }
                    
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
                Grid![incX,incY].Resource = new Resource(ResourceType.Wheat, GrowthProcess.Fallow, (incX,incY));
                CropTileList.Add(Grid![incX,incY]);
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

        string[] foliageArray = {"plant_flower1", "plant_flower2", "plant_flower3","plant_flower4", "plant_bush1"};

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

    private void CreateGrassShafts(int xMin, int yMin, int xMax, int yMax, int factor)
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

    private void CreateTrees(PlantType treeType,int qtyMin, int qtyMax, int xMin, int yMin, int xMax, int yMax)
    {
        int quantityTrees = Random.Shared.Next(qtyMin, qtyMax);
        int startingIndex = TreeList.Count;
        int maxQuantityTrees = startingIndex + quantityTrees;

        int posX = Random.Shared.Next(xMin, xMax + 1);
        int posY = Random.Shared.Next(yMin, yMax + 1);

        for (int incY = 0; incY < qtyMax; incY++)
        {
            TreeList.Add(new Tree(treeType, new Position(posX,posY), GrowthProcess.Ripe, this));
        }
        

        TreeList.Distinct();
    }

    /* private void CreateMushrooms(int xMin, int yMin, int xMax, int yMax, int factor)
    {
        var OffsetMin = (X:MaxSizeX * xMin/100, Y:MaxSizeY * yMin/100);
        var OffsetMax = (X:MaxSizeX * xMax/100, Y:MaxSizeY * yMax/100);

        string[] mushroomArray = {"forage_mushroom1", "forage_mushroom2"};

        for (int incY = OffsetMin.Y; incY < OffsetMax.Y; incY++)
        {
            for (int incX = OffsetMin.X; incX < OffsetMax.X; incX++)
            {
                if (1 == GetSimpleNoise(1,factor) && Grid![incX,incY].Texture.TextureName == "bot_grass1")
                Grid![incX,incY].Texture.TextureName = mushroomArray[Random.Shared.Next(0,mushroomArray.Length)];
            }
        }
    } */
    
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
