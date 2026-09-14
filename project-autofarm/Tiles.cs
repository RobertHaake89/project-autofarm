using System;

namespace ProjectAutofarm;

class Tile
{
    public Texture Texture {get; set;}// = new Texture();
    public Position Position {get; set;}/*  = new Position(position.x, position.y);
 */
    public Tile((int x, int y) position, string textureName = "empty")
    {
        Texture = new Texture(textureName);
        Position = new Position(position.x, position.y);
    }
    
    //public void SetTexture(string typeKey) => Texture = Texture.List[typeKey];
    
}