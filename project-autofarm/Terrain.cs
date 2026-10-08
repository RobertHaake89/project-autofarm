using System;
using System.ComponentModel;

namespace ProjectAutofarm;

partial class Terrain // Data and Parameters
{
    public Tile[,]? Grid {get; private set;} = new Tile[MaxSizeX, MaxSizeY];
    public const int MaxSizeX = 90;
    public const int MaxSizeY = MaxSizeX * (16/9) / 4;
    public List<Tree> TreeList {get; set;} = new List<Tree>();

    public void CreateMap() // xMin, yMin, xMax, yMax, radius?
    {
        CreateTiles();
        CreateDirtFoundation();
        CreateGrassPatch(60,0,100,100,radius:8);
        CreateGrassPatch(40,50,80,100,radius:5);
        CreateGrassPatch(50,0,100,40,radius:5);
        CreateFoliage(0,0,100,100,factor:80);
        CreateGrassShafts(0,0,100,100,factor:200);
        CreateStones(0,0,100,100,factor:100);
        CreateField(70,35,90,80);

        CreateTrees(TreeType.Spruce, qtyMin: 5, qtyMax: 10, xMin:MaxSizeX / 3, yMin: 4, xMax: MaxSizeX - 5, yMax: MaxSizeY / 3);
        CreateTrees(TreeType.Oak, qtyMin: 1, qtyMax: 1, xMin:5 , yMin: MaxSizeY * 2 / 3, xMax: MaxSizeX * 2 / 3, yMax: MaxSizeY - 1);

        CreateMushrooms(0,0,95,30,factor:60);
        
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
                Grid![incX,incY].Resource = new Resource(ResourceType.Wheat, GrowthProcess.Fallow, (incX,incY));
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

    private void CreateTrees(TreeType treeType,int qtyMin, int qtyMax, int xMin, int yMin, int xMax, int yMax)
    {
        int quantityTrees = Random.Shared.Next(qtyMin, qtyMax + 1);
        int startingIndex = TreeList.Count;
        int maxQuantityTrees = startingIndex + quantityTrees;

        for (int treeIndex = startingIndex; treeIndex < maxQuantityTrees; treeIndex++)
        {
            TreeList.Add(new Tree(treeType, new Position(0,0), GrowthProcess.Ripe));
            int posX = Random.Shared.Next(xMin, xMax);
            int posY = Random.Shared.Next(yMin, yMax);

            for (int incY = 0; incY < TreeList[treeIndex].Texture!.GetLength(0); incY++)
            {
                for (int incX = 0; incX < TreeList[treeIndex].Texture!.GetLength(1); incX++)
                {
                    if (TreeList[treeIndex].Texture![incY,incX] != "skip")
                    {
                        TreeList[treeIndex].Position = new Position(posX, posY);
                    }
                    
                }
            }
        }

        TreeList.Distinct();
        TreeList.Sort((a,b) => a.Position.Y.CompareTo(b.Position.Y));

        foreach (Tree tree in TreeList)
        {
            Grid![tree.Position.X,tree.Position.Y].Resource.Type = tree.Resource;
            Grid![tree.Position.X, tree.Position.Y].Resource.Status = tree.Status;
        }
        
    }

    private void CreateMushrooms(int xMin, int yMin, int xMax, int yMax, int factor)
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
