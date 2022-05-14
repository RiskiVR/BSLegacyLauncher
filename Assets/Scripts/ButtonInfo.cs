using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isHighlightDesired = false;

    public GameObject Info;
    public Text InfoText;
    public string InfoData;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHighlightDesired = true;
        Info.SetActive(true);
        InfoText.text = $"{InfoData}";
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isHighlightDesired = false;
        Info.SetActive(false);
        InfoText.text = $"{InfoData}";
    }
}
