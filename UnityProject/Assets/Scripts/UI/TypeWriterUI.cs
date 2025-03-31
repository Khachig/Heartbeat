using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypeWriterUI : MonoBehaviour
{
	TextMeshProUGUI TmpProText;
	string writer;

	public float delayBeforeStart = 0f;
	public float timeBtwChars = 0.1f;
	public string leadingChar = "";
	public bool leadingCharBeforeDelay = false;

	// Use this for initialization
	void Start()
	{
		TmpProText = gameObject.GetComponent<TextMeshProUGUI>();
	}

    void OnEnable()
    {
		TmpProText = gameObject.GetComponent<TextMeshProUGUI>();

		if (TmpProText != null)
		{
			writer = TmpProText.text;
			TmpProText.text = "";

			StartCoroutine("TypeWriterTMP");
		}
    }

	IEnumerator TypeWriterTMP()
    {
        TmpProText.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(delayBeforeStart);

		foreach (char c in writer)
		{
			if (TmpProText.text.Length > 0)
			{
				TmpProText.text = TmpProText.text.Substring(0, TmpProText.text.Length - leadingChar.Length);
			}
			TmpProText.text += c;
			TmpProText.text += leadingChar;
			yield return new WaitForSeconds(timeBtwChars);
		}

		if (leadingChar != "")
		{
			TmpProText.text = TmpProText.text.Substring(0, TmpProText.text.Length - leadingChar.Length);
		}
	}
}
