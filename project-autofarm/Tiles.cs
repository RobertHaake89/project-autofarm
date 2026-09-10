using System;
using System.Reflection.PortableExecutable;

namespace ProjectAutofarm;

class Tile(string typeKey)
{
    public Dictionary<string, char> TypeDict {get; private set => SetDict();} = new Dictionary<string, char>();
    public char Type {get; set => SetType(typeKey);}
    //public Position Position {get; set;}

    private Dictionary<string, char> SetDict()
    {
        TypeDict.Add("dirt",' ');
        TypeDict.Add("grass",'░');

        return TypeDict;
    }
    private char SetType(string typeKey) => TypeDict[typeKey];
}