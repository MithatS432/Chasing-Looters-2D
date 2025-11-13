using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightningEffect : MonoBehaviour
{
    [Header("Flash Settings")]
    public Image flashPanel;
    public float flashDuration = 0.1f;

    [Header("Bolt Settings")]
    public GameObject lightningBolt;
    public float boltDuration = 0.2f;

    [Header("Sound")]
    public AudioSource thunderAudio;

    [Header("Timing")]
    public float minInterval = 3f;
    public float maxInterval = 8f;

    [Header("Screen Shake")]
    public ScreenShake screenShake;

    void Start()
    {
        StartCoroutine(LightningRoutine());
    }

    IEnumerator LightningRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // Flash
            flashPanel.color = new Color(1, 1, 1, 1);

            if (screenShake != null)
                screenShake.TriggerShake();

            if (thunderAudio != null)
                thunderAudio.Play();

            yield return new WaitForSeconds(flashDuration);
            flashPanel.color = new Color(1, 1, 1, 0);

            // Lightning bolt
            if (lightningBolt != null)
            {
                lightningBolt.SetActive(true);
                yield return new WaitForSeconds(boltDuration);
                lightningBolt.SetActive(false);
            }
        }
    }
}
