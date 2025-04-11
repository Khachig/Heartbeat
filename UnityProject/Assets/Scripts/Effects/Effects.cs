using TMPro;  // Required for TextMeshPro
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class Effects : MonoBehaviour
{
	public GameObject canvas;
	public GameObject effectTextPrefab;
	public GameObject comboTextPrefab;
	public GameObject missTextPrefab;
	public float popupDuration = 2f;
	public Material ScreenDamageMat;
	public Image ScreenDamageImage;
	public CinemachineImpulseSource impulseSource;
    public float CameraShakeIntensity = 0.5f;
	private Coroutine screenDamageTask;
	private GameObject effectTextPopup;
	private GameObject comboTextPopup;
	private GameObject missTextPopup;
	private Animator textAnimator;
	private Dictionary<GameObject, Coroutine> popupFadeRoutines;

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
		ScreenDamageMat.SetFloat("_vignette_radius", 1f);
		ScreenDamageImage.color = GetNewAlphaForColor(ScreenDamageImage.color, 0f);
		popupFadeRoutines = new Dictionary<GameObject, Coroutine>();
	}

	private void PlayerHealEffect() 
	{
		SpawnPopup("PERFECT WAVE!\nHEAL!", effectTextPrefab, ref effectTextPopup);
	}

	private void ComboContinueEffect(int comboNum) 
	{
		SpawnPopup("COMBO: " + comboNum, comboTextPrefab, ref comboTextPopup, false);
	}

	private void ComboBreakEffect() 
	{
		SpawnPopup("COMBO BROKEN", comboTextPrefab, ref comboTextPopup);
	}

	private void ResetComboText()
	{
		if (comboTextPopup != null)
			comboTextPopup.GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 0);
	}

	private void MissEffect() 
	{
		SpawnPopup("MISS", missTextPrefab, ref missTextPopup);
	}

	private void MultiplierEffect(int multiplierNum) 
	{
		SpawnPopup("X" + multiplierNum, missTextPrefab, ref missTextPopup);
	}

	private void ScreenDamageEffect(float intensity) 
	{
		if(screenDamageTask != null)
			StopCoroutine(screenDamageTask);

		screenDamageTask = StartCoroutine(screenDamage(intensity));
	}

	private void ResetScreenDamageEffect()
	{
		ScreenDamageMat.SetFloat("_vignette_radius", 1f);
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
		var curImageAlpha = 0f;
		for(float t = 0; curRadius != targetRadius; t += Time.deltaTime)
		{
			curRadius = Mathf.Clamp(Mathf.Lerp(1, targetRadius, t), 1, targetRadius);
			ScreenDamageMat.SetFloat("_vignette_radius", curRadius);
			
			curImageAlpha = Mathf.Lerp(0f, 1f, t);
			ScreenDamageImage.color = GetNewAlphaForColor(ScreenDamageImage.color, curImageAlpha);

			yield return null;
		}
		for(float t = 0; curRadius < 1; t += Time.deltaTime)
		{
			curRadius = Mathf.Lerp(targetRadius, 1, t);
			ScreenDamageMat.SetFloat("_vignette_radius", curRadius);

			curImageAlpha = Mathf.Lerp(1f, 0f, t);
			ScreenDamageImage.color = GetNewAlphaForColor(ScreenDamageImage.color, curImageAlpha);

			yield return null;
		}
	}

	private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
	{
		return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
	}

	private void SpawnPopup(string message, GameObject prefab, ref GameObject textPopup, bool fadeOut = true)
	{
		if (!textPopup)
		{
			textPopup = Instantiate(prefab, prefab.transform.position, Quaternion.identity) as GameObject;
			textPopup.transform.SetParent(canvas.transform, false);
			popupFadeRoutines.Add(textPopup, null);
		} else if (popupFadeRoutines[textPopup] != null)
			StopCoroutine(popupFadeRoutines[textPopup]);

		textAnimator = textPopup.GetComponent<Animator>();
		textAnimator.SetTrigger("Pulse");
        TextMeshProUGUI textComponent = textPopup.GetComponent<TextMeshProUGUI>();
        textComponent.color = new Color32(255, 255, 255, 255);
        textComponent.text = message;
		if (fadeOut)
			popupFadeRoutines[textPopup] = StartCoroutine(PopupFadeRoutine(textComponent));
		else
			popupFadeRoutines[textPopup] = null;

	}

	private IEnumerator PopupFadeRoutine(TextMeshProUGUI textComponent)
    {
        Color32 startColor = textComponent.color;
        Color32 endColor = new Color32(255, 255, 255, 0);

        float elapsedTime = 0f;

        while (elapsedTime < popupDuration)
        {
            float t = elapsedTime / popupDuration; // Normalize time
			textComponent.color = Color32.Lerp(startColor, endColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textComponent.color = endColor;

        popupFadeRoutines[textComponent.gameObject] = null;
    }

	private Color GetNewAlphaForColor(Color color, float newAlpha)
	{
		Color newColor = color;
		newColor.a = newAlpha;
		return newColor;           
	}

	private void OnDisable()
	{
		ResetScreenDamageEffect();
	}

	public static class SpecialEffects
	{
		public static void ResetScreenDamageEffect() => instance.ResetScreenDamageEffect();
		public static void ScreenDamageEffect(float intensity) => instance.ScreenDamageEffect(intensity);
		public static void PlayerHealEffect() => instance.PlayerHealEffect();
		public static void ComboContinueEffect(int comboNum) => instance.ComboContinueEffect(comboNum);
		public static void ComboBreakEffect() => instance.ComboBreakEffect();
		public static void ResetComboText() => instance.ResetComboText();
		public static void MissEffect() => instance.MissEffect();
		public static void MultiplierEffect(int multiplierNum) => instance.MultiplierEffect(multiplierNum);
	}
}
