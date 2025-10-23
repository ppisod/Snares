using System;
using System.Numerics;

namespace ppilib.Utility;

public static class Vectors
{
    public static bool AreVectorsEqual (Vector2 v1, Vector2 v2, float e = 0.0001f)
    {
        return Math.Abs(v1.X - v2.X) < e && Math.Abs(v1.Y - v2.Y) < e;
    }
}