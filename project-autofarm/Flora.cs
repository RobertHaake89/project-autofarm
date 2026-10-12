using System;

namespace ProjectAutofarm;

public enum TreeType
{
    None = 0,
    Spruce,
    Oak
}
class Tree
{
    public string Name {get; set;} = "Tree";
    public string[,]? Texture {get => field = GetTreeTexture(); set;}
    public TreeType Type {get; init;}
    public ResourceType Resource {get; set;}
    public GrowthProcess Status {get; set;}
    public Position Position {get; set;}

    public Tree(TreeType type, Position position, GrowthProcess status, Terrain terrain)
    {
        Type = type;
        Status = status;
        Position = position;
        Resource = type switch
        {
            TreeType.Spruce => ResourceType.SpruceWood,
            TreeType.Oak => ResourceType.OakWood,
            _ => ResourceType.None
        };

        terrain.Grid![Position.X, Position.Y ].Resource.Type = Resource;
    }

    /* public void CreateTree(TreeType type, Position position, Terrain terrain)
    {
        if (Type == TreeType.Spruce) terrain.TreeList.Add(new Tree(TreeType.Spruce, new Position(position.X, position.Y), GrowthProcess.Planted));
        if (type == TreeType.Oak) terrain.TreeList.Add(new Tree(TreeType.Oak, new Position(position.X, position.Y), GrowthProcess.Planted));
        
        terrain.Grid![Position.X, Position.Y ].Resource.Type = Resource;
    }

    public void SetTileResource(Terrain terrain)
    {
        terrain.Grid![Position.X, Position.Y ].Resource.Type = Resource;
    } */

    public async Task GiveGrowthChance()
    {
        int factor = Type switch
        {
            TreeType.Spruce => 100,
            TreeType.Oak => 200,
            _ => 10
        };

        int RandomNumber = Random.Shared.Next(0,factor + 1);

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
        //await Task.Delay(1000);

        int randomNumber = Random.Shared.Next(0, randomFactor + 1);

        int spreadSpectrumX;
        int spreadSpectrumY;
        int borderDistanceX = 6;
        int borderDistanceY = 4;
        Position setPosition;

        do
        {
            spreadSpectrumX = Random.Shared.Next(-7, 7);
            spreadSpectrumY = Random.Shared.Next(-3, 4);
            setPosition = new Position(Position.X + spreadSpectrumX,Position.Y + spreadSpectrumY);
        }
        while (spreadSpectrumX < -3
        && spreadSpectrumY < -1
        && spreadSpectrumX > 3
        && spreadSpectrumY > 2);

        //Console.Write(setPosition);
        setPosition.X = Math.Clamp(setPosition.X, borderDistanceX, Terrain.MaxSizeX - borderDistanceX);
        setPosition.Y = Math.Clamp(setPosition.Y, borderDistanceY, Terrain.MaxSizeY - borderDistanceY - 1);

        if (terrain.TreeList.Count < Terrain.TreeMaxQty
            && randomNumber == randomFactor
            && Type is TreeType.Spruce
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

        if (Type is TreeType.Spruce)
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
        else if (Type is TreeType.Oak)
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