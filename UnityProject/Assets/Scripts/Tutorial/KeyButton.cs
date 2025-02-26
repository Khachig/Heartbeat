using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyHighlighter : MonoBehaviour
{
    private Image buttonImage;
    private TMP_Text buttonText;
    public Color normalColor = Color.gray;
    public Color highlightColor = Color.yellow;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TMP_Text>();

        buttonImage.color = normalColor; // Set default button color
    }

    void Update()
    {
        if (buttonText == null || string.IsNullOrEmpty(buttonText.text)) return;

        KeyCode keyCode = GetKeyCodeFromText(buttonText.text);

        if (keyCode != KeyCode.None)
        {
            if (Input.GetKeyDown(keyCode))
            {
                buttonImage.color = highlightColor; // Light up when key is pressed
            }
            if (Input.GetKeyUp(keyCode))
            {
                buttonImage.color = normalColor; // Return to normal color
            }
        }
    }

    KeyCode GetKeyCodeFromText(string text)
    {
        switch (text)
        {
            case "↑": return KeyCode.UpArrow;
            case "←": return KeyCode.LeftArrow;
            case "→": return KeyCode.RightArrow;
            case "↓": return KeyCode.DownArrow;
            default:
                if (System.Enum.TryParse(text.ToUpper(), out KeyCode key))
                {
                    return key;
                }
                break;
        }
        return KeyCode.None;
    }
}
