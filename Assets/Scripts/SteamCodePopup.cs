using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class SteamCodePopup : MonoBehaviour
    {
        public TextMesh Description;
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
        }

        public void EnterPressed()
        {
            callback.Invoke(codeField.text);
            Destroy(gameObject);
        }
    }
}
