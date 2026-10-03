using UnityEngine;
using UnityEngine.UI;

public class LaunchOptions : MonoBehaviour
{
    public Toggle oculusToggle;
    public Toggle verboseToggle;
    public static bool oculus;
    public static bool verbose;

    void Start()
    {
        oculusToggle.onValueChanged.AddListener(value =>
        {
            oculus = value;
        });
        verboseToggle.onValueChanged.AddListener(value =>
        {
            verbose = value;
        });
    }
}
