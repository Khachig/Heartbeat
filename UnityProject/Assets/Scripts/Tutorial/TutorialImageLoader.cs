using UnityEngine;
using UnityEngine.UI;

public class TutorialImageLoader : MonoBehaviour
{
    public Image tutorialImage; // Assign in the Inspector
    public Sprite newSprite; // Assign your sprite in the Inspector

    void Start()
    {
        if (tutorialImage != null && newSprite != null)
        {
            tutorialImage.sprite = newSprite;
        }
    }
}
