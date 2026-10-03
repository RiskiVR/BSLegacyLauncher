using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class SteamCodePopup : MonoBehaviour
    {
        public TextMeshProUGUI Description;
        public InputField codeField;
        public Button EnterButton;

        [HideInInspector]
        public Action<string> callback;

        public void Start()
        {
            codeField.onEndEdit.AddListener(value =>
            {
                if (Input.GetKey(KeyCode.Return)) EnterPressed();
            });
            EnterButton.onClick.AddListener(EnterPressed);
        }

        public void EnterPressed()
        {
            callback.Invoke(codeField.text);
            Destroy(gameObject);
        }
    }
}
