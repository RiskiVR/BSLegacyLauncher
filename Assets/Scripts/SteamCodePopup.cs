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
        public SteamLoginResponse request;

        [HideInInspector]
        public Action<string, SteamLoginResponse> callback;

        public void EnterPressed()
        {
            callback.Invoke(codeField.text, request);
            GameObject.Destroy(gameObject);
        }
    }
}
