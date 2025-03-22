using UnityEngine;

public enum ArrowDirection : ushort {
    UP    = 0b_0000_0000_0000_0001,
    DOWN  = 0b_0000_0000_0001_0000,
    LEFT  = 0b_0000_0001_0000_0000,
    RIGHT = 0b_0001_0000_0000_0000,
}

public class ArrowDirections : MonoBehaviour
{
    public static ArrowDirection GetRandomArrowDirection()
    {
        var values = System.Enum.GetValues(typeof(ArrowDirection));
        int random = Random.Range(0, values.Length);
        return (ArrowDirection)values.GetValue(random);
    }
}
