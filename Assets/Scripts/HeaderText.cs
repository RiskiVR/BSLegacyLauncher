using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HeaderText : MonoBehaviour
{
    private static HeaderText i;
    private CanvasGroup cg;
    private Text text;
    public Image bsl;
    private void Awake()
    {
        i = this;
        text = GetComponent<Text>();
        cg = GetComponent<CanvasGroup>();
        bsl.DOFade(0, 0);
        bsl.transform.DOScale(0.3f, 0);
        Display("");
    }
    public static void Display(string text)
    {
        i.BSLLogoScale(text.Length == 0);
        if (i.text.text == text) return;
        i.TextAnim(text);
    }

    void BSLLogoScale(bool a)
    {
        if (a)
        {
            bsl.DOFade(1, 0.5f).SetDelay(0.1f);
            bsl.transform.DOScale(0.24f, 1).SetDelay(0.1f).SetEase(Ease.OutExpo);
        }
        else
        {
            bsl.DOFade(0, 0.25f);
            bsl.transform.DOScale(0.3f, 0.3f).SetEase(Ease.InExpo);
        }
    }

    void TextAnim(string input)
    {
        i.cg.DOFade(0, 0.25f);
        i.text.transform.DOLocalMoveY(350, 0.25f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            cg.DOFade(1, 0.25f);
            text.text = input.ToUpper();
            text.transform.DOLocalMoveY(285, 0.3f).SetEase(Ease.OutExpo);
        });
    }
}