using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioClip[] textSFXClips;

    private AudioSource audioSource;
    private Coroutine typingCoroutine;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void StartTypingSFX()
    {
        if (typingCoroutine != null) return; // already running
        typingCoroutine = StartCoroutine(TypingLoop());
    }

    public void StopTypingSFX()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        audioSource.Stop();
    }

    IEnumerator TypingLoop()
    {
        while (true)
        {
            AudioClip clip = textSFXClips[Random.Range(0, textSFXClips.Length)];

            audioSource.clip = clip;
            // audioSource.pitch = Random.Range(1.8f, 2f);
            audioSource.Play();

            yield return new WaitForSeconds(clip.length);
        }
    }
}
