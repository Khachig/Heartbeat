using UnityEngine;
using System.Collections.Generic;

public class EnemyRhythms : MonoBehaviour
{
    static private List<List<int>> allRhythms = new List<List<int>>() {
        new List<int>() {1, 3, 5, 7},
        new List<int>() {1, 5, 7},
        new List<int>() {1, 3, 5},
        new List<int>() {1, 5},
        new List<int>() {1, 3, 7},
        new List<int>() {1, 3, 4, 5, 7},
        new List<int>() {1, 3, 4, 5, 6, 7},
        new List<int>() {1, 2, 3, 5, 7, 8},
        new List<int>() {1, 2, 3, 4, 5, 7},
        new List<int>() {1, 2, 3, 5, 6, 7 },
        new List<int>() {1, 3, 4, 5, 7, 8},
        new List<int>() {1, 3, 5, 6, 7},
        new List<int>() {1, 3, 5, 6, 7, 8},
    };
    
    static private List<List<int>> easyRhythms = new List<List<int>>() {
        new List<int>() {1, 3, 5, 7},
        new List<int>() {1, 5, 7},
        new List<int>() {1, 3, 5},
        new List<int>() {1, 5},
        new List<int>() {1, 3, 7},
    };

    static private List<List<int>> tutorialRhythm = new List<List<int>>() {
        new List<int>() {1, 5}
    };

    public static List<int> GenerateRandomRhythm()
    {
        int idx = Random.Range(0, allRhythms.Count);
        return allRhythms[idx];
    }
    
    public static List<int> GenerateRandomEasyRhythm()
    {
        int idx = Random.Range(0, easyRhythms.Count);
        return easyRhythms[idx];
    }

    public static List<int> GenerateTutorialRhythm()
    {
        return tutorialRhythm[0];
    }
}
