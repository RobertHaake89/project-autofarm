using System;

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
}