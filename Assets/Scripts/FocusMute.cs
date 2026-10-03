using DG.Tweening;
using UnityEngine;

public class FocusMute : MonoBehaviour
{
    AudioSource _audioSource;
    void Awake() => _audioSource = GetComponent<AudioSource>();
    void Start()
    {
        _audioSource.volume = 0;
        _audioSource.DOFade(0.3f, 2);
    }
    void Update()
    {
        _audioSource.mute = !Application.isFocused;
    }
}
