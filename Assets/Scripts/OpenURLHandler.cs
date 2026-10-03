using UnityEngine;
public class OpenURLHandler : MonoBehaviour
{
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}