using System.Runtime.CompilerServices;

namespace ProjectAutofarm;

public static class SupportMethod
{
    public static void Shuffle<Position>(this IList<Position> list)
    // Fisher-Yates Algorithm
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static int ClampToTerrainAxis(this int value, string axis)
    {
        if (axis is "x" or "X")
            return Math.Clamp(value, 0, Terrain.MaxSizeX - 1);
        if (axis is "y" or "Y")
            return Math.Clamp(value, 0, Terrain.MaxSizeY - 1);

        else return 0;

    }
}