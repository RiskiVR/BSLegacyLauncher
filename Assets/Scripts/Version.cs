using DG.Tweening;
using TMPro;
using UnityEngine;

public class Version : MonoBehaviour
{
    void Start()
    {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        text.text = $"v{Application.version}";
        text.DOFade(0, 0);
        text.transform.DOLocalMoveX(-650, 0);
        text.DOFade(1, 1).SetDelay(0.1f);
        text.transform.DOLocalMoveX(-629, 0.5f).SetEase(Ease.OutExpo).SetDelay(0.1f);
    }
}