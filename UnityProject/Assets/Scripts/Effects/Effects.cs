using TMPro;  // Required for TextMeshPro
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class Effects : MonoBehaviour
{
	public GameObject canvas;
	public GameObject effectTextPrefab;
	public float popupDuration = 2f;
	public Material ScreenDamageMat;
	public CinemachineImpulseSource impulseSource;
    public float CameraShakeIntensity = 0.5f;
	private Coroutine screenDamageTask;
	private GameObject effectTextPopup;
	private Animator textAnimator;
	private bool isMessageOnScreen = false;
	private float timeSinceLastPopup;
	private Coroutine popupFadeRoutine;

	private static Effects instance;
 
	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		timeSinceLastPopup = Time.time; 
	}

	private void Update()
	{
		if (Time.time - timeSinceLastPopup > popupDuration)
			DestroyPopup();
	}

	private void PlayerHealEffect() 
	{
		SpawnPopup("Heal!");
	}

	private void ComboContinueEffect(int comboNum) 
	{
		SpawnPopup("Combo: " + comboNum);
	
	}

	private void ComboBreakEffect() 
	{
		SpawnPopup("Combo broken");
	}

	private void ScreenDamageEffect(float intensity) 
	{
		if(screenDamageTask != null)
			StopCoroutine(screenDamageTask);

		screenDamageTask = StartCoroutine(screenDamage(intensity));
	}

	private IEnumerator screenDamage(float intensity)
	{
		// Cinemachine Camera shake
		var velocity = new Vector3(0, -0.5f, -1);
		velocity.Normalize();
		impulseSource.GenerateImpulse(velocity * CameraShakeIntensity);
		// Screen Effect
		var targetRadius = Remap(intensity, 0, 1, 0.4f, -0.1f);
		var curRadius = 1f;
		for(float t = 0; curRadius != targetRadius; t += Time.deltaTime)
		{
			curRadius = Mathf.Clamp(Mathf.Lerp(1, targetRadius, t), 1, targetRadius);
			ScreenDamageMat.SetFloat("_vignette_radius", curRadius);
			yield return null;
		}
		for(float t = 0; curRadius < 1; t += Time.deltaTime)
		{
			curRadius = Mathf.Lerp(targetRadius, 1, t);
			ScreenDamageMat.SetFloat("_vignette_radius", curRadius);
			yield return null;
		}
	}

	private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
	{
		return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
	}

	private void SpawnPopup(string message)
	{
		if (popupFadeRoutine != null)
			StopCoroutine(popupFadeRoutine);

		if (!isMessageOnScreen)
		{
			isMessageOnScreen = true;
			Vector3 position = effectTextPrefab.transform.position;
			effectTextPopup = Instantiate(effectTextPrefab, position, Quaternion.identity) as GameObject;
			effectTextPopup.transform.SetParent(canvas.transform, false);
		}
		textAnimator = effectTextPopup.GetComponent<Animator>();
		textAnimator.SetTrigger("Pulse");
        TextMeshProUGUI textComponent = effectTextPopup.GetComponent<TextMeshProUGUI>();
        textComponent.text = message;
        popupFadeRoutine = StartCoroutine(PopupFadeRoutine(textComponent));
		timeSinceLastPopup = Time.time;
	}

	private void DestroyPopup()
	{
		if (!isMessageOnScreen)
			return;

		isMessageOnScreen = false;
		if (popupFadeRoutine != null)
			StopCoroutine(popupFadeRoutine);
		Destroy(effectTextPopup);
		effectTextPopup = null;
		textAnimator = null;
	}

	private IEnumerator PopupFadeRoutine(TextMeshProUGUI textComponent)
    {
        Color32 startColor = new Color32(255, 255, 255, 255);
        Color32 endColor = new Color32(255, 255, 255, 0);
        textComponent.color = startColor;

        float elapsedTime = 0f;

        while (elapsedTime < popupDuration)
        {
            float t = elapsedTime / popupDuration; // Normalize time
			textComponent.color = Color32.Lerp(startColor, endColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textComponent.color = endColor;

        popupFadeRoutine = null;
    }

	public static class SpecialEffects
	{
		public static void ScreenDamageEffect(float intensity) => instance.ScreenDamageEffect(intensity);
		public static void PlayerHealEffect() => instance.PlayerHealEffect();
		public static void ComboContinueEffect(int comboNum) => instance.ComboContinueEffect(comboNum);
		public static void ComboBreakEffect() => instance.ComboBreakEffect();
	}
}
