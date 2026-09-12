using System;

namespace ProjectAutofarm;

class Tile((int x, int y) position)
{
    public Texture Texture {get; set;} = new Texture();
    public Position Position {get; set;} = new Position(position.x, position.y);
    
    //public void SetTexture(string typeKey) => Texture = Texture.List[typeKey];
    
}