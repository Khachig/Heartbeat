using UnityEngine;

public class JudgementLine : MonoBehaviour
{
    private static SpriteRenderer downArrowIndicator;
    private static SpriteRenderer leftArrowIndicator;
    private static SpriteRenderer rightArrowIndicator;
    private static SpriteRenderer upArrowIndicator;
    private static SpriteRenderer sr;
    private static bool isEnabled = true;
    private static int activeLane = -1;
    private static float disabledAlpha = 0.04f;
    private static float activeAlpha = 0.5f;

    private void Start()
    {
        sr = transform.GetChild(4).GetComponent<SpriteRenderer>();
        downArrowIndicator = transform.GetChild(0).GetComponent<SpriteRenderer>();
        rightArrowIndicator = transform.GetChild(1).GetComponent<SpriteRenderer>();
        upArrowIndicator = transform.GetChild(2).GetComponent<SpriteRenderer>();
        leftArrowIndicator = transform.GetChild(3).GetComponent<SpriteRenderer>();
    }
    
    public static bool IsEnabled() { return isEnabled;}
    public static void EnableJudgementLine()
    {
        if (isEnabled)
            return;

        isEnabled = true;
        Color tmp = sr.color;
        tmp.a = 1f;
        sr.color = tmp;
        EnableIndicators();
    }

    public static void DisableJudgementLine()
    {
        if (!isEnabled)
            return;

        isEnabled = false;
        Color tmp = sr.color;
        tmp.a = 0f;
        sr.color = tmp;
        DisableIndicators();
    }

    public static void ActivateIndicatorAtLane(int lane)
    {
        if (!isEnabled)
            return;

        DeactivateIndicators();
        SpriteRenderer sr = GetIndicatorForLane(lane);
        Color tmp = sr.color;
        tmp.a = activeAlpha;
        sr.color = tmp;

        activeLane = lane;
    }

    public static void DeactivateIndicators()
    {
        if (!isEnabled)
            return;

        if (activeLane < 0)
            return;

        SpriteRenderer sr = GetIndicatorForLane(activeLane);
        Color tmp = sr.color;
        tmp.a = disabledAlpha;
        sr.color = tmp;

        activeLane = -1;
    }

    private static void DisableIndicators()
    {
        for (int i = 0; i < 4; i++)
        {
            SpriteRenderer sr = GetIndicatorForLane(i);
            Color tmp = sr.color;
            tmp.a = 0f;
            sr.color = tmp;           
        }
    }

    private static void EnableIndicators()
    {
        for (int i = 0; i < 4; i++)
        {
            SpriteRenderer sr = GetIndicatorForLane(i);
            Color tmp = sr.color;
            tmp.a = disabledAlpha;
            sr.color = tmp;           
        }
    }

    private static SpriteRenderer GetIndicatorForLane(int lane)
    {
        if (lane == 0)
            return downArrowIndicator;
        else if (lane == 1)
            return rightArrowIndicator;
        else if (lane == 2)
            return upArrowIndicator;
        else if (lane == 3)
            return leftArrowIndicator;
        else 
            return null;
    }
}
