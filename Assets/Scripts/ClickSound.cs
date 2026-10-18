using UnityEngine;
public class ClickSound : MonoBehaviour
{
    private static ClickSound instance;
    AudioSource audioSource;
    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }
    public static void Play() => instance.audioSource.Play();
}