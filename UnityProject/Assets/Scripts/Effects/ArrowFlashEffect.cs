using TMPro;  // Required for TextMeshPro
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowFlashEffect : MonoBehaviour
{
    public Material flashMaterial;

    public float flashDuration;
    public float labelDuration;
    public TextMeshProUGUI labelComponent;

    // The SpriteRenderer that should flash.
    public RawImage rawImage;
    
    // The material that was in use, when the script started.
    private Material originalMaterial;

    // The currently running coroutine.
    private Coroutine flashRoutine;
    private Coroutine labelRoutine;

    void Start()
    {
        originalMaterial = rawImage.material;
        labelComponent.alpha = 0f;
    }

    public void Flash(int label)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        if (labelRoutine != null)
            StopCoroutine(labelRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
        labelRoutine = StartCoroutine(LabelRoutine(label));
    }

    private IEnumerator FlashRoutine()
    {
        rawImage.material = flashMaterial;

        yield return new WaitForSeconds(flashDuration);

        rawImage.material = originalMaterial;

        flashRoutine = null;
    }

    private IEnumerator LabelRoutine(int label)
    {
        labelComponent.text = label.ToString();
        Color32 startColor = new Color32(255, 255, 255, 255);
        Color32 endColor = new Color32(255, 255, 255, 0);
        labelComponent.color = startColor;

        float elapsedTime = 0f;

        while (elapsedTime < labelDuration)
        {
            float t = elapsedTime / labelDuration; // Normalize time
            labelComponent.color = Color32.Lerp(startColor, endColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        labelComponent.color = endColor;

        labelRoutine = null;
    }
}
