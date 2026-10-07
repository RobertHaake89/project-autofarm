using System;

namespace ProjectAutofarm;

class Tile : ITargetable
{
    public Texture Texture {get; set;}
    public Position Position {get; set;}
    public Resource Resource {get; set;}
    public Tile((int x, int y) position, string textureName = "empty")
    {
        Texture = new Texture(textureName);
        Position = new Position(position.x, position.y);
        Resource = new Resource(ResourceType.None, GrowthProcess.Fallow, (position.x,position.y));
    }

    public void UpdateAcre()
    {
        if (Resource != null)
        {
            if (Resource.Type == ResourceType.Wheat)
            {
                Texture.TextureName = Resource.Status switch
                {   
                    GrowthProcess.Fallow => "bot_grass1",
                    GrowthProcess.Plowed => "bot_dirt1",
                    GrowthProcess.Sown => "acre_wheat_sown",
                    GrowthProcess.Young => "acre_wheat_growing",
                    GrowthProcess.Mature => "acre_wheat_mature",
                    GrowthProcess.Ripe => "acre_wheat_ripe",
                    GrowthProcess.Harvested => "bot_dirt1",
                    _ => "acre_empty"
                };
            }
        }

        Texture.TextureName = Texture.TextureName;
    }

    public void UpdateTrees()
    {
        if (Resource != null)
        {
            if (Resource.Type is ResourceType.SpruceWood or ResourceType.OakWood)
            {
                Texture.TextureName = Resource.Status switch
                {   
                    GrowthProcess.Sown => "tree_any_germinating",
                    GrowthProcess.Young => "tree_any_sprouting",
                    GrowthProcess.Mature => "tree_any_growing",
                    GrowthProcess.Ripe => "bot_grass1",
                    //GrowthProcess.Harvested => "bot_dirt1",
                    _ => "bot_grass1" // THIS NEEDS TO BE REWORKED!!
                };
            }
        }

        Texture.TextureName = Texture.TextureName;
    }
}