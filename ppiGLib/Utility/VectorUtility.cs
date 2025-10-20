using System;
using Microsoft.Xna.Framework;

namespace ppiGLib;

public class VectorUtility
{
    public static bool AreVectorsEqual (Vector2 v1, Vector2 v2, float e = 0.0001f)
    {
        return Math.Abs(v1.X - v2.X) < e && Math.Abs(v1.Y - v2.Y) < e;
    }
}