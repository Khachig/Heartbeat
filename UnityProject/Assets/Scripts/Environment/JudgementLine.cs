using UnityEngine;

public class JudgementLine : MonoBehaviour
{
    private static SpriteRenderer sr;

    private void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        // DisableJudgementLine();
    }

    public static void EnableJudgementLine()
    {
        Color tmp = sr.color;
        tmp.a = 1f;
        sr.color = tmp;
    }

    public static void DisableJudgementLine()
    {
        Color tmp = sr.color;
        tmp.a = 0f;
        sr.color = tmp;
    }
}
