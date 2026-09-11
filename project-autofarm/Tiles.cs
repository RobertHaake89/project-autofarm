using System;

namespace ProjectAutofarm;

class Tile(string typeKey, (int x, int y) position)
{
    public char Texture {get; private set;} = Textures.GetTexture(typeKey);
    public Position Position {get; set;} = new Position(position.x, position.y);
    
    public void SetTexture(string typeKey) => Texture = Textures.List[typeKey];
    
}