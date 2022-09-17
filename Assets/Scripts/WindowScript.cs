using UnityEngine;
using UnityEngine.EventSystems;

public class WindowScript : MonoBehaviour
{
    public Vector2Int defaultWindowSize;
    public Vector2Int borderSize;

    private Vector2 _deltaValue = Vector2.zero;
    private bool _maximized;

    void Awake()
    {
        if (!Application.isEditor)
        {
            Screen.SetResolution(1280, 800, false, 120);

            if (!BorderlessWindow.framed)
                return;

            BorderlessWindow.SetFramelessWindow();
            BorderlessWindow.MoveWindowPos(Vector2Int.zero, defaultWindowSize.x, defaultWindowSize.y);
            // BorderlessWindow.MoveWindowPos(Vector2Int.zero, Screen.width - borderSize.x, Screen.height - borderSize.y);
        }
    }
}
