using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    public float duration = 0.25f;
    private Image i;
    private float amount;
    private readonly string property = "_Opacity";
    void Start()
    {
        i = GetComponent<Image>();
        Reset();
    }

    void Reset()
    {
        i.materialForRendering.SetFloat(property, 1);
        i.fillAmount = 0;
        amount = 0;
    }

    public void SetAmount(float value)
    {
        amount = value;
        i.DOFillAmount(amount, duration).SetEase(Ease.OutCubic);
        if (amount >= 1) Complete();
    }

    public void Complete()
    {
        i.DOFillAmount(1, duration).OnComplete(() => i.materialForRendering.DOFloat(0, property, 0.5f).OnComplete(Reset));
    }
}