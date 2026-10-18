using DG.Tweening;
using UnityEngine;

public class FocusMute : MonoBehaviour
{
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
        audioSource.DOFade(0.3f, 3);
    }
    void Update()
    {
        audioSource.mute = !Application.isFocused;
    }

    public void FadeOut()
    {
        audioSource.DOFade(0, 3);
    }
}
