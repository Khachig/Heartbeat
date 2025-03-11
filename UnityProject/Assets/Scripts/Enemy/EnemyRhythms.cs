using UnityEngine;
using System.Collections.Generic;

public class EnemyRhythms : MonoBehaviour
{
    static private List<List<int>> allRhythms = new List<List<int>>() {
        new List<int>() {1, 2, 3, 4},
        new List<int>() {1, 3, 4},
        new List<int>() {1, 2, 3},
        new List<int>() {1, 3},
        new List<int>() {1, 2, 4},
    };

    public static List<int> GenerateRandomRhythm()
    {
        int idx = Random.Range(0, allRhythms.Count);
        return allRhythms[idx];
    }
}
