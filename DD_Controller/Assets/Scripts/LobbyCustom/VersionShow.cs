using TMPro;
using UnityEngine;

namespace LobbyCustom
{
    public class VersionShow : MonoBehaviour
    {
        public TextMeshProUGUI versionText;
        void Start()
        {
            versionText.text = "v" + Application.version;
        }
    }
}

