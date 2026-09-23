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
        Resource = new Resource(ResourceType.None, GrowthProcess.None, (position.x,position.y));
    }

    public void TextureRefresher()
    {
        if (Resource != null)
        {
            if (Resource.Type == ResourceType.Wheat)
            {
                Texture.TextureName = Resource.Status switch
                {
                    GrowthProcess.Harvested => "acre_empty",
                    GrowthProcess.Sown => "acre_wheat_sown",
                    GrowthProcess.Young => "acre_wheat_growing",
                    GrowthProcess.Mature => "acre_wheat_mature",
                    GrowthProcess.Ripe => "acre_wheat_ripe",
                    _ => "acre_empty"
                };
            }
        }

        Texture.TextureName = Texture.TextureName;
    }
}