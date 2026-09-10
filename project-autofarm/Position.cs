using System;

namespace ProjectAutofarm;

record struct Position(int x, int y)
{
    public int X {get; set;} = x;
    public int Y {get; set;} = y;
}