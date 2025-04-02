using UnityEngine;
using System.Collections.Generic;

public class EnemyRhythms : MonoBehaviour
{
    static private List<List<int>> sixRhythms = new List<List<int>>() {
        new List<int>() {1, 3, 4, 5, 6, 7},
        new List<int>() {1, 2, 3, 5, 7, 8},
        new List<int>() {1, 2, 3, 4, 5, 7},
        new List<int>() {1, 2, 3, 5, 6, 7 },
        new List<int>() {1, 3, 4, 5, 7, 8},
        new List<int>() {1, 3, 5, 6, 7, 8},
    };

    static private List<List<int>> fiveRhythms = new List<List<int>>() {
        new List<int>() {1, 3, 5, 6, 7},
        new List<int>() {1, 3, 4, 5, 7},
        new List<int>() {1, 2, 3, 5, 6},
        new List<int>() {1, 3, 5, 6, 7},
    };

    static private List<List<int>> fourRhythms = new List<List<int>>() {
        new List<int>() {1, 3, 5, 7},
    };

    
    static private List<List<int>> threeRhythms = new List<List<int>>() {
        new List<int>() {1, 5, 7},
        new List<int>() {1, 3, 5},
        new List<int>() {1, 3, 7},
    };

    static private List<List<int>> tutorialRhythm = new List<List<int>>() {
        new List<int>() {1, 5}
    };

    public static List<int> GenerateDifficultyMatchedRhythm(int difficulty){
        switch (difficulty)
        {
            case 3:
                return GenerateThreeRhythm();
            case 4:
            return GenerateFourRhythm();
            case 5:
            return GenerateFiveRhythm();
            case 6:
            return GenerateSixRhythm();
            default:
                return GenerateFourRhythm();
        }
    }

    public static List<int> GenerateSixRhythm()
    {
        int idx = Random.Range(0, sixRhythms.Count);
        return sixRhythms[idx];
    }

    public static List<int> GenerateFiveRhythm()
    {
        int idx = Random.Range(0, fourRhythms.Count);
        return fourRhythms[idx];
    }
    public static List<int> GenerateFourRhythm()
    {
        int idx = Random.Range(0, fourRhythms.Count);
        return fourRhythms[idx];
    }
    
    public static List<int> GenerateThreeRhythm()
    {
        int idx = Random.Range(0, threeRhythms.Count);
        return threeRhythms[idx];
    }

    public static List<int> GenerateTutorialRhythm()
    {
        return tutorialRhythm[0];
    }
}
