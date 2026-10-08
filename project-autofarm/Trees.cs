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
    public string[,]? Texture {get; init;}
    public TreeType Type {get; init;}
    public ResourceType Resource {get; set;}
    public GrowthProcess Status {get; set;}
    public Position Position {get; set;}

    public Tree(TreeType type, Position position, GrowthProcess status = GrowthProcess.Ripe)
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
        Texture = GetTreeTexture();
    }

    public void CreateTree(TreeType type, Position position, Terrain terrain)
    {
        if (type == TreeType.Spruce) terrain.TreeList.Add(new Tree(TreeType.Spruce, new Position(position.X, position.Y), GrowthProcess.Planted));
        if (type == TreeType.Oak) terrain.TreeList.Add(new Tree(TreeType.Oak, new Position(position.X, position.Y), GrowthProcess.Planted));
    }

    public string[,] GetTreeTexture()
    {
        int randomizer = Random.Shared.Next(1,3);

        string[,] spruceArray1 =
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
            chosenTexture = randomizer switch
            {
                _ => spruceArray1
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
        else chosenTexture = spruceArray1;

        return chosenTexture;
    }
}