using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] musicList;

    private int currentIndex = 0;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (musicList.Length > 0)
            StartCoroutine(PlayMusicSequentially());
    }

    IEnumerator PlayMusicSequentially()
    {
        while (true)
        {
            audioSource.clip = musicList[currentIndex];
            audioSource.Play();

            yield return new WaitForSeconds(audioSource.clip.length);

            currentIndex++;

            if (currentIndex >= musicList.Length)
                currentIndex = 0;
        }
    }
}
