using System;

namespace ProjectAutofarm;

class Tile
{
    public Texture Texture {get; set;}
    public Position Position {get; set;}
    public Ressources Ressource {get; set;} = new Wheat(Ressources.GrowthProcess.None);
    public Tile((int x, int y) position, string textureName = "empty")
    {
        Texture = new Texture(textureName);
        Position = new Position(position.x, position.y);
    }

    public void TextureRefresher()
    {
        //Console.WriteLine("Test");

        if (Ressource != null)
        {
            if (Ressource.Type == Ressources.RessourceType.Wheat)
            {
                if (Ressource.Status == Ressources.GrowthProcess.Harvested) Texture.TextureName = "acre_empty";
                if (Ressource.Status == Ressources.GrowthProcess.Sown) Texture.TextureName = "acre_wheat_sown";
                if (Ressource.Status == Ressources.GrowthProcess.Young) Texture.TextureName = "acre_wheat_growing";
                if (Ressource.Status == Ressources.GrowthProcess.Mature) Texture.TextureName = "acre_wheat_mature";
                if (Ressource.Status == Ressources.GrowthProcess.Ripe) Texture.TextureName = "acre_wheat_ripe";
            }
        }

        Texture.TextureName = Texture.TextureName;
    }
}