using TMPro;  // Required for TextMeshPro
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowFlashEffect : MonoBehaviour
{
    public Material flashMaterial;

    public float flashDuration;

    // The SpriteRenderer that should flash.
    public RawImage rawImage;
    
    // The material that was in use, when the script started.
    private Material originalMaterial;

    // The currently running coroutine.
    private Coroutine flashRoutine;

    void Start()
    {
        originalMaterial = rawImage.material;
    }

    public void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        rawImage.material = flashMaterial;

        yield return new WaitForSeconds(flashDuration);

        rawImage.material = originalMaterial;

        flashRoutine = null;
    }
}
