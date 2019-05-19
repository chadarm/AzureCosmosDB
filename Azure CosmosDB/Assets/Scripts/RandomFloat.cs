using UnityEngine;

public class RandomFloat
{
    private static readonly float MinValue = -250000;
    private static readonly float MaxValue = 250000;


    public static float NextFloat()
    {
        return Random.Range(MinValue, MaxValue);
    }
}
