using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class ExitTrigger : MonoBehaviour
{
    private Image image;
    private void Start()
    {
        image = GetComponent<Image>();
        image.DOFade(0, 0);
        image.DOFade(1, 1).SetDelay(0.05f);
        transform.localPosition = new Vector2(700, transform.localPosition.y);
        transform.DOLocalMoveX(630, 1).SetEase(Ease.OutExpo);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}