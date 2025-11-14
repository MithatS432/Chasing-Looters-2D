using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal; // Light2D için gerekli


public class ScreenShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [Tooltip("Duration of the screen shake effect in seconds.")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;

    [Header("Scene Darken Settings")]
    public Image darkenPanel;
    public float darkenAmount = 0.5f;
    public float darkenDuration = 0.2f;

    private Vector3 originalPosition;
    private bool isShaking = false;

    [Header("Scene Light Flash")]
    public Light2D sceneLight;
    public float flashIntensity = 3f;
    public float flashTime = 0.05f;
    void Start()
    {
        originalPosition = transform.localPosition;
        if (darkenPanel != null)
            darkenPanel.color = new Color(0, 0, 0, 0);
    }

    public void TriggerShake()
    {
        if (!isShaking)
        {
            StartCoroutine(Shake());
            if (darkenPanel != null)
                StartCoroutine(TemporaryDarken());
            if (sceneLight != null)
                StartCoroutine(LightFlash());
        }
    }

    IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        isShaking = false;
    }

    IEnumerator TemporaryDarken()
    {
        darkenPanel.color = new Color(0, 0, 0, darkenAmount);
        yield return new WaitForSeconds(darkenDuration);
        darkenPanel.color = new Color(0, 0, 0, 0);
    }
    IEnumerator LightFlash()
    {
        float original = sceneLight.intensity;
        sceneLight.intensity = flashIntensity;
        yield return new WaitForSeconds(flashTime);
        sceneLight.intensity = original;
    }
}
