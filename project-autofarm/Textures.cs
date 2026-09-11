using System;
using System.IO;

namespace ProjectAutofarm;

static class Textures
{
    public static Dictionary<string, char> List {get;} = new Dictionary<string, char>();

    public static Dictionary<string, char> LoadTextures(Dictionary<string, char> dictionary,string filePath)
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

    private static string[] ReadTextureFile(string targetDirectory) => File.ReadAllLines(targetDirectory)[1..];

    public static char GetTexture(string typeKey) => List[typeKey];
    
}