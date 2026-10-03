using DG.Tweening;
using UnityEngine;

public class InitFade : MonoBehaviour
{
    void Start()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = 1;
        cg.DOFade(0, 3).SetDelay(0.8f).OnComplete(() => gameObject.SetActive(false));
    }
}