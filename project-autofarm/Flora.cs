using System;

namespace ProjectAutofarm;

public enum PlantType
{
    None = 0,
    Spruce,
    Oak
}

abstract class Vegetation
{
    public string Name {get; set;} = "Tree";
    public PlantType Type {get; init;}
    public ResourceType Resource {get; set;}
    public GrowthProcess Status {get; set;}
    public Position Position {get; set;}

    public Vegetation(PlantType type, Position position, GrowthProcess status, Terrain terrain)
    {
        Type = type;
        Status = status;
        Position = position;
        terrain.Grid![Position.X, Position.Y ].Resource.Type = Resource;
    }
}

class Tree : Vegetation
{
    public Tree(PlantType type, Position position, GrowthProcess status, Terrain terrain) :base(type, position, status, terrain)
    {
        Resource = type switch
        {
            PlantType.Spruce => ResourceType.SpruceWood,
            PlantType.Oak => ResourceType.OakWood,
            _ => ResourceType.None
        };
    }
    public string[,]? Texture {get => field = GetTreeTexture(); set;}

    public async Task GiveGrowthChance()
    {
        int factor = Type switch
        {
            PlantType.Spruce => 100,
            PlantType.Oak => 200,
            _ => 10
        };

        int RandomNumber = Random.Shared.Next(factor + 1);

        if (Status == GrowthProcess.Ripe) return;
        else if (RandomNumber == factor)
        {
            if (Status == GrowthProcess.Planted) Status = GrowthProcess.Young;
            else if (Status == GrowthProcess.Young) Status = GrowthProcess.Mature;
            else if (Status == GrowthProcess.Mature) Status = GrowthProcess.Ripe;
        }
    }

    public Task SelfSeeding(Terrain terrain, int randomFactor)
    {
        int randomNumber = Random.Shared.Next(randomFactor + 1);

        int spreadSpectrumX;
        int spreadSpectrumY;
        int borderDistanceX = 6;
        int borderDistanceY = 4;
        Position setPosition;

        do
        {
            spreadSpectrumX = Random.Shared.Next(-7, 8);
            spreadSpectrumY = Random.Shared.Next(-3, 4);
            setPosition = new Position(Position.X + spreadSpectrumX,Position.Y + spreadSpectrumY);
        }
        while (spreadSpectrumX < -4
            && spreadSpectrumY < -2
            && spreadSpectrumX > 4
            && spreadSpectrumY > 2);

        setPosition.X = Math.Clamp(setPosition.X, borderDistanceX, Terrain.MaxSizeX - borderDistanceX);
        setPosition.Y = Math.Clamp(setPosition.Y, borderDistanceY, Terrain.MaxSizeY - borderDistanceY - 1);

        if (terrain.TreeList.Count < Terrain.TreeMaxQty
            && randomNumber == randomFactor
            && Type is PlantType.Spruce
            && Status is GrowthProcess.Ripe
            && terrain.Grid![setPosition.X, setPosition.Y]
                .Resource.Type is ResourceType.Fertile/* Soil is Soil.SemiArid or Soil.Fertile */)
        {
            terrain.TreeList.Add(new Tree(Type, setPosition, GrowthProcess.Planted, terrain));
            //terrain.Grid![spreadSpectrumX,spreadSpectrumY].Resource.Type = tree.Resource;
            //terrain.Grid![spreadSpectrumX,spreadSpectrumY].Resource.Status = tree.Status;
        }

        return Task.CompletedTask;
    }

    public string[,] GetTreeTexture()
    {
        int randomizer = Random.Shared.Next(1,3);

        string[,] sprucePlanted =
        {
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip","tree_any_planted", "skip", "skip", "skip"},
        };

        string[,] spruceGerm =
        {
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip","tree_any_germinating", "skip", "skip", "skip"},
        };

        string[,] spruceGrowing =
        {
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip","tree_spruce_growing", "skip", "skip", "skip"},
        };

        string[,] spruceRipe =
        {
            {"skip", "skip", "skip", "tree_spruce_top", "skip", "skip", "skip"},
            {"skip", "skip", "tree_spruce_side_left", "tree_spruce_centre", "tree_spruce_side_right", "skip", "skip"},
            {"skip", "tree_spruce_side_left", "tree_spruce_centre", "tree_spruce_centre", "tree_spruce_centre", "tree_spruce_side_right", "skip"},
            {"skip", "skip", "skip","tree_spruce_stem", "skip", "skip", "skip"},
        };
        /*    ⋀
             /^\
            /^^^\
            /^^^\
              █   */

        string[,] oakArray1 =
        {
            {"skip", "skip", "skip", "skip", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "skip", "skip", "skip"},
            {"skip", "skip", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "skip", "skip"},
            {"skip", "skip", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "skip"},
            {"skip", "skip", "skip", "skip", "tree_oak_branch_left", "tree_oak_stem", "skip", "tree_oak_top", "tree_oak_top", "tree_oak_top", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "tree_oak_stem", "tree_oak_branch_right", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "tree_oak_stem", "skip", "skip", "skip", "skip", "skip"},
        };
        /*    @@@@
            @@@@@@@@
            @@@@@@@@@
              \█ @@@
               █/  
               █*/

        string[,] chosenTexture;

        if (Type is PlantType.Spruce)
        {
            chosenTexture = Status switch
            {
                GrowthProcess.Planted => sprucePlanted,
                GrowthProcess.Young => spruceGerm,
                GrowthProcess.Mature => spruceGrowing,
                GrowthProcess.Ripe => spruceRipe,
                _ => spruceRipe
            };
        }
        else if (Type is PlantType.Oak)
        {
            chosenTexture = randomizer switch
            {
                1 => oakArray1,
                _ => oakArray1
            };
        }
        else chosenTexture = spruceRipe;

        return chosenTexture;
    }
}