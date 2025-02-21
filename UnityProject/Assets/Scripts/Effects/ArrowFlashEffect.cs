using System.Collections;

using UnityEngine;

public class ArrowFlashEffect : MonoBehaviour
{
    public Material flashMaterial;

    public float flashDuration;

    // The SpriteRenderer that should flash.
    public MeshRenderer meshRenderer;
    
    // The material that was in use, when the script started.
    private Material originalMaterial;

    // The currently running coroutine.
    private Coroutine flashRoutine;

    void Start()
    {
        originalMaterial = meshRenderer.material;
    }

    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        meshRenderer.material = flashMaterial;

        yield return new WaitForSeconds(flashDuration);

        meshRenderer.material = originalMaterial;

        flashRoutine = null;
    }
}
