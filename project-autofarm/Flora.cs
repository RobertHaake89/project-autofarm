using System.Diagnostics;

namespace ProjectAutofarm;

public enum PlantType
{
    None = 0,
    Spruce,
    Oak,
    Mycel,
    Mushroom,
    Blueberry
}

abstract class Vegetation
{
    public string Name {get; set;}
    public PlantType Type {get; init;}
    public ResourceType Resource {get; set;}
    public GrowthProcess Status {get; set;}
    public virtual Position Position {get; set
        {
            field = new Position(
                Math.Clamp(value.X, 1, Terrain.MaxSizeX - 1),
                Math.Clamp(value.Y, 1, Terrain.MaxSizeY - 1));
        }}

    public Vegetation(PlantType type, Position position, GrowthProcess status, Terrain terrain)
    {
        Name = Type.ToString();
        Type = type;
        Status = status;
        Position = position;
        terrain.Grid![Position.X, Position.Y ].Resource.Type = Resource;
    }

    public async Task GiveGrowthChance(Terrain terrain)
    {
        if (Status == GrowthProcess.Ripe)
            return;

        int factor = Type switch
        {
            PlantType.Spruce => 100,
            PlantType.Oak => 200,
            PlantType.Blueberry => 150,
            _ => 10
        };

        int RandomNumber = Random.Shared.Next(factor + 1);

        if (RandomNumber == factor)
        {
            if (Status == GrowthProcess.Planted) Status = GrowthProcess.Young;
            else if (Status == GrowthProcess.Young)
            {
                if (terrain.Grid![
                    (Position.X - 1).ClampToAxis(0),
                    (Position.Y - 1).ClampToAxis(1)].
                        Resource.Type is ResourceType.SpruceWood or ResourceType.OakWood
                || terrain.Grid[
                    (Position.X - 1).ClampToAxis(0),
                    Position.Y.ClampToAxis(1)].
                        Resource.Type is ResourceType.SpruceWood or ResourceType.OakWood
                || terrain.Grid[
                    Position.X.ClampToAxis(0),
                    (Position.Y - 1).ClampToAxis(1)].
                        Resource.Type is ResourceType.SpruceWood or ResourceType.OakWood
                || terrain.Grid[
                    Position.X.ClampToAxis(0),
                    (Position.Y + 1).ClampToAxis(1)].
                        Resource.Type is ResourceType.SpruceWood or ResourceType.OakWood
                || terrain.Grid[
                    (Position.X + 1).ClampToAxis(0),
                    Position.Y.ClampToAxis(1)].
                        Resource.Type is ResourceType.SpruceWood or ResourceType.OakWood
                || terrain.Grid[(
                    Position.X + 1).ClampToAxis(0),
                    (Position.Y + 1).ClampToAxis(1)].
                        Resource.Type is ResourceType.SpruceWood or ResourceType.OakWood)
                {

                    object chosenLock = Type switch
                    {
                        PlantType.Spruce or PlantType.Oak => terrain.TreeListLock,
                        PlantType.Blueberry => terrain.BushListLock,
                        _ => terrain.TreeListLock
                    };

                    lock (chosenLock)
                    {
                        if (Type is PlantType.Spruce or PlantType.Oak)
                            terrain.TreeList.Remove((Tree)this);
                        if (Type is PlantType.Blueberry)
                            terrain.BushList.Remove((Bush)this);

                        terrain.Grid[Position.X, Position.Y].Resource.Type = ResourceType.None;
                        terrain.Grid![Position.X, Position.Y].Resource.Status = GrowthProcess.Fallow;
                    }
                }
                else
            {
                Status = GrowthProcess.Mature;
            }
            }
        }
        else if (Status == GrowthProcess.Mature)
        {
            Status = GrowthProcess.Ripe;
            terrain.Grid![Position.X,Position.Y].Resource.Type = Resource;
            terrain.Grid![Position.X, Position.Y].Resource.Status = Status;
        }            
    }
}

class Tree : Vegetation
{
    public override Position Position {get; set =>
        field = Type switch
        {
            PlantType.Spruce => new Position(
                Math.Clamp(value.X, 4, Terrain.MaxSizeX - 3), 
                Math.Clamp(value.Y, 4, Terrain.MaxSizeY - 1)),
            PlantType.Oak => new Position(
                Math.Clamp(value.X, 7, Terrain.MaxSizeX - 6), 
                Math.Clamp(value.Y, 7, Terrain.MaxSizeY - 1)),
            _ => new Position(
                Math.Clamp(value.X, 5, Terrain.MaxSizeX - 5), 
                Math.Clamp(value.Y, 5, Terrain.MaxSizeY - 5))
        };}
    public Mycel? Mycel {get; set;}
    public string[,]? Texture {get => field = GetTreeTexture(); set;}
    public Tree(PlantType type, Position position, GrowthProcess status, Terrain terrain) :base(type, position, status, terrain)
    {
        Type = type;
        Resource = type switch
        {
            PlantType.Spruce => ResourceType.SpruceWood,
            PlantType.Oak => ResourceType.OakWood,
            _ => ResourceType.None
        };
    }

    public async Task CreateMycel(Terrain terrain, int randomFactor)
    {
        int randomNumber = Random.Shared.Next(randomFactor + 1);

        if (terrain.TreeList.ToList().Count(
                tree => tree.Type is PlantType.Spruce or PlantType.Oak) 
                >= Terrain.SpruceMaxQty * 90 / 100
            && terrain.TreeList.ToList().Count(
                tree => tree.Mycel is not null) <= Terrain.MycelMaxQty
            && Mycel is null
            && Status is GrowthProcess.Ripe
            && Type is PlantType.Spruce or PlantType.Oak
            
            && randomNumber == randomFactor)
        {
            Mycel = new Mycel(this, terrain);
        }   
    }

    public string[,] GetTreeTexture()
    {
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

        string[,] oakPlanted =
        {
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "tree_any_planted", "skip", "skip", "skip", "skip", "skip"}
        };

        string[,] oakGerm =
        {
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "tree_any_germinating", "skip", "skip", "skip", "skip", "skip"}
        };

        string[,] oakGrowing =
        {
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip", "skip"},
            {"skip", "skip", "skip", "skip", "skip", "tree_oak_growing", "skip", "skip", "skip", "skip", "skip"}
        };

        string[,] oakRipe =
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
            chosenTexture = Status switch
            {
                GrowthProcess.Planted => oakPlanted,
                GrowthProcess.Young => oakGerm,
                GrowthProcess.Mature => oakGrowing,
                GrowthProcess.Ripe => oakRipe,
                _ => oakRipe
            };
        }
        else chosenTexture = spruceRipe;

        return chosenTexture;
    }

    public async Task SelfSeeding(Terrain terrain, int randomFactor)
    {
        int randomNumber = Random.Shared.Next(randomFactor + 1);

        int spreadSpectrumX;
        int spreadSpectrumY;
        int borderDistanceX = 6;
        int borderDistanceY = 1;
        Position setPosition;

        do
        {
            spreadSpectrumX = Random.Shared.Next(-12, 13);
            spreadSpectrumY = Random.Shared.Next(-6, 7);
            setPosition = new Position(
                Position.X + spreadSpectrumX,Position.Y + spreadSpectrumY);
        }
        while (spreadSpectrumX < -6
            && spreadSpectrumY < -3
            && spreadSpectrumX > 7
            && spreadSpectrumY > 4);

        setPosition.X = Math.Clamp(
            setPosition.X, borderDistanceX, 
            Terrain.MaxSizeX - borderDistanceX);
        setPosition.Y = Math.Clamp(
            setPosition.Y, borderDistanceY, 
            Terrain.MaxSizeY - borderDistanceY - 1); // If removing, Pos Property Setter can't prevent crash !!

        if (terrain.TreeList.Count < Terrain.TreeMaxQty
            && randomNumber == randomFactor
            && Type is PlantType.Spruce
            && Status is GrowthProcess.Ripe
            && terrain.Grid![setPosition.X, setPosition.Y]
                .Soil is Soil.Fertile or Soil.SemiArid
            && terrain.Grid![setPosition.X, setPosition.Y]
                .Resource.Type is not (ResourceType.Wheat or ResourceType.SpruceWood or ResourceType.OakWood))
        {
            PlantType typeModifier =
                terrain.TreeList.Count(tree => tree.Type == PlantType.Oak) < Terrain.OakMaxQty
                && terrain.TreeList.Count(tree => tree.Type is PlantType.Spruce) == Terrain.SpruceMaxQty
                && terrain.TreeList.Count(tree => tree.Status is GrowthProcess.Ripe) == Terrain.SpruceMaxQty
                    ? PlantType.Oak 
                    : PlantType.Spruce;

                    if (typeModifier == PlantType.Oak) Math.Clamp(Position.Y, 7, Terrain.MaxSizeY - 1);

            lock (terrain.TreeListLock)
            {
                terrain.TreeList.Add(
                    new Tree(typeModifier, setPosition, GrowthProcess.Planted, terrain));
            }
        }        

        await Task.CompletedTask;
    }
}


class Mycel : Vegetation
{
    public Tree Parent;
    public const int MycelMaxMushrooms = 6;
    public const int MushroomMaxRadius = 10;
    public List<Mushroom> MushroomList {get; set;} = [];
    public Mycel(Tree parent, Terrain terrain) : base(PlantType.Mycel, parent.Position, GrowthProcess.Planted, terrain)
    {
        Parent = parent;
    }

    public async Task AddMushrooms(Terrain terrain, int randomFactor)
    {
        int randomNumber = Random.Shared.Next(randomFactor + 1);
        int radiusPosX = Random.Shared.Next(-MushroomMaxRadius, MushroomMaxRadius + 1);
        int radiusPosY = Random.Shared.Next(-(MushroomMaxRadius / 3), (MushroomMaxRadius + 1) / 3);

        Position setPosition = new Position(
            Math.Clamp(Parent.Position.X + radiusPosX, 1, Terrain.MaxSizeX - 1),
            Math.Clamp(Parent.Position.Y + radiusPosY, 1, Terrain.MaxSizeY - 1));

        if (randomNumber == randomFactor
            && radiusPosX != 0
            && radiusPosY != 0
            && MushroomList.Count <= MycelMaxMushrooms
            && terrain.Grid![setPosition.X, setPosition.Y].Resource.Type is ResourceType.None)
        {
            MushroomList.Add(
                new Mushroom(this,new Position(setPosition.X, setPosition.Y), terrain));
            MushroomList[^1].Position = MushroomList[^1].Position; // To trigger Clamped Setter

            terrain.Grid![setPosition.X, setPosition.Y].Resource.Type = Resource;
            //Console.Write("has been added");
        }
                
    }
}

class Mushroom : Vegetation
{
    public Mycel Parent;
    public char Icon {get
    {
        if (Status is GrowthProcess.Ripe)
            return Parent.Parent.Type is PlantType.Oak
                ? '♣' 
                : '♧';
        else if (Status is GrowthProcess.Mature) return '⌃';
        else if (Status is GrowthProcess.Young) return '^';
        else if (Status is GrowthProcess.Planted) return '.';

        else return '?';
    }}
    
    public Mushroom(Mycel parent, Position position, Terrain terrain) : base(PlantType.Mushroom, position, GrowthProcess.Planted, terrain)
    {
        Parent = parent;      
    }

    public async Task GiveGrowthChance(int randomFactor)
    {
        int randomNumber = Random.Shared.Next(randomFactor + 1);

        if (Status is not GrowthProcess.Ripe
            && randomNumber == randomFactor)
        {
            Status = Status switch 
            {
                GrowthProcess.Mature => GrowthProcess.Ripe,
                GrowthProcess.Young => GrowthProcess.Mature,
                GrowthProcess.Planted => GrowthProcess.Young,
                _ => GrowthProcess.Planted
            };
        }
    }
}

class Bush : Vegetation
{
    public int MaxClusterSize {get; init;} = Random.Shared.Next(3, 8);
    public int CurrentClusterSize = 0;
    public Bush(PlantType type, Position position, Terrain terrain) : base(type, position, GrowthProcess.Planted, terrain)
    {
        Type = type;
        Position = position;
    }

    public string[,]? Texture {get => field = GetBushTexture(); set;}

    public async Task SelfSeeding(Terrain terrain, int randomFactor)
    {
        if (CurrentClusterSize == MaxClusterSize) return;

        int randomNumber = Random.Shared.Next(randomFactor + 1);

        int spreadSpectrumX;
        int spreadSpectrumY;
        int borderDistanceX = 1;
        int borderDistanceY = 1;
        Position setPosition;

        do
        {
            spreadSpectrumX = Random.Shared.Next(-2, 3);
            spreadSpectrumY = Random.Shared.Next(-1, 2);
            setPosition = new Position(
                Position.X + spreadSpectrumX,Position.Y + spreadSpectrumY);
        }
        while (spreadSpectrumX != 0
            && spreadSpectrumY != 0);

        setPosition.X = Math.Clamp(
            setPosition.X, borderDistanceX, 
            Terrain.MaxSizeX - borderDistanceX);
        setPosition.Y = Math.Clamp(
            setPosition.Y, borderDistanceY, 
            Terrain.MaxSizeY - borderDistanceY - 1); // If removing, Pos Property Setter can't prevent crash !!

        if (terrain.BushList.Count < Terrain.BushMaxQty
            && randomNumber == randomFactor
            && Type is PlantType.Blueberry
            && Status is GrowthProcess.Ripe
            && terrain.Grid![setPosition.X, setPosition.Y]
                .Soil is Soil.Fertile or Soil.SemiArid
            && terrain.Grid![setPosition.X, setPosition.Y]
                .Resource.Type is not (ResourceType.Wheat or ResourceType.SpruceWood or ResourceType.OakWood or ResourceType.Blueberry))
        {
            lock (terrain.BushListLock)
            {
                terrain.BushList.Add(
                    new Bush(PlantType.Blueberry, setPosition, terrain));
                    CurrentClusterSize++;
            }
        }        

        await Task.CompletedTask;
    }
    public string[,] GetBushTexture()
    {
        string[,] blueBerryPlanted =
        {
            {"forage_blueberry_planted"},
        };

        string[,] blueBerryGerm =
        {
            {"forage_blueberry_young"},
        };

        string[,] blueBerryMature =
        {
            {"forage_blueberry_mature"},
        };

        string[,] blueBerryRipe =
        {
            {"forage_blueberry_ripe"},
        };

        string[,] chosenTexture;

        if (Type is PlantType.Blueberry)
        {
            chosenTexture = Status switch
            {
                GrowthProcess.Planted => blueBerryPlanted,
                GrowthProcess.Young => blueBerryGerm,
                GrowthProcess.Mature => blueBerryMature,
                GrowthProcess.Ripe => blueBerryRipe,
                _ => blueBerryRipe
            };
        }
        
        else chosenTexture = blueBerryRipe;

        return chosenTexture;
    }
}
