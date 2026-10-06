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
    public string[,]? Texture {get => field; 
    init => field = GetTreeTexture();}
    public TreeType Type {get; init;}
    public ResourceType Resource {get; set;} = ResourceType.Wood;
    public Position Position {get; init;}

    public Tree(TreeType type, Position position)
    {
        Type = type;
        Position = position;
    }

    public void CreateTree(TreeType type, Position position)
    {
        if (type == TreeType.Spruce) new Tree(TreeType.Spruce, new Position(position.X, position.Y));
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

        string[,] oakArray1 =
        {
            {"skip", "skip", "skip", "skip", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "skip", "skip", "skip"},
            {"skip", "skip", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "skip", "skip"},
            {"skip", "skip", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "tree_oak_top", "skip"},
            {"skip", "skip", "skip", "skip", "tree_oak_branch_left", "tree_oak_stem", "skip", "tree_oak_top", "tree_oak_top", "tree_oak_top", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "tree_oak_stem", "tree_oak_branch_right", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "tree_oak_stem", "skip", "skip", "skip", "skip", "skip"},
        };

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