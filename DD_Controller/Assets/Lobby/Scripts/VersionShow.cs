using TMPro;
using UnityEngine;

public class VersionShow : MonoBehaviour
{
    public TextMeshProUGUI versionText;
    void Start()
    {
        versionText.text = "v" + Application.version;
    }
}

