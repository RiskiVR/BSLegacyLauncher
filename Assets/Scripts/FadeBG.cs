using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeBG : MonoBehaviour
{
    [SerializeField] Sprite[] backgrounds;
    [SerializeField] float timeBetweenSlides = 10f;
    [SerializeField] float crossfadeDuration = 3f;
    [SerializeField] Vector2 scale = new(1f, 1.2f);
    [SerializeField] Image a, b;

    private int currentIndex;
    private bool usingA;

    void Start()
    {
        if (backgrounds == null || backgrounds.Length == 0) return;

        a.sprite = backgrounds[0];
        a.color = Color.white;
        a.transform.localScale = Vector3.one * scale.x;

        b.color = new Color(1f, 1f, 1f, 0f);
        b.transform.localScale = Vector3.one * scale.x;

        usingA = true;
        ShowNext();
    }

    void ShowNext()
    {
        currentIndex = (currentIndex + 1) % backgrounds.Length;

        Image incoming = usingA ? b : a;
        Image outgoing = usingA ? a : b;

        incoming.sprite = backgrounds[currentIndex];
        incoming.color = new Color(1f, 1f, 1f, 0f);
        incoming.transform.localScale = Vector3.one * scale.x;

        incoming.DOFade(1f, crossfadeDuration);
        incoming.transform.DOScale(scale.y, timeBetweenSlides).SetEase(Ease.Linear);

        outgoing.DOFade(0f, crossfadeDuration);
        DOVirtual.DelayedCall(timeBetweenSlides, () =>
        {
            usingA = !usingA;
            ShowNext();
        });
    }

    void OnDestroy()
    {
        a.DOKill();
        b.DOKill();
    }
}