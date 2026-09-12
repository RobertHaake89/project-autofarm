using System;
using System.IO;

namespace ProjectAutofarm;

class Texture
{
    public string TextureName {get; set;} = "";
    public char Icon => List[TextureName];
    public Dictionary<string, char> List {get;} = new();

    public Dictionary<string, char> LoadTextures(Dictionary<string, char> dictionary,string filePath)
    {
        string[] text = ReadTextureFile(filePath);
        string[] parts;
        /* int key = 0;
        int type = 1; */


        foreach (string line in text) 
        { 
            parts = line.Split(',');

            dictionary.Add(parts[0], char.Parse(parts[1]));

            /* key++;
            type++; */
        }

        return dictionary;
    }

    private string[] ReadTextureFile(string targetDirectory) => File.ReadAllLines(targetDirectory)[1..];

    //public char SetTexture(string typeKey) => Icon = List[typeKey];
    
}