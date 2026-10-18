using UnityEngine;
using UnityEngine.EventSystems;

public class Unselectable : MonoBehaviour, ISelectHandler
{
    private bool _suspectedSelected;
    public void OnSelect(BaseEventData eventData)
    {
        _suspectedSelected = true;
    }
    private void Update()
    {
        if (!_suspectedSelected) return;
        if (EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);

        _suspectedSelected = false;
    }
}